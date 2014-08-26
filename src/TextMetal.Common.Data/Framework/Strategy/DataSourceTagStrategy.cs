/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

using TextMetal.Common.Core;
using TextMetal.Common.Data.Framework.Mapping;

namespace TextMetal.Common.Data.Framework.Strategy
{
	public abstract class DataSourceTagStrategy : IDataSourceTagStrategy, ISqlNuance
	{
		#region Constructors/Destructors

		protected DataSourceTagStrategy(string dataSourceTag, bool canCreateNativeDatabaseFile)
		{
			this.dataSourceTag = dataSourceTag;
			this.canCreateNativeDatabaseFile = canCreateNativeDatabaseFile;
		}

		#endregion

		#region Fields/Constants

		private readonly bool canCreateNativeDatabaseFile;
		private readonly string dataSourceTag;

		#endregion

		#region Properties/Indexers/Events

		public bool CanCreateNativeDatabaseFile
		{
			get
			{
				return this.canCreateNativeDatabaseFile;
			}
		}

		public string DataSourceTag
		{
			get
			{
				return this.dataSourceTag;
			}
		}

		#endregion

		#region Methods/Operators

		private static void AssertValidMapping(Type modelType, TableMappingAttribute tableMappingAttribute)
		{
			if ((object)modelType == null)
				throw new ArgumentNullException("modelType");

			if ((object)tableMappingAttribute == null)
				throw new InvalidOperationException(string.Format("The model type '{0}' does not specify the '{1}' attribute.", modelType.FullName, typeof(TableMappingAttribute).FullName));

			if ((object)tableMappingAttribute._ColumnMappingAttributes == null ||
				tableMappingAttribute._ColumnMappingAttributes.Count == 0)
				throw new InvalidOperationException(string.Format("The model type '{0}' does not specify the '{1}' attribute on any public, instance, read-write property.", modelType.FullName, typeof(ColumnMappingAttribute).FullName));
		}

		private static void AssertValidMapping(Type requestModelType, Type resultModelType, Type responseModelType, ProcedureMappingAttribute procedureMappingAttribute)
		{
			if ((object)requestModelType == null)
				throw new ArgumentNullException("requestModelType");

			if ((object)resultModelType == null)
				throw new ArgumentNullException("resultModelType");

			if ((object)responseModelType == null)
				throw new ArgumentNullException("responseModelType");

			if ((object)procedureMappingAttribute == null)
				throw new InvalidOperationException(string.Format("The request model type '{0}' does not specify the '{1}' attribute.", requestModelType.FullName, typeof(ProcedureMappingAttribute).FullName));

			if ((object)procedureMappingAttribute._RequestParameterMappingAttributes == null ||
				procedureMappingAttribute._RequestParameterMappingAttributes.Count == 0)
				throw new InvalidOperationException(string.Format("The request model type '{0}' does not specify the '{1}' attribute on any public, instance, read-write property.", requestModelType.FullName, typeof(ParameterMappingAttribute).FullName));

			if ((object)procedureMappingAttribute._ResponseParameterMappingAttributes == null ||
				procedureMappingAttribute._ResponseParameterMappingAttributes.Count == 0)
				throw new InvalidOperationException(string.Format("The result model type '{0}' does not specify the '{1}' attribute on any public, instance, read-write property.", resultModelType.FullName, typeof(ColumnMappingAttribute).FullName));

			if ((object)procedureMappingAttribute._ResponseParameterMappingAttributes == null ||
				procedureMappingAttribute._ResponseParameterMappingAttributes.Count == 0)
				throw new InvalidOperationException(string.Format("The response model type '{0}' does not specify the '{1}' attribute on any public, instance, read-write property.", resultModelType.FullName, typeof(ParameterMappingAttribute).FullName));
		}

		private static Action<TModel, IDictionary<string, object>> GetMapToMethod<TModel>(TableMappingAttribute tableMappingAttribute)
			where TModel : class, IModelObject
		{
			Action<TModel, IDictionary<string, object>> callback;

			if ((object)tableMappingAttribute == null)
				throw new ArgumentNullException("tableMappingAttribute");

			callback = (md, ts) =>
						{
							ColumnMappingAttribute[] columnMappingAttributes;

							if ((object)md == null)
								throw new ArgumentNullException("md");

							if ((object)ts == null)
								throw new ArgumentNullException("ts");

							columnMappingAttributes = tableMappingAttribute._ColumnMappingAttributes.OrderBy(cma => cma.ColumnOrdinal).ToArray();
							for (int index = 0; index < columnMappingAttributes.Length; index++)
							{
								object columnValue, propertyValue;

								if (ts.TryGetValue(columnMappingAttributes[index].ColumnName, out columnValue))
								{
									propertyValue = columnValue.ChangeType(columnMappingAttributes[index]._TargetProperty.PropertyType);

									if (!Reflexion.SetLogicalPropertyValue(md, columnMappingAttributes[index]._TargetProperty.Name, propertyValue))
										throw new InvalidOperationException(string.Format("Ah snap."));
								}
							}
						};

			return callback;
		}

		private static Action<TResultModel, IDictionary<string, object>> GetMapToMethod<TResultModel>(ProcedureMappingAttribute procedureMappingAttribute)
			where TResultModel : class, IResultModelObject
		{
			Action<TResultModel, IDictionary<string, object>> callback;

			if ((object)procedureMappingAttribute == null)
				throw new ArgumentNullException("procedureMappingAttribute");

			callback = (md, ts) =>
						{
							ColumnMappingAttribute[] columnMappingAttributes;

							if ((object)md == null)
								throw new ArgumentNullException("md");

							if ((object)ts == null)
								throw new ArgumentNullException("ts");

							columnMappingAttributes = procedureMappingAttribute._ResultColumnMappingAttributes.OrderBy(cma => cma.ColumnOrdinal).ToArray();
							for (int index = 0; index < columnMappingAttributes.Length; index++)
							{
								object columnValue, propertyValue;

								if (ts.TryGetValue(columnMappingAttributes[index].ColumnName, out columnValue))
								{
									propertyValue = columnValue.ChangeType(columnMappingAttributes[index]._TargetProperty.PropertyType);

									if (!Reflexion.SetLogicalPropertyValue(md, columnMappingAttributes[index]._TargetProperty.Name, propertyValue))
										throw new InvalidOperationException(string.Format("Ah snap."));
								}
							}
						};

			return callback;
		}

		private static Action<TResponseModel, IDictionary<string, object>> GetMapToMethod<TResultModel, TResponseModel>(ProcedureMappingAttribute procedureMappingAttribute)
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>
		{
			Action<TResponseModel, IDictionary<string, object>> callback;

			if ((object)procedureMappingAttribute == null)
				throw new ArgumentNullException("procedureMappingAttribute");

			callback = (md, ts) =>
						{
							ParameterMappingAttribute[] parameterMappingAttributes;

							if ((object)md == null)
								throw new ArgumentNullException("md");

							if ((object)ts == null)
								throw new ArgumentNullException("ts");

							parameterMappingAttributes = procedureMappingAttribute._ResponseParameterMappingAttributes.OrderBy(pma => pma.ParameterOrdinal).ToArray();
							for (int index = 0; index < parameterMappingAttributes.Length; index++)
							{
								object parameterValue, propertyValue;

								if (ts.TryGetValue(parameterMappingAttributes[index].ParameterName, out parameterValue))
								{
									propertyValue = parameterValue.ChangeType(parameterMappingAttributes[index]._TargetProperty.PropertyType);

									if (!Reflexion.SetLogicalPropertyValue(md, parameterMappingAttributes[index]._TargetProperty.Name, propertyValue))
										throw new InvalidOperationException(string.Format("Ah snap."));
								}
							}
						};

			return callback;
		}

		public bool CreateNativeDatabaseFile(string databaseFilePath)
		{
			// do nothing
			return false;
		}

		public abstract string GetAliasedColumnName(string tableAlias, string columnName);

		public abstract string GetColumnName(string columnName);

		private TacticCommand<TModel> GetDeleteAllTacticCommand<TModel>(IUnitOfWork unitOfWork, IModelObject model, IModelQuery modelQuery, TableMappingAttribute tableMappingAttribute)
			where TModel : class, IModelObject
		{
			throw new NotImplementedException();
		}

		private TacticCommand<TModel> GetDeleteOneTacticCommand<TModel>(IUnitOfWork unitOfWork, TModel prototype, TableMappingAttribute tableMappingAttribute)
			where TModel : class, IModelObject
		{
			TacticCommand<TModel> tacticCommand;
			const bool IS_NULLIPOTENT = false;
			const bool COMMAND_PREPARE = false;
			const object COMMAND_TIMEOUT = null;
			const CommandType COMMAND_TYPE = CommandType.Text;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			int expectedRecordsAffected;
			Action<TModel, IDictionary<string, object>> tableToModelMappingCallback;
			string commandText;
			IDataParameter commandParameter;
			IDictionary<string, IDataParameter> commandParameters;
			ColumnMappingAttribute[] columnMappingAttributes;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)prototype == null)
				throw new ArgumentNullException("prototype");

			if ((object)tableMappingAttribute == null)
				throw new ArgumentNullException("tableMappingAttribute");

			commandText = @"DELETE FROM ";

			commandText += this.GetTableName(tableMappingAttribute.SchemaName, tableMappingAttribute.TableName);

			commandText += @" WHERE ";
			commandParameters = new Dictionary<string, IDataParameter>();

			columnMappingAttributes = tableMappingAttribute._ColumnMappingAttributes.Where(cma => !tableMappingAttribute._ColumnMappingAttributes.Any() || cma.ColumnIsPrimaryKey).OrderBy(cma => cma.ColumnOrdinal).ToArray();
			for (int index = 0; index < columnMappingAttributes.Length; index++)
			{
				string parameterName;
				object parameterValue;

				if (!Reflexion.GetLogicalPropertyValue(prototype, columnMappingAttributes[index]._TargetProperty.Name, out parameterValue))
					throw new InvalidOperationException(string.Format("Ah snap."));

				parameterName = this.GetParameterName(columnMappingAttributes[index].ColumnName);
				commandParameter = unitOfWork.CreateParameter(ParameterDirection.Input, columnMappingAttributes[index].ColumnDbType, columnMappingAttributes[index].ColumnSize, columnMappingAttributes[index].ColumnPrecision, columnMappingAttributes[index].ColumnScale, columnMappingAttributes[index].ColumnNullable, parameterName, parameterValue);
				this.ParameterMagic(unitOfWork, commandParameter, columnMappingAttributes[index].ColumnSqlType);
				commandParameters.Add(parameterName, commandParameter);

				commandText += this.GetColumnName(columnMappingAttributes[index].ColumnName) + @" = " + parameterName;

				if (index != (columnMappingAttributes.Length - 1))
					commandText += @" AND ";
			}

			commandText += @";";

			expectedRecordsAffected = this.GetExpectedRecordsAffected(IS_NULLIPOTENT);
			tableToModelMappingCallback = GetMapToMethod<TModel>(tableMappingAttribute);

			tacticCommand = new TacticCommand<TModel>()
							{
								CommandBehavior = COMMAND_BEHAVIOR,
								CommandParameters = commandParameters.Values,
								CommandPrepare = COMMAND_PREPARE,
								CommandText = commandText,
								CommandTimeout = (int?)COMMAND_TIMEOUT,
								CommandType = COMMAND_TYPE,
								ExpectedRecordsAffected = expectedRecordsAffected,
								IsNullipotent = IS_NULLIPOTENT,
								TableToModelMappingCallback = tableToModelMappingCallback
							};

			return tacticCommand;
		}

		public TacticCommand<TModel> GetDeleteTacticCommand<TModel>(IUnitOfWork unitOfWork, TModel modelValue, IModelQuery modelQuery) where TModel : class, IModelObject
		{
			TacticCommand<TModel> tacticCommand;
			Type modelType;
			TableMappingAttribute tableMappingAttribute;
			bool isDeleteOne;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)modelValue == null)
				throw new ArgumentNullException("modelValue");

			modelType = typeof(TModel);
			tableMappingAttribute = this.GetTableMapping(modelType);

			AssertValidMapping(modelType, tableMappingAttribute);

			isDeleteOne = (object)modelQuery == null;

			if (isDeleteOne)
				tacticCommand = this.GetDeleteOneTacticCommand<TModel>(unitOfWork, modelValue, tableMappingAttribute);
			else
				tacticCommand = this.GetDeleteAllTacticCommand<TModel>(unitOfWork, modelValue, modelQuery, tableMappingAttribute);

			return tacticCommand;
		}

		public TacticCommand<TRequestModel, TResultModel, TResponseModel> GetExecuteOneTacticCommand<TRequestModel, TResultModel, TResponseModel>(IUnitOfWork unitOfWork, TRequestModel requestModelValue, ProcedureMappingAttribute procedureMappingAttribute)
			where TRequestModel : class, IRequestModelObject
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>
		{
			TacticCommand<TRequestModel, TResultModel, TResponseModel> tacticCommand;
			const bool IS_NULLIPOTENT = true;
			const bool COMMAND_PREPARE = false;
			const object COMMAND_TIMEOUT = null;
			const CommandType COMMAND_TYPE = CommandType.Text;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			int expectedRecordsAffected;
			Action<TResultModel, IDictionary<string, object>> tableToResultModelMappingCallback;
			Action<TResponseModel, IDictionary<string, object>> tableToResponseModelMappingCallback;
			string commandText;
			IDictionary<string, IDataParameter> commandParameters;
			IDataParameter commandParameter;
			ParameterMappingAttribute[] parameterMappingAttributes;
			ColumnMappingAttribute[] columnMappingAttributes;
			ModelQuery _modelQuery;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)procedureMappingAttribute == null)
				throw new ArgumentNullException("procedureMappingAttribute");

			commandText = this.GetProcedureName(procedureMappingAttribute.SchemaName, procedureMappingAttribute.ProcedureName);
			commandParameters = new Dictionary<string, IDataParameter>();

			var temp = new List<ParameterMappingAttribute>();
			temp.AddRange(procedureMappingAttribute._RequestParameterMappingAttributes);
			temp.AddRange(procedureMappingAttribute._ResponseParameterMappingAttributes);

			parameterMappingAttributes = temp.OrderBy(pma => pma.ParameterOrdinal).ToArray();
			for (int index = 0; index < parameterMappingAttributes.Length; index++)
			{
				string parameterName;
				object parameterValue;

				if (!Reflexion.GetLogicalPropertyValue(requestModelValue, parameterMappingAttributes[index]._TargetProperty.Name, out parameterValue))
					throw new InvalidOperationException(string.Format("Ah snap."));

				parameterName = this.GetParameterName(parameterMappingAttributes[index].ParameterName);
				commandParameter = unitOfWork.CreateParameter(parameterMappingAttributes[index].ParameterDirection, parameterMappingAttributes[index].ParameterDbType, parameterMappingAttributes[index].ParameterSize, parameterMappingAttributes[index].ParameterPrecision, parameterMappingAttributes[index].ParameterScale, parameterMappingAttributes[index].ParameterNullable, parameterName, parameterValue);
				this.ParameterMagic(unitOfWork, commandParameter, parameterMappingAttributes[index].ParameterSqlType);
				commandParameters.Add(parameterName, commandParameter);
			}

			expectedRecordsAffected = this.GetExpectedRecordsAffected(IS_NULLIPOTENT);
			tableToResultModelMappingCallback = GetMapToMethod<TResultModel>(procedureMappingAttribute);
			tableToResponseModelMappingCallback = GetMapToMethod<TResultModel, TResponseModel>(procedureMappingAttribute);

			tacticCommand = new TacticCommand<TRequestModel, TResultModel, TResponseModel>()
							{
								CommandBehavior = COMMAND_BEHAVIOR,
								CommandParameters = commandParameters.Values,
								CommandPrepare = COMMAND_PREPARE,
								CommandText = commandText,
								CommandTimeout = (int?)COMMAND_TIMEOUT,
								CommandType = COMMAND_TYPE,
								ExpectedRecordsAffected = expectedRecordsAffected,
								IsNullipotent = IS_NULLIPOTENT,
								TableToResultModelMappingCallback = tableToResultModelMappingCallback,
								TableToResponseModelMappingCallback = tableToResponseModelMappingCallback
							};

			return tacticCommand;
		}

		public TacticCommand<TRequestModel, TResultModel, TResponseModel> GetExecuteTacticCommand<TRequestModel, TResultModel, TResponseModel>(IUnitOfWork unitOfWork, TRequestModel requestModelValue)
			where TRequestModel : class, IRequestModelObject
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>
		{
			TacticCommand<TRequestModel, TResultModel, TResponseModel> tacticCommand;
			Type requestModelType;
			Type resultModelType;
			Type responseModelType;
			ProcedureMappingAttribute procedureMappingAttribute;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)requestModelValue == null)
				throw new ArgumentNullException("requestModelValue");

			requestModelType = typeof(TRequestModel);
			resultModelType = typeof(TResultModel);
			responseModelType = typeof(TResponseModel);
			procedureMappingAttribute = this.GetProcedureMapping(requestModelType, resultModelType, responseModelType);

			AssertValidMapping(requestModelType, resultModelType, responseModelType, procedureMappingAttribute);

			tacticCommand = this.GetExecuteOneTacticCommand<TRequestModel, TResultModel, TResponseModel>(unitOfWork, requestModelValue, procedureMappingAttribute);

			return tacticCommand;
		}

		public abstract int GetExpectedRecordsAffected(bool isNullipotent);

		public abstract string GetIdentityCommand();

		public TacticCommand<TModel> GetIdentityTacticCommand<TModel>(IUnitOfWork unitOfWork) where TModel : class, IModelObject
		{
			TacticCommand<TModel> tacticCommand;
			Type modelType;
			TableMappingAttribute tableMappingAttribute;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			modelType = typeof(TModel);
			tableMappingAttribute = this.GetTableMapping(modelType);

			AssertValidMapping(modelType, tableMappingAttribute);

			tacticCommand = this.GetIdentityTacticCommand<TModel>(unitOfWork, tableMappingAttribute);

			return tacticCommand;
		}

		private TacticCommand<TModel> GetIdentityTacticCommand<TModel>(IUnitOfWork unitOfWork, TableMappingAttribute tableMappingAttribute)
			where TModel : class, IModelObject
		{
			TacticCommand<TModel> tacticCommand;
			const bool IS_NULLIPOTENT = true;
			const bool COMMAND_PREPARE = false;
			const object COMMAND_TIMEOUT = null;
			const CommandType COMMAND_TYPE = CommandType.Text;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			int expectedRecordsAffected;
			Action<TModel, IDictionary<string, object>> tableToModelMappingCallback;
			string commandText;
			IDictionary<string, IDataParameter> commandParameters;
			ColumnMappingAttribute columnMappingAttribute;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableMappingAttribute == null)
				throw new ArgumentNullException("tableMappingAttribute");

			commandParameters = new Dictionary<string, IDataParameter>();
			columnMappingAttribute = tableMappingAttribute._ColumnMappingAttributes.Where(cma => cma.IsColumnServerGeneratedPrimaryKey).SingleOrDefault();

			if ((object)columnMappingAttribute == null)
				return null;

			commandText = @"SELECT " + this.GetIdentityCommand() + @" AS " + this.GetColumnName(columnMappingAttribute.ColumnName) + @";";

			expectedRecordsAffected = this.GetExpectedRecordsAffected(IS_NULLIPOTENT);
			tableToModelMappingCallback = GetMapToMethod<TModel>(tableMappingAttribute);

			tacticCommand = new TacticCommand<TModel>()
							{
								CommandBehavior = COMMAND_BEHAVIOR,
								CommandParameters = commandParameters.Values,
								CommandPrepare = COMMAND_PREPARE,
								CommandText = commandText,
								CommandTimeout = (int?)COMMAND_TIMEOUT,
								CommandType = COMMAND_TYPE,
								ExpectedRecordsAffected = expectedRecordsAffected,
								IsNullipotent = IS_NULLIPOTENT,
								TableToModelMappingCallback = tableToModelMappingCallback
							};

			return tacticCommand;
		}

		private TacticCommand<TModel> GetInsertAllTacticCommand<TModel>(IUnitOfWork unitOfWork, IModelObject model, IModelQuery modelQuery, TableMappingAttribute tableMappingAttribute)
			where TModel : class, IModelObject
		{
			throw new NotImplementedException();
		}

		private TacticCommand<TModel> GetInsertOneTacticCommand<TModel>(IUnitOfWork unitOfWork, TModel model, TableMappingAttribute tableMappingAttribute) where TModel : class, IModelObject
		{
			TacticCommand<TModel> tacticCommand;
			const bool IS_NULLIPOTENT = false;
			const bool COMMAND_PREPARE = false;
			const object COMMAND_TIMEOUT = null;
			const CommandType COMMAND_TYPE = CommandType.Text;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			int expectedRecordsAffected;
			Action<TModel, IDictionary<string, object>> tableToModelMappingCallback;
			string commandText;
			IDataParameter commandParameter;
			IDictionary<string, IDataParameter> commandParameters;
			ColumnMappingAttribute[] columnMappingAttributes;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			if ((object)tableMappingAttribute == null)
				throw new ArgumentNullException("tableMappingAttribute");

			commandText = @"INSERT INTO ";

			commandText += this.GetTableName(tableMappingAttribute.SchemaName, tableMappingAttribute.TableName) + @" (";

			columnMappingAttributes = tableMappingAttribute._ColumnMappingAttributes.Where(cma => !cma.IsColumnServerGeneratedPrimaryKey).OrderBy(cma => cma.ColumnOrdinal).ToArray();
			for (int index = 0; index < columnMappingAttributes.Length; index++)
			{
				commandText += this.GetColumnName(columnMappingAttributes[index].ColumnName);

				if (index != (columnMappingAttributes.Length - 1))
					commandText += @", ";
			}

			commandText += @") VALUES (";
			commandParameters = new Dictionary<string, IDataParameter>();

			// yes, this is redundant loop but it makes it easier to maintain for now
			for (int index = 0; index < columnMappingAttributes.Length; index++)
			{
				string parameterName;
				object parameterValue;

				if (!Reflexion.GetLogicalPropertyValue(model, columnMappingAttributes[index]._TargetProperty.Name, out parameterValue))
					throw new InvalidOperationException(string.Format("Ah snap."));

				parameterName = this.GetParameterName(columnMappingAttributes[index].ColumnName);
				commandParameter = unitOfWork.CreateParameter(ParameterDirection.Input, columnMappingAttributes[index].ColumnDbType, columnMappingAttributes[index].ColumnSize, columnMappingAttributes[index].ColumnPrecision, columnMappingAttributes[index].ColumnScale, columnMappingAttributes[index].ColumnNullable, parameterName, parameterValue);
				this.ParameterMagic(unitOfWork, commandParameter, columnMappingAttributes[index].ColumnSqlType);
				commandParameters.Add(parameterName, commandParameter);

				commandText += parameterName;

				if (index != (columnMappingAttributes.Length - 1))
					commandText += @", ";
			}

			commandText += @");";

			expectedRecordsAffected = this.GetExpectedRecordsAffected(IS_NULLIPOTENT);
			tableToModelMappingCallback = GetMapToMethod<TModel>(tableMappingAttribute);

			tacticCommand = new TacticCommand<TModel>()
							{
								CommandBehavior = COMMAND_BEHAVIOR,
								CommandParameters = commandParameters.Values,
								CommandPrepare = COMMAND_PREPARE,
								CommandText = commandText,
								CommandTimeout = (int?)COMMAND_TIMEOUT,
								CommandType = COMMAND_TYPE,
								ExpectedRecordsAffected = expectedRecordsAffected,
								IsNullipotent = IS_NULLIPOTENT,
								TableToModelMappingCallback = tableToModelMappingCallback
							};

			return tacticCommand;
		}

		public TacticCommand<TModel> GetInsertTacticCommand<TModel>(IUnitOfWork unitOfWork, TModel modelValue, IModelQuery modelQuery) where TModel : class, IModelObject
		{
			TacticCommand<TModel> tacticCommand;
			Type modelType;
			TableMappingAttribute tableMappingAttribute;
			bool isInsertOne;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)modelValue == null)
				throw new ArgumentNullException("modelValue");

			modelType = typeof(TModel);
			tableMappingAttribute = this.GetTableMapping(modelType);

			AssertValidMapping(modelType, tableMappingAttribute);

			isInsertOne = (object)modelQuery == null;

			if (isInsertOne)
				tacticCommand = this.GetInsertOneTacticCommand<TModel>(unitOfWork, modelValue, tableMappingAttribute);
			else
				tacticCommand = this.GetInsertAllTacticCommand<TModel>(unitOfWork, modelValue, modelQuery, tableMappingAttribute);

			return tacticCommand;
		}

		public abstract string GetParameterName(string parameterName);

		protected virtual ProcedureMappingAttribute GetProcedureMapping(Type requestModelType, Type resultModelType, Type responseModelType)
		{
			ProcedureMappingAttribute procedureMappingAttribute;
			ParameterMappingAttribute parameterMappingAttribute;
			ColumnMappingAttribute columnMappingAttribute;
			PropertyInfo[] propertyInfos;

			if ((object)requestModelType == null)
				throw new ArgumentNullException("requestModelType");

			if ((object)resultModelType == null)
				throw new ArgumentNullException("resultModelType");

			if ((object)responseModelType == null)
				throw new ArgumentNullException("responseModelType");

			procedureMappingAttribute = Reflexion.GetOneAttribute<ProcedureMappingAttribute>(requestModelType);

			if ((object)procedureMappingAttribute == null)
				return null;

			procedureMappingAttribute._TargetType = requestModelType;

			propertyInfos = requestModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			if ((object)propertyInfos != null)
			{
				foreach (PropertyInfo propertyInfo in propertyInfos)
				{
					if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
						continue;

					parameterMappingAttribute = Reflexion.GetOneAttribute<ParameterMappingAttribute>(propertyInfo);

					if ((object)parameterMappingAttribute == null)
						continue;

					if (parameterMappingAttribute.ParameterDirection != ParameterDirection.Input &&
						parameterMappingAttribute.ParameterDirection != ParameterDirection.InputOutput)
						continue;

					parameterMappingAttribute._TargetProperty = propertyInfo;

					procedureMappingAttribute._RequestParameterMappingAttributes.Add(parameterMappingAttribute);
				}
			}

			propertyInfos = resultModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			if ((object)propertyInfos != null)
			{
				foreach (PropertyInfo propertyInfo in propertyInfos)
				{
					if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
						continue;

					columnMappingAttribute = Reflexion.GetOneAttribute<ColumnMappingAttribute>(propertyInfo);

					if ((object)columnMappingAttribute == null)
						continue;

					columnMappingAttribute._TargetProperty = propertyInfo;

					procedureMappingAttribute._ResultColumnMappingAttributes.Add(columnMappingAttribute);
				}
			}

			propertyInfos = responseModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			if ((object)propertyInfos != null)
			{
				foreach (PropertyInfo propertyInfo in propertyInfos)
				{
					if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
						continue;

					parameterMappingAttribute = Reflexion.GetOneAttribute<ParameterMappingAttribute>(propertyInfo);

					if ((object)parameterMappingAttribute == null)
						continue;

					if (parameterMappingAttribute.ParameterDirection != ParameterDirection.InputOutput &&
						parameterMappingAttribute.ParameterDirection != ParameterDirection.Output &&
						parameterMappingAttribute.ParameterDirection != ParameterDirection.ReturnValue)
						continue;

					parameterMappingAttribute._TargetProperty = propertyInfo;

					procedureMappingAttribute._RequestParameterMappingAttributes.Add(parameterMappingAttribute);
				}
			}

			return procedureMappingAttribute;
		}

		public abstract string GetProcedureName(string schemaName, string procedureName);

		private TacticCommand<TModel> GetSelectAllTacticCommand<TModel>(IUnitOfWork unitOfWork, IModelObject dummy, IModelQuery modelQuery, TableMappingAttribute tableMappingAttribute)
			where TModel : class, IModelObject
		{
			TacticCommand<TModel> tacticCommand;
			const bool IS_NULLIPOTENT = true;
			const bool COMMAND_PREPARE = false;
			const object COMMAND_TIMEOUT = null;
			const CommandType COMMAND_TYPE = CommandType.Text;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;
			const string TABLE_ALIAS = "t0";

			int expectedRecordsAffected;
			Action<TModel, IDictionary<string, object>> tableToModelMappingCallback;
			string commandText;
			IDictionary<string, IDataParameter> commandParameters;
			ColumnMappingAttribute[] columnMappingAttributes;
			ModelQuery _modelQuery;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)dummy == null)
				throw new ArgumentNullException("dummy");

			if ((object)tableMappingAttribute == null)
				throw new ArgumentNullException("tableMappingAttribute");

			commandText = @"SELECT ";

			columnMappingAttributes = tableMappingAttribute._ColumnMappingAttributes.OrderBy(cma => cma.ColumnOrdinal).ToArray();
			for (int index = 0; index < columnMappingAttributes.Length; index++)
			{
				commandText += this.GetAliasedColumnName(TABLE_ALIAS, columnMappingAttributes[index].ColumnName) + @" AS " + this.GetColumnName(columnMappingAttributes[index].ColumnName);

				if (index != (columnMappingAttributes.Length - 1))
					commandText += @", ";
			}

			commandText += @" FROM ";

			commandText += this.GetTableName(tableMappingAttribute.SchemaName, tableMappingAttribute.TableName) + @" " + this.GetTableAlias(TABLE_ALIAS);

			commandText += @" WHERE {0} ORDER BY {1} "; // late replacement
			commandParameters = new Dictionary<string, IDataParameter>();

			_modelQuery = modelQuery.GetUnderlyingQuery() as ModelQuery;

			if ((object)_modelQuery == null)
				throw new InvalidOperationException(string.Format("Ah snap."));

			var filterFragment = SqlExpressionVisitor.GetFilterText(this, unitOfWork, commandParameters, _modelQuery.FilterExpression);
			var sortFragment = SqlExpressionVisitor.GetSortText(this, unitOfWork, commandParameters, _modelQuery.SortSequences);

			commandText += @";";

			commandText = string.Format(commandText, filterFragment, sortFragment);

			expectedRecordsAffected = this.GetExpectedRecordsAffected(IS_NULLIPOTENT);
			tableToModelMappingCallback = GetMapToMethod<TModel>(tableMappingAttribute);

			tacticCommand = new TacticCommand<TModel>()
							{
								CommandBehavior = COMMAND_BEHAVIOR,
								CommandParameters = commandParameters.Values,
								CommandPrepare = COMMAND_PREPARE,
								CommandText = commandText,
								CommandTimeout = (int?)COMMAND_TIMEOUT,
								CommandType = COMMAND_TYPE,
								ExpectedRecordsAffected = expectedRecordsAffected,
								IsNullipotent = IS_NULLIPOTENT,
								TableToModelMappingCallback = tableToModelMappingCallback
							};

			return tacticCommand;
		}

		private TacticCommand<TModel> GetSelectOneTacticCommand<TModel>(IUnitOfWork unitOfWork, TModel prototype, TableMappingAttribute tableMappingAttribute)
			where TModel : class, IModelObject
		{
			TacticCommand<TModel> tacticCommand;
			const bool IS_NULLIPOTENT = true;
			const bool COMMAND_PREPARE = false;
			const object COMMAND_TIMEOUT = null;
			const CommandType COMMAND_TYPE = CommandType.Text;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;
			const string TABLE_ALIAS = "t0";

			int expectedRecordsAffected;
			Action<TModel, IDictionary<string, object>> tableToModelMappingCallback;
			string commandText;
			IDataParameter commandParameter;
			IDictionary<string, IDataParameter> commandParameters;
			ColumnMappingAttribute[] columnMappingAttributes;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)prototype == null)
				throw new ArgumentNullException("prototype");

			if ((object)tableMappingAttribute == null)
				throw new ArgumentNullException("tableMappingAttribute");

			commandText = @"SELECT ";

			columnMappingAttributes = tableMappingAttribute._ColumnMappingAttributes.OrderBy(cma => cma.ColumnOrdinal).ToArray();
			for (int index = 0; index < columnMappingAttributes.Length; index++)
			{
				commandText += this.GetAliasedColumnName(TABLE_ALIAS, columnMappingAttributes[index].ColumnName) + @" AS " + this.GetColumnName(columnMappingAttributes[index].ColumnName);

				if (index != (columnMappingAttributes.Length - 1))
					commandText += @", ";
			}

			commandText += @" FROM ";

			commandText += this.GetTableName(tableMappingAttribute.SchemaName, tableMappingAttribute.TableName) + @" " + this.GetTableAlias(TABLE_ALIAS);

			commandText += @" WHERE ";
			commandParameters = new Dictionary<string, IDataParameter>();

			columnMappingAttributes = tableMappingAttribute._ColumnMappingAttributes.Where(cma => !tableMappingAttribute._ColumnMappingAttributes.Any() || cma.ColumnIsPrimaryKey).OrderBy(cma => cma.ColumnOrdinal).ToArray();
			for (int index = 0; index < columnMappingAttributes.Length; index++)
			{
				string parameterName;
				object parameterValue;

				if (!Reflexion.GetLogicalPropertyValue(prototype, columnMappingAttributes[index]._TargetProperty.Name, out parameterValue))
					throw new InvalidOperationException(string.Format("Ah snap."));

				parameterName = this.GetParameterName(columnMappingAttributes[index].ColumnName);
				commandParameter = unitOfWork.CreateParameter(ParameterDirection.Input, columnMappingAttributes[index].ColumnDbType, columnMappingAttributes[index].ColumnSize, columnMappingAttributes[index].ColumnPrecision, columnMappingAttributes[index].ColumnScale, columnMappingAttributes[index].ColumnNullable, parameterName, parameterValue);
				this.ParameterMagic(unitOfWork, commandParameter, columnMappingAttributes[index].ColumnSqlType);
				commandParameters.Add(parameterName, commandParameter);

				commandText += this.GetAliasedColumnName(TABLE_ALIAS, columnMappingAttributes[index].ColumnName) + @" = " + parameterName;

				if (index != (columnMappingAttributes.Length - 1))
					commandText += @" AND ";
			}

			commandText += @" ORDER BY ";

			// yes, this is redundant loop but it makes it easier to maintain for now
			for (int index = 0; index < columnMappingAttributes.Length; index++)
			{
				commandText += this.GetAliasedColumnName(TABLE_ALIAS, columnMappingAttributes[index].ColumnName) + @" ASC";

				if (index != (columnMappingAttributes.Length - 1))
					commandText += @", ";
			}

			commandText += @";";

			expectedRecordsAffected = this.GetExpectedRecordsAffected(IS_NULLIPOTENT);
			tableToModelMappingCallback = GetMapToMethod<TModel>(tableMappingAttribute);

			tacticCommand = new TacticCommand<TModel>()
							{
								CommandBehavior = COMMAND_BEHAVIOR,
								CommandParameters = commandParameters.Values,
								CommandPrepare = COMMAND_PREPARE,
								CommandText = commandText,
								CommandTimeout = (int?)COMMAND_TIMEOUT,
								CommandType = COMMAND_TYPE,
								ExpectedRecordsAffected = expectedRecordsAffected,
								IsNullipotent = IS_NULLIPOTENT,
								TableToModelMappingCallback = tableToModelMappingCallback
							};

			return tacticCommand;
		}

		public TacticCommand<TModel> GetSelectTacticCommand<TModel>(IUnitOfWork unitOfWork, TModel modelValue, IModelQuery modelQuery) where TModel : class, IModelObject
		{
			TacticCommand<TModel> tacticCommand;
			Type modelType;
			TableMappingAttribute tableMappingAttribute;
			bool isSelectOne;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)modelValue == null)
				throw new ArgumentNullException("modelValue");

			modelType = typeof(TModel);
			tableMappingAttribute = this.GetTableMapping(modelType);

			AssertValidMapping(modelType, tableMappingAttribute);

			isSelectOne = (object)modelQuery == null;

			if (isSelectOne)
				tacticCommand = this.GetSelectOneTacticCommand<TModel>(unitOfWork, modelValue, tableMappingAttribute);
			else
				tacticCommand = this.GetSelectAllTacticCommand<TModel>(unitOfWork, modelValue, modelQuery, tableMappingAttribute);

			return tacticCommand;
		}

		public abstract string GetTableAlias(string tableAlias);

		protected virtual TableMappingAttribute GetTableMapping(Type targetType)
		{
			TableMappingAttribute tableMappingAttribute;
			ColumnMappingAttribute columnMappingAttribute;
			PropertyInfo[] propertyInfos;

			if ((object)targetType == null)
				throw new ArgumentNullException("targetType");

			tableMappingAttribute = Reflexion.GetOneAttribute<TableMappingAttribute>(targetType);

			if ((object)tableMappingAttribute == null)
				return null;

			tableMappingAttribute._TargetType = targetType;

			propertyInfos = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			if ((object)propertyInfos != null)
			{
				foreach (PropertyInfo propertyInfo in propertyInfos)
				{
					if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
						continue;

					columnMappingAttribute = Reflexion.GetOneAttribute<ColumnMappingAttribute>(propertyInfo);

					if ((object)columnMappingAttribute == null)
						continue;

					columnMappingAttribute._TargetProperty = propertyInfo;

					tableMappingAttribute._ColumnMappingAttributes.Add(columnMappingAttribute);
				}
			}

			return tableMappingAttribute;
		}

		public abstract string GetTableName(string schemaName, string tableName);

		private TacticCommand<TModel> GetUpdateAllTacticCommand<TModel>(IUnitOfWork unitOfWork, IModelObject model, IModelQuery modelQuery, TableMappingAttribute tableMappingAttribute)
			where TModel : class, IModelObject
		{
			throw new NotImplementedException();
		}

		private TacticCommand<TModel> GetUpdateOneTacticCommand<TModel>(IUnitOfWork unitOfWork, TModel model, TableMappingAttribute tableMappingAttribute) where TModel : class, IModelObject
		{
			TacticCommand<TModel> tacticCommand;
			const bool IS_NULLIPOTENT = false;
			const bool COMMAND_PREPARE = false;
			const object COMMAND_TIMEOUT = null;
			const CommandType COMMAND_TYPE = CommandType.Text;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			int expectedRecordsAffected;
			Action<TModel, IDictionary<string, object>> tableToModelMappingCallback;
			string commandText;
			IDataParameter commandParameter;
			IDictionary<string, IDataParameter> commandParameters;
			ColumnMappingAttribute[] columnMappingAttributes;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			if ((object)tableMappingAttribute == null)
				throw new ArgumentNullException("tableMappingAttribute");

			commandText = @"UPDATE ";

			commandText += this.GetTableName(tableMappingAttribute.SchemaName, tableMappingAttribute.TableName) + @" SET ";
			commandParameters = new Dictionary<string, IDataParameter>();

			columnMappingAttributes = tableMappingAttribute._ColumnMappingAttributes.Where(cma => !cma.IsColumnServerGeneratedPrimaryKey).OrderBy(cma => cma.ColumnOrdinal).ToArray();
			for (int index = 0; index < columnMappingAttributes.Length; index++)
			{
				string parameterName;
				object parameterValue;

				if (!Reflexion.GetLogicalPropertyValue(model, columnMappingAttributes[index]._TargetProperty.Name, out parameterValue))
					throw new InvalidOperationException(string.Format("Ah snap."));

				parameterName = this.GetParameterName(columnMappingAttributes[index].ColumnName);
				commandParameter = unitOfWork.CreateParameter(ParameterDirection.Input, columnMappingAttributes[index].ColumnDbType, columnMappingAttributes[index].ColumnSize, columnMappingAttributes[index].ColumnPrecision, columnMappingAttributes[index].ColumnScale, columnMappingAttributes[index].ColumnNullable, parameterName, parameterValue);
				this.ParameterMagic(unitOfWork, commandParameter, columnMappingAttributes[index].ColumnSqlType);
				commandParameters.Add(parameterName, commandParameter);

				commandText += this.GetColumnName(columnMappingAttributes[index].ColumnName) + @" = " + parameterName;

				if (index != (columnMappingAttributes.Length - 1))
					commandText += @", ";
			}

			commandText += @" WHERE ";

			columnMappingAttributes = tableMappingAttribute._ColumnMappingAttributes.Where(cma => !tableMappingAttribute._ColumnMappingAttributes.Any() || cma.ColumnIsPrimaryKey).OrderBy(cma => cma.ColumnOrdinal).ToArray();
			for (int index = 0; index < columnMappingAttributes.Length; index++)
			{
				string parameterName;
				object parameterValue;

				if (!Reflexion.GetLogicalPropertyValue(model, columnMappingAttributes[index]._TargetProperty.Name, out parameterValue))
					throw new InvalidOperationException(string.Format("Ah snap."));

				parameterName = this.GetParameterName(columnMappingAttributes[index].ColumnName);
				commandParameter = unitOfWork.CreateParameter(ParameterDirection.Input, columnMappingAttributes[index].ColumnDbType, columnMappingAttributes[index].ColumnSize, columnMappingAttributes[index].ColumnPrecision, columnMappingAttributes[index].ColumnScale, columnMappingAttributes[index].ColumnNullable, parameterName, parameterValue);
				this.ParameterMagic(unitOfWork, commandParameter, columnMappingAttributes[index].ColumnSqlType);
				commandParameters.Add(parameterName, commandParameter);

				commandText += this.GetColumnName(columnMappingAttributes[index].ColumnName) + @" = " + parameterName;

				if (index != (columnMappingAttributes.Length - 1))
					commandText += @" AND ";
			}

			commandText += @";";

			expectedRecordsAffected = this.GetExpectedRecordsAffected(IS_NULLIPOTENT);
			tableToModelMappingCallback = GetMapToMethod<TModel>(tableMappingAttribute);

			tacticCommand = new TacticCommand<TModel>()
							{
								CommandBehavior = COMMAND_BEHAVIOR,
								CommandParameters = commandParameters.Values,
								CommandPrepare = COMMAND_PREPARE,
								CommandText = commandText,
								CommandTimeout = (int?)COMMAND_TIMEOUT,
								CommandType = COMMAND_TYPE,
								ExpectedRecordsAffected = expectedRecordsAffected,
								IsNullipotent = IS_NULLIPOTENT,
								TableToModelMappingCallback = tableToModelMappingCallback
							};

			return tacticCommand;
		}

		public TacticCommand<TModel> GetUpdateTacticCommand<TModel>(IUnitOfWork unitOfWork, TModel modelValue, IModelQuery modelQuery) where TModel : class, IModelObject
		{
			TacticCommand<TModel> tacticCommand;
			Type modelType;
			TableMappingAttribute tableMappingAttribute;
			bool isUpdateOne;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)modelValue == null)
				throw new ArgumentNullException("modelValue");

			modelType = typeof(TModel);
			tableMappingAttribute = this.GetTableMapping(modelType);

			AssertValidMapping(modelType, tableMappingAttribute);

			isUpdateOne = (object)modelQuery == null;

			if (isUpdateOne)
				tacticCommand = this.GetUpdateOneTacticCommand<TModel>(unitOfWork, modelValue, tableMappingAttribute);
			else
				tacticCommand = this.GetUpdateAllTacticCommand<TModel>(unitOfWork, modelValue, modelQuery, tableMappingAttribute);

			return tacticCommand;
		}

		public abstract void ParameterMagic(IUnitOfWork unitOfWork, IDataParameter commandParameter, string generatedFromColumnNativeType);

		#endregion
	}
}