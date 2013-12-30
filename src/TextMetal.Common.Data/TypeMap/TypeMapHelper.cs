/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

using TextMetal.Common.Data.TypeMap.LowLevel;

namespace TextMetal.Common.Data.TypeMap
{
	/// <summary>
	/// Provides static helper and/or extension methods for TypeMap O/RM.
	/// </summary>
	public static class TypeMapHelper
	{
		#region Methods/Operators

		private static void AssertCommand(Command command, DataOperation dataOperation, Type targetType)
		{
			if ((object)targetType == null)
				throw new ArgumentNullException("targetType");

			if ((object)command == null)
				throw new InvalidOperationException(string.Format("Failed to obtain data source command '{1}' for the type '{0}' from the data source mapping.", targetType.FullName, dataOperation));

			if ((object)command.Parameters == null)
				throw new InvalidOperationException(string.Format("Failed to obtain data source command '{1}' parameters for the type '{0}' from the data source mapping.", targetType.FullName, dataOperation));

			if ((object)command.Fields == null)
				throw new InvalidOperationException(string.Format("Failed to obtain data source command '{1}' fields for the type '{0}' from the data source mapping.", targetType.FullName, dataOperation));
		}

		private static string GetCommandString(Command command, CommandBehavior behavior, DataOperation operation, ObjectState? objectState, bool? isNew)
		{
			string value = "";
			int i;

			if ((object)command == null)
				return value;

			value += "[begin]\r\n";

			value += string.Format("{0} =>\r\nOperation:\t{1}\r\nObjectState:\t{2}\r\nIsNew:\t{3}", command.GetType().FullName, operation, objectState, isNew);
			value += string.Format("COMMAND =>\r\nType:\t{0}\r\nText:\t{1}\r\nPrepare:\t{2}\r\nTimeout:\t{3}\r\nIdentity:\t{4}\r\nBehavior:\t{5}\r\n",
			                       command.Type, command.Text, command.Prepare, command.Timeout, command.Identity, behavior);

			i = 0;
			foreach (Parameter parameter in command.Parameters)
			{
				value += string.Format("PARAMETER[{0}] =>\r\nDirection:\t{1}\r\nName:\t{2}\r\nNullablet:\t{3}\r\nPrecision:\t{4}\r\nProperty:\t{5}\r\nScale:\t{6}\r\nSize:\t{7}\r\nType:\t{8}\r\n",
				                       i++, parameter.Direction, parameter.Name, parameter.Nullable, parameter.Precision, parameter.Property, parameter.Scale, parameter.Size, parameter.Type);
			}

			i = 0;
			foreach (Field field in command.Fields)
			{
				value += string.Format("FIELD[{0}] =>\r\nName:\t{1}\r\nProperty:\t{2}\r\n",
				                       i++, field.Name, field.Property);
			}

			value += "[end]\r\n";

			return value;
		}

		/// <summary>
		/// Performs an insert, update, or delete depending on the object state.
		/// The object must implement the IPlainObject interface.
		/// </summary>
		/// <typeparam name="TPlainObject">The type of object to perform operations on.</typeparam>
		/// <param name="plainObject">An object to save.</param>
		/// <returns>The data operation performed on the specified object.</returns>
		public static DataOperation ExecutePersistObject<TPlainObject>(this IUnitOfWorkContext unitOfWorkContext, TPlainObject plainObject) where TPlainObject : class, IPlainObject, new()
		{
			Type targetType;

			targetType = typeof(TPlainObject);

			return ExecutePersistObject(unitOfWorkContext, targetType, plainObject);
		}

		/// <summary>
		/// Performs an insert, update, or delete depending on the object state.
		/// The object must implement the IPlainObject interface.
		/// </summary>
		/// <param name="targetType">The type of object to perform operations on.
		/// The type specificed must be assignable to IPlainObject.</param>
		/// <param name="plainObject">Ahe plain object to save.</param>
		/// <returns>The data operation performed on the specified object.</returns>
		public static DataOperation ExecutePersistObject(this IUnitOfWorkContext unitOfWorkContext, Type targetType, IPlainObject plainObject)
		{
			CommandBehavior commandBehavior = CommandBehavior.Default;
			IDbConnection dbConnection = null;
			IDbTransaction dbTransaction;
			DataOperation dataOperation;
			ObjectState objectState;

			DataSourceMap dataSourceMap;
			Command command;
			List<IDataParameter> dataParameters;
			IDataReader dataReader;

			IEnumerable foreignPlainObjects;
			DataOperation foreignDataOperation;

			try
			{
				if ((object)targetType == null)
					throw new ArgumentNullException("targetType");

				if ((object)plainObject == null)
					throw new ArgumentNullException("plainObject");

				dataSourceMap = this.dataSourceMapFactory.GetMap(targetType, plainObject);

				if ((object)dataSourceMap == null)
					throw new InvalidOperationException(string.Format("Failed to obtain data source mapping for the type '{0}' from the data source mapping factory.", targetType.FullName));

				objectState = plainObject.ObjectState;
				dataOperation = PlainObject.DetermineBefore(plainObject);

				if (dataOperation == DataOperation.None)
					return dataOperation;

				if (dataOperation == DataOperation.Delete)
					command = dataSourceMap.Delete;
				else if (dataOperation == DataOperation.Insert)
					command = dataSourceMap.Insert;
				else if (dataOperation == DataOperation.Update)
					command = dataSourceMap.Update;
				else
					throw new InvalidOperationException(string.Format("Invalid CRUD data operation '{0}' encountered during persist operation.", dataOperation));

				AssertCommand(command, dataOperation, targetType);

				dataParameters = new List<IDataParameter>();

				MappingUtils.MapObjectToInputParameters(dataParameters, command, dataOperation, plainObject, this);
				MappingUtils.MapObjectToOutputParameters(dataParameters, command, dataOperation, plainObject, this);

				// get connection and optional local transaction
				this.GetConnectionAndTransaction(out dbConnection, out dbTransaction);

				commandBehavior = base.ShouldDisposeResources ? CommandBehavior.CloseConnection : CommandBehavior.Default;

				// +++++++++++++++++++++++++++++++++++++

				if (dataOperation == DataOperation.Delete)
				{
					foreach (Relationship oneSideOfOneToManyRelationship in dataSourceMap.Relationships.Where(r => r.Kind == "OneSideOfOneToMany"))
					{
						foreignPlainObjects = MappingUtils.MapForeignObjectsFromObject(oneSideOfOneToManyRelationship, dataOperation, plainObject);

						foreach (IPlainObject foreignPlainObject in foreignPlainObjects)
						{
							foreignPlainObject.ObjectState = ObjectState.Removed;

							foreignDataOperation = this.ExecutePersistObject(foreignPlainObject.GetType(), foreignPlainObject);

							if (foreignDataOperation != DataOperation.Delete)
								throw new InvalidOperationException(string.Format("Invalid foreign data operation '{0}' encountered during persist operation.", foreignDataOperation));
						}
					}
				}

				// +++++++++++++++++++++++++++++++++++++

				if (DataType.IsNullOrWhiteSpace(command.Text))
					throw new InvalidOperationException(string.Format("An null or zero length command text value was encountered during persist operation."));

				Debug.WriteLine(GetCommandString(command, commandBehavior, dataOperation, plainObject.ObjectState, plainObject.IsNew));
				using (dataReader = this.ExecuteReader(command.Type, command.Text, dataParameters.ToArray(), commandBehavior, command.Timeout, command.Prepare))
				{
					if (dataReader.Read())
						MappingUtils.MapObjectFromRecord(dataReader, command, dataOperation, plainObject);

					if (dataReader.Read())
						throw new InvalidOperationException(string.Format("A row count greater than one encountered from the data source mapping for the type '{0}' from the data source mapping factory.", targetType.FullName));
				}

				if (dataReader.RecordsAffected != 1) // concurrency check
					return DataOperation.ChangeConflict;
				//throw new InvalidOperationException(string.Format("An unexpected records affected count of '{0}' encountered during persist operation. Expected value was '{1}'.", dataReader.RecordsAffected, 1));

				// per MSDN, output parameters after reader close
				MappingUtils.MapObjectFromOutputParameters(dataParameters, command, dataOperation, plainObject);

				// For OLE/DB+JET+Access+4.0
				if (!DataType.IsNullOrWhiteSpace(command.Identity))
				{
					if ((object)DataSourceTransaction.Current == null)
						throw new InvalidOperationException("An ambient data source transaction on the current thread and application domain is required to ensure single connection execution of batch identity fetch.");

					Debug.WriteLine(GetCommandString(command, commandBehavior, DataOperation.SelectId, plainObject.ObjectState, plainObject.IsNew));
					using (dataReader = this.ExecuteReader(command.Type, command.Identity, null, commandBehavior, command.Timeout, command.Prepare)) // no parameters
					{
						if (dataReader.Read())
							MappingUtils.MapObjectFromRecord(dataReader, command, dataOperation, plainObject);

						if (dataReader.Read())
							throw new InvalidOperationException(string.Format("A row count greater than one encountered from the data source mapping for the type '{0}' from the data source mapping factory.", targetType.FullName));
					}

					if (dataReader.RecordsAffected != ExpectedRecordsAffected)
						throw new InvalidOperationException(string.Format("An unexpected records affected count of '{0}' encountered during query operation. Expected value was '{1}'.", dataReader.RecordsAffected, ExpectedRecordsAffected));

					// per MSDN, output parameters after reader close
					// n/a
				}

				// +++++++++++++++++++++++++++++++++++++

				if (dataOperation == DataOperation.Insert ||
				    dataOperation == DataOperation.Update)
				{
					foreach (Relationship oneSideOfOneToManyRelationship in dataSourceMap.Relationships.Where(r => r.Kind == "OneSideOfOneToMany"))
					{
						foreignPlainObjects = MappingUtils.MapForeignObjectsFromObject(oneSideOfOneToManyRelationship, dataOperation, plainObject);

						foreach (IPlainObject foreignPlainObject in foreignPlainObjects)
						{
							foreignDataOperation = this.ExecutePersistObject(foreignPlainObject.GetType(), foreignPlainObject);

							if (dataOperation == DataOperation.Insert)
							{
								if (foreignDataOperation != DataOperation.Insert &&
								    foreignDataOperation != DataOperation.None)
									throw new InvalidOperationException(string.Format("Invalid foreign data operation '{0}' encountered during persist operation.", foreignDataOperation));
							}
							else
							{
								if (foreignDataOperation != DataOperation.Insert &&
								    foreignDataOperation != DataOperation.Update &&
								    foreignDataOperation != DataOperation.None)
									throw new InvalidOperationException(string.Format("Invalid foreign data operation '{0}' encountered during persist operation.", foreignDataOperation));
							}
						}
					}
				}

				// +++++++++++++++++++++++++++++++++++++

				plainObject.ObjectState = PlainObject.DetermineAfter(plainObject);

				if (objectState == plainObject.ObjectState)
					throw new InvalidOperationException(string.Format("Invalid object state '{0}' encountered during persist operation.", objectState));

				if (dataOperation == DataOperation.Delete)
					plainObject.IsNew = true;

				return dataOperation;
			}
			finally
			{
				// DO NOT DISPOSE OF TRANSACTION

				// CommandBehavior.CloseConnection => DO NOT CLOSE CONNECTION: when caller closes reader, connection will be closed.

				// destroy and tear-down the connection
				if (this.ShouldDisposeResources && (object)dbConnection != null &&
				    ((commandBehavior & CommandBehavior.CloseConnection) != CommandBehavior.CloseConnection))
				{
					dbConnection.Dispose();
					dbConnection = null;
				}
			}
		}

		/// <summary>
		/// Find objects matching the implementation specific parameter;
		/// if parameter is a null, then all objects are returned.
		/// </summary>
		/// <typeparam name="TPlainObject">The type of object to perform operations on.</typeparam>
		/// <param name="criteriaObject">The criteria object to use in querying.</param>
		/// <returns>A list of objects matching the specified criteria parameter.</returns>
		public static IList<TPlainObject> ExecuteQueryObjects<TPlainObject>(this IUnitOfWorkContext unitOfWorkContext, ICriteriaObject criteriaObject) where TPlainObject : class, IPlainObject, new()
		{
			Type targetType;
			IList<IPlainObject> result;
			List<TPlainObject> retval;

			targetType = typeof(TPlainObject);

			result = ExecuteQueryObjects(unitOfWorkContext, targetType, criteriaObject);

			if ((object)result == null)
				return null;

			retval = new List<TPlainObject>();

			foreach (TPlainObject plainObject in result)
				retval.Add(plainObject);

			return retval;
		}

		/// <summary>
		/// Find objects matching the implementation specific parameter;
		/// if parameter is a null, then all objects are returned.
		/// </summary>
		/// <param name="targetType">The type of object to perform operations on.
		/// The type specificed must be assignable to IPlainObject.</param>
		/// <param name="criteriaObject">The criteria object to use in querying.</param>
		/// <returns>A list of plain objects matching the specified criteria parameter.</returns>
		public static IList<IPlainObject> ExecuteQueryObjects(this IUnitOfWorkContext unitOfWorkContext, Type targetType, ICriteriaObject criteriaObject)
		{
			CommandBehavior commandBehavior = CommandBehavior.Default;
			IDbConnection dbConnection = null;
			IDbTransaction dbTransaction;
			List<IPlainObject> plainObjects;
			IPlainObject plainObject;

			DataSourceMap dataSourceMap;
			Command command;
			List<IDataParameter> dataParameters;
			IDataReader dataReader;
			DataOperation dataOperation;

			try
			{
				if ((object)targetType == null)
					throw new ArgumentNullException("targetType");

				dataSourceMap = this.dataSourceMapFactory.GetMap(targetType, criteriaObject);

				if ((object)dataSourceMap == null)
					throw new InvalidOperationException(string.Format("Failed to obtain data source mapping for the type '{0}' from the data source mapping factory.", targetType.FullName));

				if ((object)criteriaObject == null)
				{
					dataOperation = DataOperation.SelectAll;
					command = dataSourceMap.SelectAll;
				}
				else
				{
					string id;
					For @for;

					dataOperation = DataOperation.SelectFor;

					id = criteriaObject.SelectForId;

					if ((object)id == null)
						throw new InvalidOperationException(string.Format("Failed to obtain data source command '{1}' under the for identifier '{2}' for the type '{0}' from the data source mapping.", targetType.FullName, dataOperation, id));

					@for = dataSourceMap.SelectFors.SingleOrDefault(t => t.Id == id);

					if ((object)@for == null)
						throw new InvalidOperationException(string.Format("Failed to obtain data source command '{1}' under the for identifier '{2}' for the type '{0}' from the data source mapping.", targetType.FullName, dataOperation, id));

					command = @for.Command;
				}

				AssertCommand(command, dataOperation, targetType);

				dataParameters = new List<IDataParameter>();

				if ((object)criteriaObject != null)
				{
					MappingUtils.MapObjectToInputParameters(dataParameters, command, dataOperation, criteriaObject, this);
					MappingUtils.MapObjectToOutputParameters(dataParameters, command, dataOperation, criteriaObject, this);
				}
				else
				{
					MappingUtils.MapObjectToInputParameters(dataParameters, command, dataOperation, this);
					MappingUtils.MapObjectToOutputParameters(dataParameters, command, dataOperation, this);
				}

				plainObjects = new List<IPlainObject>();

				// get connection and optional local transaction
				this.GetConnectionAndTransaction(out dbConnection, out dbTransaction);

				commandBehavior = base.ShouldDisposeResources ? CommandBehavior.CloseConnection : CommandBehavior.Default;

				if (DataType.IsNullOrWhiteSpace(command.Text))
					throw new InvalidOperationException(string.Format("An null or zero length command text value was encountered during persist operation."));

				Debug.WriteLine(GetCommandString(command, commandBehavior, dataOperation, null, null));
				using (dataReader = this.ExecuteReader(command.Type, command.Text, dataParameters.ToArray(), commandBehavior, command.Timeout, command.Prepare))
				{
					while (dataReader.Read())
					{
						plainObject = (IPlainObject)Activator.CreateInstance(targetType);

						MappingUtils.MapObjectFromRecord(dataReader, command, dataOperation, plainObject);

						plainObjects.Add(plainObject);
					}
				}

				if (dataReader.RecordsAffected != ExpectedRecordsAffected)
					throw new InvalidOperationException(string.Format("An unexpected records affected count of '{0}' encountered during query operation. Expected value was '{1}'.", dataReader.RecordsAffected, ExpectedRecordsAffected));

				// per MSDN, output parameters after reader close
				if ((object)criteriaObject != null)
					MappingUtils.MapObjectFromOutputParameters(dataParameters, command, dataOperation, criteriaObject);
				else
					MappingUtils.MapObjectFromOutputParameters(dataParameters, command, dataOperation);

				return plainObjects;
			}
			finally
			{
				// DO NOT DISPOSE OF TRANSACTION

				// CommandBehavior.CloseConnection => DO NOT CLOSE CONNECTION: when caller closes reader, connection will be closed.

				// destroy and tear-down the connection
				if (this.ShouldDisposeResources && (object)dbConnection != null &&
				    ((commandBehavior & CommandBehavior.CloseConnection) != CommandBehavior.CloseConnection))
				{
					dbConnection.Dispose();
					dbConnection = null;
				}
			}
		}

		/// <summary>
		/// Load an object by the specified ID (primary key).
		/// </summary>
		/// <typeparam name="TPlainObject">The type of object to perform operations on.</typeparam>
		/// <param name="identityObject">The identity object to use in restoration.</param>
		/// <returns>An object which is identified by the specified identity object or null if the object could not be found.</returns>
		public static TPlainObject ExecuteRestoreObject<TPlainObject>(this IUnitOfWorkContext unitOfWorkContext, IIdentityObject identityObject) where TPlainObject : class, IPlainObject, new()
		{
			Type targetType;
			object result;
			TPlainObject retval;

			targetType = typeof(TPlainObject);

			result = ExecuteRestoreObject(unitOfWorkContext, targetType, identityObject);

			if ((object)result == null)
				return null;

			retval = result as TPlainObject;

			if ((object)retval == null)
				throw new InvalidOperationException(string.Format("Target type '{0}' is not assignable from type '{1}'.", targetType.FullName, result.GetType().FullName));

			return retval;
		}

		/// <summary>
		/// Load an object by the specified ID (primary key).
		/// </summary>
		/// <param name="targetType">The type of object to perform operations on.
		/// The type specificed must be assignable to IPlainObject.</param>
		/// <param name="identityObject">The identity object to use in restoration.</param>
		/// <returns>A plain object which is identified by the specified identity object or null if the object could not be found.</returns>
		public static IPlainObject ExecuteRestoreObject(this IUnitOfWorkContext unitOfWorkContext, Type targetType, IIdentityObject identityObject)
		{
			CommandBehavior commandBehavior = CommandBehavior.Default;
			IDbConnection dbConnection = null;
			IDbTransaction dbTransaction;
			IPlainObject plainObject;

			DataSourceMap dataSourceMap;
			Command command;
			List<IDataParameter> dataParameters;
			IDataReader dataReader;
			const DataOperation DATA_OPERATION = DataOperation.SelectOne;

			if ((object)identityObject == null)
				throw new ArgumentNullException("identityObject");

			try
			{
				if ((object)targetType == null)
					throw new ArgumentNullException("targetType");

				dataSourceMap = this.dataSourceMapFactory.GetMap(targetType, identityObject);

				if ((object)dataSourceMap == null)
					throw new InvalidOperationException(string.Format("Failed to obtain data source mapping for the type '{0}' from the data source mapping factory.", targetType.FullName));

				command = dataSourceMap.SelectOne;

				AssertCommand(command, DATA_OPERATION, targetType);

				dataParameters = new List<IDataParameter>();

				MappingUtils.MapObjectToInputParameters(dataParameters, command, DATA_OPERATION, identityObject, this);

				// get connection and optional local transaction
				this.GetConnectionAndTransaction(out dbConnection, out dbTransaction);

				commandBehavior = base.ShouldDisposeResources ? CommandBehavior.CloseConnection : CommandBehavior.Default;

				if (DataType.IsNullOrWhiteSpace(command.Text))
					throw new InvalidOperationException(string.Format("An null or zero length command text value was encountered during persist operation."));

				Debug.WriteLine(GetCommandString(command, commandBehavior, DATA_OPERATION, null, null));
				using (dataReader = this.ExecuteReader(command.Type, command.Text, dataParameters.ToArray(), commandBehavior, command.Timeout, command.Prepare))
				{
					if (dataReader.Read())
					{
						plainObject = (IPlainObject)Activator.CreateInstance(targetType);

						MappingUtils.MapObjectFromRecord(dataReader, command, DATA_OPERATION, plainObject);
					}
					else
						plainObject = null;
				}

				if (dataReader.RecordsAffected != ExpectedRecordsAffected)
					throw new InvalidOperationException(string.Format("An unexpected records affected count of '{0}' encountered during restore operation. Expected value was '{1}'.", dataReader.RecordsAffected, ExpectedRecordsAffected));

				// per MSDN, output parameters after reader close
				MappingUtils.MapObjectFromOutputParameters(dataParameters, command, DATA_OPERATION, identityObject);

				// +++++++++++++++++++++++++++++++++++++

				IEnumerable foreignPlainObjects;
				CriteriaObject foreignCriteriaObject;

				foreach (Relationship oneSideOfOneToManyRelationship in dataSourceMap.Relationships.Where(r => r.Kind == "OneSideOfOneToMany"))
				{
					foreignCriteriaObject = new CriteriaObject();
					foreignCriteriaObject.SelectForId = oneSideOfOneToManyRelationship.Foreign;
					//oneSideOfOneToManyRelationship.Property;

					//foreignPlainObjects = this.ExecuteQueryObjects(XXXXXX, foreignCriteriaObject);
				}

				// +++++++++++++++++++++++++++++++++++++

				return plainObject;
			}
			finally
			{
				// DO NOT DISPOSE OF TRANSACTION

				// CommandBehavior.CloseConnection => DO NOT CLOSE CONNECTION: when caller closes reader, connection will be closed.

				// destroy and tear-down the connection
				if (this.ShouldDisposeResources && (object)dbConnection != null &&
				    ((commandBehavior & CommandBehavior.CloseConnection) != CommandBehavior.CloseConnection))
				{
					dbConnection.Dispose();
					dbConnection = null;
				}
			}
		}

		#endregion
	}
}