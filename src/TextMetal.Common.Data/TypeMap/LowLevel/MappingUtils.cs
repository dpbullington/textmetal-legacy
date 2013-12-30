/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data.TypeMap.LowLevel
{
	public static class MappingUtils
	{
		#region Methods/Operators

		public static IEnumerable MapForeignObjectsFromObject(Relationship relationship, DataOperation dataOperation, object targetInstance)
		{
			object propertyValue;
			IEnumerable foreignObjects;

			if ((object)relationship == null)
				throw new ArgumentNullException("relationship");

			if ((object)targetInstance == null)
				throw new ArgumentNullException("targetInstance");

			if (!Reflexion.GetLogicalPropertyValue(targetInstance, relationship.Property, out propertyValue))
				throw new InvalidOperationException("TODO: add meaningful message");

			foreignObjects = propertyValue as IEnumerable;

			if ((object)foreignObjects == null)
				throw new InvalidOperationException("");

			foreach (object foreignObject in foreignObjects)
			{
				foreach (Mapping mapping in relationship.Mappings)
				{
					if (!Reflexion.GetLogicalPropertyValue(targetInstance, mapping.This, out propertyValue))
						throw new InvalidOperationException("TODO: add meaningful message");

					if (!Reflexion.SetLogicalPropertyValue(foreignObject, mapping.That, propertyValue))
						throw new InvalidOperationException("TODO: add meaningful message");
				}
			}

			return foreignObjects;
		}

		public static void MapObjectFromOutputParameters(IList<IDataParameter> dataParameters, Command command, DataOperation dataOperation, object targetInstance)
		{
			Type targetType;
			object propertyValue;
			IDataParameter dataParameter;

			if ((object)dataParameters == null)
				throw new ArgumentNullException("dataParameters");

			if ((object)command == null)
				throw new ArgumentNullException("command");

			if ((object)targetInstance == null)
				throw new ArgumentNullException("targetInstance");

			targetType = targetInstance.GetType();

			foreach (Parameter outputParameter in command.Parameters.Where(p => p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.ReturnValue))
			{
				dataParameter = dataParameters.SingleOrDefault(dp => dp.ParameterName == outputParameter.Name);

				if ((object)dataParameter == null)
					throw new InvalidOperationException(string.Format("The command does not contain the (output) parameter '{2}' for the type '{0}' specified in the data source (output) parameter mapping for the data source command '{1}'.", targetType.FullName, dataOperation, outputParameter.Name));

				propertyValue = dataParameter.Value.ChangeType<object>();

				if (!Reflexion.SetLogicalPropertyValue(targetInstance, outputParameter.Property, propertyValue))
					throw new InvalidOperationException(string.Format("Failed to obtain a writable, public instance property '{2}' for the type '{0}' specified in the data source (output) parameter mapping for the data source command '{1}'.", targetType.FullName, dataOperation, outputParameter.Property));
			}
		}

		public static void MapObjectFromOutputParameters(IList<IDataParameter> dataParameters, Command command, DataOperation dataOperation)
		{
			if ((object)dataParameters == null)
				throw new ArgumentNullException("dataParameters");

			if ((object)command == null)
				throw new ArgumentNullException("command");

			// do nothing intentially
		}

		public static void MapObjectFromRecord(IDataRecord dataRecord, Command command, DataOperation dataOperation, object targetInstance)
		{
			Type targetType;
			object propertyValue;

			if ((object)dataRecord == null)
				throw new ArgumentNullException("dataRecord");

			if ((object)command == null)
				throw new ArgumentNullException("command");

			if ((object)targetInstance == null)
				throw new ArgumentNullException("targetInstance");

			targetType = targetInstance.GetType();

			if (command.Fields.Count > 0)
			{
				// explicit mapping
				foreach (Field field in command.Fields)
				{
					try
					{
						propertyValue = dataRecord[field.Name].ChangeType<object>();
					}
					catch (IndexOutOfRangeException ioorex)
					{
						throw new InvalidOperationException(string.Format("The resultset does not contain the field '{2}' for the type '{0}' specified in the data source field mapping for the data source command '{1}'.", targetType.FullName, dataOperation, field.Name), ioorex);
					}

					if (!Reflexion.SetLogicalPropertyValue(targetInstance, field.Property, propertyValue))
						throw new InvalidOperationException(string.Format("Failed to obtain a writable, public instance property '{2}' for the type '{0}' specified in the data source field mapping for the data source command '{1}'.", targetType.FullName, dataOperation, field.Property));
				}
			}
			else
			{
				// implicit mapping
				for (int i = 0; i < dataRecord.FieldCount; i++)
				{
					string columnName;

					columnName = dataRecord.GetName(i);

					try
					{
						propertyValue = dataRecord[columnName].ChangeType<object>();
					}
					catch (IndexOutOfRangeException ioorex)
					{
						throw new InvalidOperationException(string.Format("The resultset does not contain the field '{2}' for the type '{0}' specified in the data source field mapping for the data source command '{1}'.", targetType.FullName, dataOperation, columnName), ioorex);
					}

					if (!Reflexion.SetLogicalPropertyValue(targetInstance, columnName, propertyValue))
						throw new InvalidOperationException(string.Format("Failed to obtain a writable, public instance property '{2}' for the type '{0}' specified in the data source field mapping for the data source command '{1}'.", targetType.FullName, dataOperation, columnName));
				}
			}
		}

		public static void MapObjectToInputParameters(IList<IDataParameter> dataParameters, Command command, DataOperation dataOperation, object targetInstance, IUnitOfWork unitOfWork)
		{
			Type targetType;
			object parameterValue;
			IDataParameter dataParameter;

			if ((object)dataParameters == null)
				throw new ArgumentNullException("dataParameters");

			if ((object)command == null)
				throw new ArgumentNullException("command");

			if ((object)targetInstance == null)
				throw new ArgumentNullException("targetInstance");

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			targetType = targetInstance.GetType();

			foreach (Parameter inputParameter in command.Parameters.Where(p => p.Direction == ParameterDirection.Input || p.Direction == ParameterDirection.InputOutput))
			{
				if (!Reflexion.GetLogicalPropertyValue(targetInstance, inputParameter.Property, out parameterValue))
					throw new InvalidOperationException(string.Format("Failed to obtain a readable, public instance property '{2}' for the type '{0}' specified in the data source (input) parameter mapping for the data source command '{1}'.", targetType.FullName, dataOperation, inputParameter.Property));

				dataParameter = unitOfWork.CreateParameter(inputParameter.Direction, inputParameter.Type, inputParameter.Size, inputParameter.Precision, inputParameter.Scale, inputParameter.Nullable, inputParameter.Name, parameterValue);

				dataParameters.Add(dataParameter);
			}
		}

		public static void MapObjectToInputParameters(IList<IDataParameter> dataParameters, Command command, DataOperation dataOperation, IUnitOfWork unitOfWork)
		{
			IDataParameter dataParameter;

			if ((object)dataParameters == null)
				throw new ArgumentNullException("dataParameters");

			if ((object)command == null)
				throw new ArgumentNullException("command");

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			foreach (Parameter inputParameter in command.Parameters.Where(p => p.Direction == ParameterDirection.Input || p.Direction == ParameterDirection.InputOutput))
			{
				dataParameter = unitOfWork.CreateParameter(inputParameter.Direction, inputParameter.Type, inputParameter.Size, inputParameter.Precision, inputParameter.Scale, inputParameter.Nullable, inputParameter.Name, null);

				dataParameters.Add(dataParameter);
			}
		}

		public static void MapObjectToOutputParameters(IList<IDataParameter> dataParameters, Command command, DataOperation dataOperation, object targetInstance, IUnitOfWork unitOfWork)
		{
			IDataParameter dataParameter;

			if ((object)dataParameters == null)
				throw new ArgumentNullException("dataParameters");

			if ((object)command == null)
				throw new ArgumentNullException("command");

			if ((object)targetInstance == null)
				throw new ArgumentNullException("targetInstance");

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			foreach (Parameter inputParameter in command.Parameters.Where(p => p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue))
			{
				dataParameter = unitOfWork.CreateParameter(inputParameter.Direction, inputParameter.Type, inputParameter.Size, inputParameter.Precision, inputParameter.Scale, inputParameter.Nullable, inputParameter.Name, null);

				dataParameters.Add(dataParameter);
			}
		}

		public static void MapObjectToOutputParameters(IList<IDataParameter> dataParameters, Command command, DataOperation dataOperation, IUnitOfWork unitOfWork)
		{
			IDataParameter dataParameter;

			if ((object)dataParameters == null)
				throw new ArgumentNullException("dataParameters");

			if ((object)command == null)
				throw new ArgumentNullException("command");

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			foreach (Parameter inputParameter in command.Parameters.Where(p => p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue))
			{
				dataParameter = unitOfWork.CreateParameter(inputParameter.Direction, inputParameter.Type, inputParameter.Size, inputParameter.Precision, inputParameter.Scale, inputParameter.Nullable, inputParameter.Name, null);

				dataParameters.Add(dataParameter);
			}
		}

		#endregion
	}
}