/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using TextMetal.Middleware.Common.Fascades.AdoNet.UoW;
using TextMetal.Middleware.Common.Fascades.Utilities;
using TextMetal.Middleware.Data.Impl.FreakazoidMapper.Expressions;
using TextMetal.Middleware.Data.Impl.FreakazoidMapper.Mappings;
using TextMetal.Middleware.Data.Impl.FreakazoidMapper.Tactics;
using TextMetal.Middleware.Data.Models;
using TextMetal.Middleware.Data.Models.Functional;
using TextMetal.Middleware.Data.Models.Tabular;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper.Strategies
{
	public abstract class DataSourceTagStrategy : IDataSourceTagStrategy, ISqlNuance
	{
		#region Constructors/Destructors

		protected DataSourceTagStrategy(string dataSourceTag, bool canCreateNativeDatabaseFile, bool useBatchScopeIdentificationSemantics)
		{
			this.dataSourceTag = dataSourceTag;
			this.canCreateNativeDatabaseFile = canCreateNativeDatabaseFile;
			this.useBatchScopeIdentificationSemantics = useBatchScopeIdentificationSemantics;
		}

		#endregion

		#region Fields/Constants

		private readonly bool canCreateNativeDatabaseFile;
		private readonly string dataSourceTag;
		private readonly bool useBatchScopeIdentificationSemantics;

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

		public bool UseBatchScopeIdentificationSemantics
		{
			get
			{
				return this.useBatchScopeIdentificationSemantics;
			}
		}

		#endregion

		#region Methods/Operators

		private static void AssertValidMapping(Type tableModelType, TableMappingAttribute tableMappingAttribute)
		{
			if ((object)tableModelType == null)
				throw new ArgumentNullException("tableModelType");

			if ((object)tableMappingAttribute == null)
				throw new InvalidOperationException(string.Format("The table model type '{0}' does not specify the '{1}' attribute.", tableModelType.FullName, typeof(TableMappingAttribute).FullName));

			if ((object)tableMappingAttribute._ColumnMappingAttributes == null ||
				tableMappingAttribute._ColumnMappingAttributes.Count == 0)
				Trace.WriteLine(string.Format("The table model type '{0}' does not specify the '{1}' attribute on any public, instance, read-write property.", tableModelType.FullName, typeof(ColumnMappingAttribute).FullName));
		}

		private static void AssertValidMapping(Type callProcedureModelType, Type resultModelType, Type returnProcedureModelType, ProcedureMappingAttribute procedureMappingAttribute)
		{
			if ((object)callProcedureModelType == null)
				throw new ArgumentNullException("callProcedureModelType");

			if ((object)resultModelType == null)
				throw new ArgumentNullException("resultModelType");

			if ((object)returnProcedureModelType == null)
				throw new ArgumentNullException("returnProcedureModelType");

			if ((object)procedureMappingAttribute == null)
				throw new InvalidOperationException(string.Format("The call procedure model type '{0}' does not specify the '{1}' attribute.", callProcedureModelType.FullName, typeof(ProcedureMappingAttribute).FullName));

			if ((object)procedureMappingAttribute._InputParameterMappingAttributes == null ||
				procedureMappingAttribute._InputParameterMappingAttributes.Count == 0)
				Trace.WriteLine(string.Format("The call procedure model type '{0}' does not specify the '{1}' attribute on any public, instance, read-write property.", callProcedureModelType.FullName, typeof(ParameterMappingAttribute).FullName));

			if ((object)procedureMappingAttribute._ResultColumnMappingAttributes == null ||
				procedureMappingAttribute._ResultColumnMappingAttributes.Count == 0)
				Trace.WriteLine(string.Format("The result model type '{0}' does not specify the '{1}' attribute on any public, instance, read-write property.", resultModelType.FullName, typeof(ColumnMappingAttribute).FullName));

			if ((object)procedureMappingAttribute._OutputParameterMappingAttributes == null ||
				procedureMappingAttribute._OutputParameterMappingAttributes.Count == 0)
				Trace.WriteLine(string.Format("The return procedure model type '{0}' does not specify the '{1}' attribute on any public, instance, read-write property.", resultModelType.FullName, typeof(ParameterMappingAttribute).FullName));
		}

		public bool CreateNativeDatabaseFile(string databaseFilePath)
		{
			// do nothing
			return false;
		}

		public abstract void FixupParameter(IUnitOfWork unitOfWork, ITacticParameter tacticParameter, string generatedFromColumnNativeType);

		public abstract string GetAliasedColumnName(string tableAlias, string columnName);

		public abstract string GetColumnName(string columnName);

		private ITableTacticCommand<TTableModelObject> GetDeleteAllTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject, ITableModelQuery tableModelQuery, TableMappingAttribute tableMappingAttribute)
			where TTableModelObject : class, ITableModelObject
		{
			throw new NotImplementedException();
		}

		private ITableTacticCommand<TTableModelObject> GetDeleteOneTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject prototype, TableMappingAttribute tableMappingAttribute)
			where TTableModelObject : class, ITableModelObject
		{
			TableTacticCommand<TTableModelObject> tableTacticCommand;
			const bool IS_NULLIPOTENT = false;
			const bool COMMAND_PREPARE = false;
			const object COMMAND_TIMEOUT = null;
			const CommandType COMMAND_TYPE = CommandType.Text;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			int expectedRecordsAffected;
			Action<TTableModelObject, IDictionary<string, object>> tableToModelMappingCallback;
			string commandText;
			ITacticParameter tacticParameter;
			IDictionary<string, ITacticParameter> tacticParameters;
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
			tacticParameters = new Dictionary<string, ITacticParameter>();

			columnMappingAttributes = tableMappingAttribute._ColumnMappingAttributes.Where(cma => !tableMappingAttribute._ColumnMappingAttributes.Any() || cma.ColumnIsPrimaryKey).OrderBy(cma => cma.ColumnOrdinal).ToArray();
			for (int index = 0; index < columnMappingAttributes.Length; index++)
			{
				string parameterName;
				object parameterValue;

				if (!ReflectionFascade.Instance.GetLogicalPropertyValue(prototype, columnMappingAttributes[index]._TargetProperty.Name, out parameterValue))
					throw new InvalidOperationException(string.Format("Ah snap."));

				parameterName = this.GetParameterName(columnMappingAttributes[index].ColumnName);

				tacticParameter = new TacticParameter()
								{
									ParameterDirection = ParameterDirection.Input,
									ParameterDbType = columnMappingAttributes[index].ColumnDbType,
									ParameterSize = columnMappingAttributes[index].ColumnSize,
									ParameterPrecision = columnMappingAttributes[index].ColumnPrecision,
									ParameterScale = columnMappingAttributes[index].ColumnScale,
									ParameterNullable = columnMappingAttributes[index].ColumnNullable,
									ParameterName = parameterName,
									ParameterValue = parameterValue
								};

				this.FixupParameter(unitOfWork, tacticParameter, columnMappingAttributes[index].ColumnSqlType);
				tacticParameters.Add(parameterName, tacticParameter);

				commandText += this.GetColumnName(columnMappingAttributes[index].ColumnName) + @" = " + parameterName;

				if (index != (columnMappingAttributes.Length - 1))
					commandText += @" AND ";
			}

			commandText += @";";

			expectedRecordsAffected = this.GetExpectedRecordsAffected(IS_NULLIPOTENT);
			tableToModelMappingCallback = this.GetMapToMethod<TTableModelObject>(tableMappingAttribute);

			tableTacticCommand = new TableTacticCommand<TTableModelObject>()
								{
									UseBatchScopeIdentificationSemantics = this.UseBatchScopeIdentificationSemantics,
									CommandBehavior = COMMAND_BEHAVIOR,
									TacticParameters = tacticParameters.Values,
									CommandPrepare = COMMAND_PREPARE,
									CommandText = commandText,
									CommandTimeout = (int?)COMMAND_TIMEOUT,
									CommandType = COMMAND_TYPE,
									ExpectedRecordsAffected = expectedRecordsAffected,
									IsNullipotent = IS_NULLIPOTENT,
									RecordToTableModelMappingCallback = tableToModelMappingCallback
								};

			return tableTacticCommand;
		}

		public ITableTacticCommand<TTableModelObject> GetDeleteTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject modelValue, ITableModelQuery tableModelQuery)
			where TTableModelObject : class, ITableModelObject, new()
		{
			ITableTacticCommand<TTableModelObject> tableTacticCommand;
			Type modelType;
			TableMappingAttribute tableMappingAttribute;
			bool isDeleteOne;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)modelValue == null)
				throw new ArgumentNullException("modelValue");

			modelType = typeof(TTableModelObject);
			tableMappingAttribute = this.GetTableMapping(modelType);

			AssertValidMapping(modelType, tableMappingAttribute);

			isDeleteOne = (object)tableModelQuery == null;

			if (isDeleteOne)
				tableTacticCommand = this.GetDeleteOneTacticCommand<TTableModelObject>(unitOfWork, modelValue, tableMappingAttribute);
			else
				tableTacticCommand = this.GetDeleteAllTacticCommand<TTableModelObject>(unitOfWork, modelValue, tableModelQuery, tableMappingAttribute);

			return tableTacticCommand;
		}

		private IProcedureTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject> GetExecuteTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, TCallProcedureModelObject callProcedureModelValue, ProcedureMappingAttribute procedureMappingAttribute)
			where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new()
		{
			ProcedureTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject> procedureTacticCommand;
			const bool IS_NULLIPOTENT = true;
			const bool COMMAND_PREPARE = false;
			const object COMMAND_TIMEOUT = null;
			const CommandType COMMAND_TYPE = CommandType.StoredProcedure;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			int expectedRecordsAffected;
			Action<TResultProcedureModelObject, IDictionary<string, object>> recordToResultModelMappingCallback;
			Action<TReturnProcedureModelObject, IDictionary<string, object>> outputToReturnProcedureModelMappingCallback;
			string commandText;
			ITacticParameter tacticParameter;
			IDictionary<string, ITacticParameter> tacticParameters;
			ParameterMappingAttribute[] parameterMappingAttributes;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)procedureMappingAttribute == null)
				throw new ArgumentNullException("procedureMappingAttribute");

			commandText = this.GetProcedureName(procedureMappingAttribute.SchemaName, procedureMappingAttribute.ProcedureName);
			tacticParameters = new Dictionary<string, ITacticParameter>();

			var temp = new List<ParameterMappingAttribute>();
			temp.AddRange(procedureMappingAttribute._InputParameterMappingAttributes);
			temp.AddRange(procedureMappingAttribute._OutputParameterMappingAttributes);

			parameterMappingAttributes = temp.OrderBy(pma => pma.ParameterOrdinal).ToArray();
			for (int index = 0; index < parameterMappingAttributes.Length; index++)
			{
				string parameterName;
				object parameterValue = null;

				if (parameterMappingAttributes[index].ParameterDirection == ParameterDirection.Input ||
					parameterMappingAttributes[index].ParameterDirection == ParameterDirection.InputOutput)
				{
					if (!ReflectionFascade.Instance.GetLogicalPropertyValue(callProcedureModelValue, parameterMappingAttributes[index]._TargetProperty.Name, out parameterValue))
						throw new InvalidOperationException(string.Format("Ah snap."));
				}

				parameterName = this.GetParameterName(parameterMappingAttributes[index].ParameterName);

				tacticParameter = new TacticParameter()
								{
									ParameterDirection = parameterMappingAttributes[index].ParameterDirection,
									ParameterDbType = parameterMappingAttributes[index].ParameterDbType,
									ParameterSize = parameterMappingAttributes[index].ParameterSize,
									ParameterPrecision = parameterMappingAttributes[index].ParameterPrecision,
									ParameterScale = parameterMappingAttributes[index].ParameterScale,
									ParameterNullable = parameterMappingAttributes[index].ParameterNullable,
									ParameterName = parameterName,
									ParameterValue = parameterValue
								};

				this.FixupParameter(unitOfWork, tacticParameter, parameterMappingAttributes[index].ParameterSqlType);

				if (!tacticParameters.ContainsKey(parameterName))
					tacticParameters.Add(parameterName, tacticParameter);
			}

			expectedRecordsAffected = this.GetExpectedRecordsAffected(IS_NULLIPOTENT);
			recordToResultModelMappingCallback = this.GetMapToMethod<TResultProcedureModelObject>(procedureMappingAttribute);
			outputToReturnProcedureModelMappingCallback = this.GetMapToMethod<TResultProcedureModelObject, TReturnProcedureModelObject>(procedureMappingAttribute);

			procedureTacticCommand = new ProcedureTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>()
									{
										CommandBehavior = COMMAND_BEHAVIOR,
										TacticParameters = tacticParameters.Values,
										CommandPrepare = COMMAND_PREPARE,
										CommandText = commandText,
										CommandTimeout = (int?)COMMAND_TIMEOUT,
										CommandType = COMMAND_TYPE,
										ExpectedRecordsAffected = expectedRecordsAffected,
										IsNullipotent = IS_NULLIPOTENT,
										RecordToResultModelMappingCallback = recordToResultModelMappingCallback,
										OutputToReturnProcedureModelMappingCallback = outputToReturnProcedureModelMappingCallback
									};

			return procedureTacticCommand;
		}

		public IProcedureTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject> GetExecuteTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, TCallProcedureModelObject callProcedureModelValue)
			where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new()
		{
			IProcedureTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject> procedureTacticCommand;
			Type callProcedureModelType;
			Type resultModelType;
			Type returnProcedureModelType;
			ProcedureMappingAttribute procedureMappingAttribute;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)callProcedureModelValue == null)
				throw new ArgumentNullException("callProcedureModelValue");

			callProcedureModelType = typeof(TCallProcedureModelObject);
			resultModelType = typeof(TResultProcedureModelObject);
			returnProcedureModelType = typeof(TReturnProcedureModelObject);
			procedureMappingAttribute = this.GetProcedureMapping(callProcedureModelType, resultModelType, returnProcedureModelType);

			AssertValidMapping(callProcedureModelType, resultModelType, returnProcedureModelType, procedureMappingAttribute);

			procedureTacticCommand = this.GetExecuteTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(unitOfWork, callProcedureModelValue, procedureMappingAttribute);

			return procedureTacticCommand;
		}

		public abstract int GetExpectedRecordsAffected(bool isNullipotent);

		public ITableTacticCommand<TTableModelObject> GetIdentifyTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork)
			where TTableModelObject : class, ITableModelObject, new()
		{
			ITableTacticCommand<TTableModelObject> tableTacticCommand;
			Type modelType;
			TableMappingAttribute tableMappingAttribute;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			modelType = typeof(TTableModelObject);
			tableMappingAttribute = this.GetTableMapping(modelType);

			AssertValidMapping(modelType, tableMappingAttribute);

			tableTacticCommand = this.GetIdentifyTacticCommand<TTableModelObject>(unitOfWork, tableMappingAttribute);

			return tableTacticCommand;
		}

		private ITableTacticCommand<TTableModelObject> GetIdentifyTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, TableMappingAttribute tableMappingAttribute)
			where TTableModelObject : class, ITableModelObject
		{
			TableTacticCommand<TTableModelObject> tableTacticCommand;
			const bool IS_NULLIPOTENT = true;
			const bool COMMAND_PREPARE = false;
			const object COMMAND_TIMEOUT = null;
			const CommandType COMMAND_TYPE = CommandType.Text;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			int expectedRecordsAffected;
			Action<TTableModelObject, IDictionary<string, object>> tableToModelMappingCallback;
			string commandText;
			ITacticParameter tacticParameter;
			IDictionary<string, ITacticParameter> tacticParameters;
			ColumnMappingAttribute columnMappingAttribute;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableMappingAttribute == null)
				throw new ArgumentNullException("tableMappingAttribute");

			if (this.UseBatchScopeIdentificationSemantics)
				throw new InvalidOperationException(string.Format("Aw snap."));

			tacticParameters = new Dictionary<string, ITacticParameter>();
			columnMappingAttribute = tableMappingAttribute._ColumnMappingAttributes.Where(cma => cma.IsColumnServerGeneratedPrimaryKey).SingleOrDefault();

			if ((object)columnMappingAttribute == null)
				return null;

			commandText = @"SELECT " + this.GetIdentityFunctionName() + @" AS " + this.GetColumnName(columnMappingAttribute.ColumnName) + @";";

			expectedRecordsAffected = this.GetExpectedRecordsAffected(IS_NULLIPOTENT);
			tableToModelMappingCallback = this.GetMapToMethod<TTableModelObject>(tableMappingAttribute);

			tableTacticCommand = new TableTacticCommand<TTableModelObject>()
								{
									UseBatchScopeIdentificationSemantics = this.UseBatchScopeIdentificationSemantics,
									CommandBehavior = COMMAND_BEHAVIOR,
									TacticParameters = tacticParameters.Values,
									CommandPrepare = COMMAND_PREPARE,
									CommandText = commandText,
									CommandTimeout = (int?)COMMAND_TIMEOUT,
									CommandType = COMMAND_TYPE,
									ExpectedRecordsAffected = expectedRecordsAffected,
									IsNullipotent = IS_NULLIPOTENT,
									RecordToTableModelMappingCallback = tableToModelMappingCallback
								};

			return tableTacticCommand;
		}

		public abstract string GetIdentityFunctionName();

		private ITableTacticCommand<TTableModelObject> GetInsertAllTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, IModelObject model, ITableModelQuery tableModelQuery, TableMappingAttribute tableMappingAttribute)
			where TTableModelObject : class, ITableModelObject
		{
			throw new NotImplementedException();
		}

		private ITableTacticCommand<TTableModelObject> GetInsertOneTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject model, TableMappingAttribute tableMappingAttribute) where TTableModelObject : class, ITableModelObject
		{
			TableTacticCommand<TTableModelObject> tableTacticCommand;
			const bool IS_NULLIPOTENT = false;
			const bool COMMAND_PREPARE = false;
			const object COMMAND_TIMEOUT = null;
			const CommandType COMMAND_TYPE = CommandType.Text;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			int expectedRecordsAffected;
			Action<TTableModelObject, IDictionary<string, object>> tableToModelMappingCallback;
			string commandText;
			ITacticParameter tacticParameter;
			IDictionary<string, ITacticParameter> tacticParameters;
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
			tacticParameters = new Dictionary<string, ITacticParameter>();

			// yes, this is redundant loop but it makes it easier to maintain for now
			for (int index = 0; index < columnMappingAttributes.Length; index++)
			{
				string parameterName;
				object parameterValue;

				if (!ReflectionFascade.Instance.GetLogicalPropertyValue(model, columnMappingAttributes[index]._TargetProperty.Name, out parameterValue))
					throw new InvalidOperationException(string.Format("Ah snap."));

				parameterName = this.GetParameterName(columnMappingAttributes[index].ColumnName);

				tacticParameter = new TacticParameter()
								{
									ParameterDirection = ParameterDirection.Input,
									ParameterDbType = columnMappingAttributes[index].ColumnDbType,
									ParameterSize = columnMappingAttributes[index].ColumnSize,
									ParameterPrecision = columnMappingAttributes[index].ColumnPrecision,
									ParameterScale = columnMappingAttributes[index].ColumnScale,
									ParameterNullable = columnMappingAttributes[index].ColumnNullable,
									ParameterName = parameterName,
									ParameterValue = parameterValue
								};

				this.FixupParameter(unitOfWork, tacticParameter, columnMappingAttributes[index].ColumnSqlType);
				tacticParameters.Add(parameterName, tacticParameter);

				commandText += parameterName;

				if (index != (columnMappingAttributes.Length - 1))
					commandText += @", ";
			}

			commandText += @");";

			if (this.UseBatchScopeIdentificationSemantics)
			{
				ColumnMappingAttribute columnMappingAttribute;

				columnMappingAttribute = tableMappingAttribute._ColumnMappingAttributes.Where(cma => cma.IsColumnServerGeneratedPrimaryKey).SingleOrDefault();

				if ((object)columnMappingAttribute != null)
					commandText += Environment.NewLine + @"SELECT " + this.GetIdentityFunctionName() + @" AS " + this.GetColumnName(columnMappingAttribute.ColumnName) + @";";
			}

			expectedRecordsAffected = this.GetExpectedRecordsAffected(IS_NULLIPOTENT);
			tableToModelMappingCallback = this.GetMapToMethod<TTableModelObject>(tableMappingAttribute);

			tableTacticCommand = new TableTacticCommand<TTableModelObject>()
								{
									UseBatchScopeIdentificationSemantics = this.UseBatchScopeIdentificationSemantics,
									CommandBehavior = COMMAND_BEHAVIOR,
									TacticParameters = tacticParameters.Values,
									CommandPrepare = COMMAND_PREPARE,
									CommandText = commandText,
									CommandTimeout = (int?)COMMAND_TIMEOUT,
									CommandType = COMMAND_TYPE,
									ExpectedRecordsAffected = expectedRecordsAffected,
									IsNullipotent = IS_NULLIPOTENT,
									RecordToTableModelMappingCallback = tableToModelMappingCallback
								};

			return tableTacticCommand;
		}

		public ITableTacticCommand<TTableModelObject> GetInsertTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject modelValue, ITableModelQuery tableModelQuery)
			where TTableModelObject : class, ITableModelObject, new()
		{
			ITableTacticCommand<TTableModelObject> tableTacticCommand;
			Type modelType;
			TableMappingAttribute tableMappingAttribute;
			bool isInsertOne;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)modelValue == null)
				throw new ArgumentNullException("modelValue");

			modelType = typeof(TTableModelObject);
			tableMappingAttribute = this.GetTableMapping(modelType);

			AssertValidMapping(modelType, tableMappingAttribute);

			isInsertOne = (object)tableModelQuery == null;

			if (isInsertOne)
				tableTacticCommand = this.GetInsertOneTacticCommand<TTableModelObject>(unitOfWork, modelValue, tableMappingAttribute);
			else
				tableTacticCommand = this.GetInsertAllTacticCommand<TTableModelObject>(unitOfWork, modelValue, tableModelQuery, tableMappingAttribute);

			return tableTacticCommand;
		}

		private Action<TResultProcedureModelObject, IDictionary<string, object>> GetMapToMethod<TResultProcedureModelObject>(ProcedureMappingAttribute procedureMappingAttribute)
			where TResultProcedureModelObject : class, IResultProcedureModelObject
		{
			Action<TResultProcedureModelObject, IDictionary<string, object>> callback;

			if ((object)procedureMappingAttribute == null)
				throw new ArgumentNullException("procedureMappingAttribute");

			callback = (md, ts) => this.MapRecordToResultModel(procedureMappingAttribute._ResultColumnMappingAttributes, md, ts);

			return callback;
		}

		private Action<TReturnProcedureModelObject, IDictionary<string, object>> GetMapToMethod<TResultProcedureModelObject, TReturnProcedureModelObject>(ProcedureMappingAttribute procedureMappingAttribute)
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new()
		{
			Action<TReturnProcedureModelObject, IDictionary<string, object>> callback;

			if ((object)procedureMappingAttribute == null)
				throw new ArgumentNullException("procedureMappingAttribute");

			callback = (md, ts) => this.MapOutputToReturnProcedureModel<TResultProcedureModelObject, TReturnProcedureModelObject>(procedureMappingAttribute, md, ts);

			return callback;
		}

		private Action<TTableModelObject, IDictionary<string, object>> GetMapToMethod<TTableModelObject>(TableMappingAttribute tableMappingAttribute)
			where TTableModelObject : class, ITableModelObject
		{
			Action<TTableModelObject, IDictionary<string, object>> callback;

			if ((object)tableMappingAttribute == null)
				throw new ArgumentNullException("tableMappingAttribute");

			callback = (md, ts) => this.MapRecordToResultModel<TTableModelObject>(tableMappingAttribute._ColumnMappingAttributes, md, ts);

			return callback;
		}

		public string GetOrderByListFragment(IEnumerable<SortOrder> sortOrders)
		{
			string expressionText;
			List<string> orderByList;

			if ((object)sortOrders == null)
				throw new ArgumentNullException("sortOrders");

			orderByList = new List<string>();

			foreach (var sortOrder in sortOrders)
			{
				if ((object)sortOrder.SymbolName == null)
					continue;

				if (sortOrder.SortDirection == SortDirection.Undefined)
					continue;

				orderByList.Add(string.Format("{0} {1}", this.GetAliasedColumnName("t0", sortOrder.SymbolName.Name), (sortOrder.SortDirection == SortDirection.Asc) ? "ASC" : "DESC"));
			}

			if (orderByList.Count <= 0)
				return null;

			expressionText = string.Join(", ", orderByList.ToArray());

			return expressionText;
		}

		public abstract string GetParameterName(string parameterName);

		protected virtual ProcedureMappingAttribute GetProcedureMapping(Type callProcedureModelType, Type resultModelType, Type returnProcedureModelType)
		{
			ProcedureMappingAttribute procedureMappingAttribute;
			ParameterMappingAttribute parameterMappingAttribute;
			ColumnMappingAttribute columnMappingAttribute;
			PropertyInfo[] propertyInfos;

			if ((object)callProcedureModelType == null)
				throw new ArgumentNullException("callProcedureModelType");

			if ((object)resultModelType == null)
				throw new ArgumentNullException("resultModelType");

			if ((object)returnProcedureModelType == null)
				throw new ArgumentNullException("returnProcedureModelType");

			procedureMappingAttribute = ReflectionFascade.Instance.GetOneAttribute<ProcedureMappingAttribute>(callProcedureModelType);

			if ((object)procedureMappingAttribute == null)
				return null;

			procedureMappingAttribute._TargetType = callProcedureModelType;

			propertyInfos = callProcedureModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			if ((object)propertyInfos != null)
			{
				foreach (PropertyInfo propertyInfo in propertyInfos)
				{
					if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
						continue;

					parameterMappingAttribute = ReflectionFascade.Instance.GetOneAttribute<ParameterMappingAttribute>(propertyInfo);

					if ((object)parameterMappingAttribute == null)
						continue;

					if (parameterMappingAttribute.ParameterDirection != ParameterDirection.Input &&
						parameterMappingAttribute.ParameterDirection != ParameterDirection.InputOutput)
						continue;

					parameterMappingAttribute._TargetProperty = propertyInfo;

					procedureMappingAttribute._InputParameterMappingAttributes.Add(parameterMappingAttribute);
				}
			}

			propertyInfos = resultModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			if ((object)propertyInfos != null)
			{
				foreach (PropertyInfo propertyInfo in propertyInfos)
				{
					if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
						continue;

					columnMappingAttribute = ReflectionFascade.Instance.GetOneAttribute<ColumnMappingAttribute>(propertyInfo);

					if ((object)columnMappingAttribute == null)
						continue;

					columnMappingAttribute._TargetProperty = propertyInfo;

					procedureMappingAttribute._ResultColumnMappingAttributes.Add(columnMappingAttribute);
				}
			}

			propertyInfos = returnProcedureModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			if ((object)propertyInfos != null)
			{
				foreach (PropertyInfo propertyInfo in propertyInfos)
				{
					if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
						continue;

					parameterMappingAttribute = ReflectionFascade.Instance.GetOneAttribute<ParameterMappingAttribute>(propertyInfo);

					if ((object)parameterMappingAttribute == null)
						continue;

					if (parameterMappingAttribute.ParameterDirection != ParameterDirection.InputOutput &&
						parameterMappingAttribute.ParameterDirection != ParameterDirection.Output &&
						parameterMappingAttribute.ParameterDirection != ParameterDirection.ReturnValue)
						continue;

					parameterMappingAttribute._TargetProperty = propertyInfo;

					procedureMappingAttribute._OutputParameterMappingAttributes.Add(parameterMappingAttribute);
				}
			}

			return procedureMappingAttribute;
		}

		public abstract string GetProcedureName(string schemaName, string procedureName);

		private ITableTacticCommand<TTableModelObject> GetSelectAllTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, IModelObject dummy, ITableModelQuery tableModelQuery, TableMappingAttribute tableMappingAttribute)
			where TTableModelObject : class, ITableModelObject
		{
			TableTacticCommand<TTableModelObject> tableTacticCommand;
			const bool IS_NULLIPOTENT = true;
			const bool COMMAND_PREPARE = false;
			const object COMMAND_TIMEOUT = null;
			const CommandType COMMAND_TYPE = CommandType.Text;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;
			const string TABLE_ALIAS = "t0";

			int expectedRecordsAffected;
			Action<TTableModelObject, IDictionary<string, object>> tableToModelMappingCallback;
			string commandText;
			ITacticParameter tacticParameter;
			IDictionary<string, ITacticParameter> tacticParameters;
			ColumnMappingAttribute[] columnMappingAttributes;
			FreakazoidTableModelQuery freakazoidTableModelQuery;

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

			tacticParameters = new Dictionary<string, ITacticParameter>();

			freakazoidTableModelQuery = tableModelQuery as FreakazoidTableModelQuery;

			if ((object)freakazoidTableModelQuery == null)
				throw new InvalidOperationException(string.Format("Ah snap."));

			var wherePredicateFragment = this.GetWherePredicateFragment(tableMappingAttribute, unitOfWork, tacticParameters, freakazoidTableModelQuery.FilterExpression);

			if (!DataTypeFascade.Instance.IsNullOrWhiteSpace(wherePredicateFragment))
			{
				commandText += @" WHERE {0}";
				commandText = string.Format(commandText, wherePredicateFragment);
			}

			var orderByListFragment = this.GetOrderByListFragment(freakazoidTableModelQuery.SortOrders);

			if (!DataTypeFascade.Instance.IsNullOrWhiteSpace(orderByListFragment))
			{
				commandText += " ORDER BY {0}";
				commandText = string.Format(commandText, orderByListFragment);
			}

			commandText += @";";

			expectedRecordsAffected = this.GetExpectedRecordsAffected(IS_NULLIPOTENT);
			tableToModelMappingCallback = this.GetMapToMethod<TTableModelObject>(tableMappingAttribute);

			tableTacticCommand = new TableTacticCommand<TTableModelObject>()
								{
									UseBatchScopeIdentificationSemantics = this.UseBatchScopeIdentificationSemantics,
									CommandBehavior = COMMAND_BEHAVIOR,
									TacticParameters = tacticParameters.Values,
									CommandPrepare = COMMAND_PREPARE,
									CommandText = commandText,
									CommandTimeout = (int?)COMMAND_TIMEOUT,
									CommandType = COMMAND_TYPE,
									ExpectedRecordsAffected = expectedRecordsAffected,
									IsNullipotent = IS_NULLIPOTENT,
									RecordToTableModelMappingCallback = tableToModelMappingCallback
								};

			return tableTacticCommand;
		}

		private ITableTacticCommand<TTableModelObject> GetSelectOneTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject prototype, TableMappingAttribute tableMappingAttribute)
			where TTableModelObject : class, ITableModelObject
		{
			TableTacticCommand<TTableModelObject> tableTacticCommand;
			const bool IS_NULLIPOTENT = true;
			const bool COMMAND_PREPARE = false;
			const object COMMAND_TIMEOUT = null;
			const CommandType COMMAND_TYPE = CommandType.Text;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;
			const string TABLE_ALIAS = "t0";

			int expectedRecordsAffected;
			Action<TTableModelObject, IDictionary<string, object>> tableToModelMappingCallback;
			string commandText;
			ITacticParameter tacticParameter;
			IDictionary<string, ITacticParameter> tacticParameters;
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
			tacticParameters = new Dictionary<string, ITacticParameter>();

			columnMappingAttributes = tableMappingAttribute._ColumnMappingAttributes.Where(cma => !tableMappingAttribute._ColumnMappingAttributes.Any() || cma.ColumnIsPrimaryKey).OrderBy(cma => cma.ColumnOrdinal).ToArray();
			for (int index = 0; index < columnMappingAttributes.Length; index++)
			{
				string parameterName;
				object parameterValue;

				if (!ReflectionFascade.Instance.GetLogicalPropertyValue(prototype, columnMappingAttributes[index]._TargetProperty.Name, out parameterValue))
					throw new InvalidOperationException(string.Format("Ah snap."));

				parameterName = this.GetParameterName(columnMappingAttributes[index].ColumnName);

				tacticParameter = new TacticParameter()
								{
									ParameterDirection = ParameterDirection.Input,
									ParameterDbType = columnMappingAttributes[index].ColumnDbType,
									ParameterSize = columnMappingAttributes[index].ColumnSize,
									ParameterPrecision = columnMappingAttributes[index].ColumnPrecision,
									ParameterScale = columnMappingAttributes[index].ColumnScale,
									ParameterNullable = columnMappingAttributes[index].ColumnNullable,
									ParameterName = parameterName,
									ParameterValue = parameterValue
								};

				this.FixupParameter(unitOfWork, tacticParameter, columnMappingAttributes[index].ColumnSqlType);
				tacticParameters.Add(parameterName, tacticParameter);

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
			tableToModelMappingCallback = this.GetMapToMethod<TTableModelObject>(tableMappingAttribute);

			tableTacticCommand = new TableTacticCommand<TTableModelObject>()
								{
									UseBatchScopeIdentificationSemantics = this.UseBatchScopeIdentificationSemantics,
									CommandBehavior = COMMAND_BEHAVIOR,
									TacticParameters = tacticParameters.Values,
									CommandPrepare = COMMAND_PREPARE,
									CommandText = commandText,
									CommandTimeout = (int?)COMMAND_TIMEOUT,
									CommandType = COMMAND_TYPE,
									ExpectedRecordsAffected = expectedRecordsAffected,
									IsNullipotent = IS_NULLIPOTENT,
									RecordToTableModelMappingCallback = tableToModelMappingCallback
								};

			return tableTacticCommand;
		}

		public ITableTacticCommand<TTableModelObject> GetSelectTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject modelValue, ITableModelQuery tableModelQuery)
			where TTableModelObject : class, ITableModelObject, new()
		{
			ITableTacticCommand<TTableModelObject> tableTacticCommand;
			Type modelType;
			TableMappingAttribute tableMappingAttribute;
			bool isSelectOne;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)modelValue == null)
				throw new ArgumentNullException("modelValue");

			modelType = typeof(TTableModelObject);
			tableMappingAttribute = this.GetTableMapping(modelType);

			AssertValidMapping(modelType, tableMappingAttribute);

			isSelectOne = (object)tableModelQuery == null;

			if (isSelectOne)
				tableTacticCommand = this.GetSelectOneTacticCommand<TTableModelObject>(unitOfWork, modelValue, tableMappingAttribute);
			else
				tableTacticCommand = this.GetSelectAllTacticCommand<TTableModelObject>(unitOfWork, modelValue, tableModelQuery, tableMappingAttribute);

			return tableTacticCommand;
		}

		public abstract string GetTableAlias(string tableAlias);

		protected virtual TableMappingAttribute GetTableMapping(Type targetType)
		{
			TableMappingAttribute tableMappingAttribute;
			ColumnMappingAttribute columnMappingAttribute;
			PropertyInfo[] propertyInfos;

			if ((object)targetType == null)
				throw new ArgumentNullException("targetType");

			tableMappingAttribute = ReflectionFascade.Instance.GetOneAttribute<TableMappingAttribute>(targetType);

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

					columnMappingAttribute = ReflectionFascade.Instance.GetOneAttribute<ColumnMappingAttribute>(propertyInfo);

					if ((object)columnMappingAttribute == null)
						continue;

					columnMappingAttribute._TargetProperty = propertyInfo;

					tableMappingAttribute._ColumnMappingAttributes.Add(columnMappingAttribute);
				}
			}

			return tableMappingAttribute;
		}

		public abstract string GetTableName(string schemaName, string tableName);

		private ITableTacticCommand<TTableModelObject> GetUpdateAllTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, IModelObject model, ITableModelQuery tableModelQuery, TableMappingAttribute tableMappingAttribute)
			where TTableModelObject : class, ITableModelObject
		{
			throw new NotImplementedException();
		}

		private ITableTacticCommand<TTableModelObject> GetUpdateOneTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject model, TableMappingAttribute tableMappingAttribute) where TTableModelObject : class, ITableModelObject
		{
			TableTacticCommand<TTableModelObject> tableTacticCommand;
			const bool IS_NULLIPOTENT = false;
			const bool COMMAND_PREPARE = false;
			const object COMMAND_TIMEOUT = null;
			const CommandType COMMAND_TYPE = CommandType.Text;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			int expectedRecordsAffected;
			Action<TTableModelObject, IDictionary<string, object>> tableToModelMappingCallback;
			string commandText;
			ITacticParameter tacticParameter;
			IDictionary<string, ITacticParameter> tacticParameters;
			ColumnMappingAttribute[] columnMappingAttributes;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			if ((object)tableMappingAttribute == null)
				throw new ArgumentNullException("tableMappingAttribute");

			commandText = @"UPDATE ";

			commandText += this.GetTableName(tableMappingAttribute.SchemaName, tableMappingAttribute.TableName) + @" SET ";
			tacticParameters = new Dictionary<string, ITacticParameter>();

			columnMappingAttributes = tableMappingAttribute._ColumnMappingAttributes.Where(cma => !cma.IsColumnServerGeneratedPrimaryKey).OrderBy(cma => cma.ColumnOrdinal).ToArray();
			for (int index = 0; index < columnMappingAttributes.Length; index++)
			{
				string parameterName;
				object parameterValue;

				if (!ReflectionFascade.Instance.GetLogicalPropertyValue(model, columnMappingAttributes[index]._TargetProperty.Name, out parameterValue))
					throw new InvalidOperationException(string.Format("Ah snap."));

				parameterName = this.GetParameterName(columnMappingAttributes[index].ColumnName);

				tacticParameter = new TacticParameter()
								{
									ParameterDirection = ParameterDirection.Input,
									ParameterDbType = columnMappingAttributes[index].ColumnDbType,
									ParameterSize = columnMappingAttributes[index].ColumnSize,
									ParameterPrecision = columnMappingAttributes[index].ColumnPrecision,
									ParameterScale = columnMappingAttributes[index].ColumnScale,
									ParameterNullable = columnMappingAttributes[index].ColumnNullable,
									ParameterName = parameterName,
									ParameterValue = parameterValue
								};

				this.FixupParameter(unitOfWork, tacticParameter, columnMappingAttributes[index].ColumnSqlType);
				tacticParameters.Add(parameterName, tacticParameter);

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

				if (!ReflectionFascade.Instance.GetLogicalPropertyValue(model, columnMappingAttributes[index]._TargetProperty.Name, out parameterValue))
					throw new InvalidOperationException(string.Format("Ah snap."));

				parameterName = this.GetParameterName(columnMappingAttributes[index].ColumnName);

				tacticParameter = new TacticParameter()
								{
									ParameterDirection = ParameterDirection.Input,
									ParameterDbType = columnMappingAttributes[index].ColumnDbType,
									ParameterSize = columnMappingAttributes[index].ColumnSize,
									ParameterPrecision = columnMappingAttributes[index].ColumnPrecision,
									ParameterScale = columnMappingAttributes[index].ColumnScale,
									ParameterNullable = columnMappingAttributes[index].ColumnNullable,
									ParameterName = parameterName,
									ParameterValue = parameterValue
								};

				this.FixupParameter(unitOfWork, tacticParameter, columnMappingAttributes[index].ColumnSqlType);
				tacticParameters.Add(parameterName, tacticParameter);

				commandText += this.GetColumnName(columnMappingAttributes[index].ColumnName) + @" = " + parameterName;

				if (index != (columnMappingAttributes.Length - 1))
					commandText += @" AND ";
			}

			commandText += @";";

			expectedRecordsAffected = this.GetExpectedRecordsAffected(IS_NULLIPOTENT);
			tableToModelMappingCallback = this.GetMapToMethod<TTableModelObject>(tableMappingAttribute);

			tableTacticCommand = new TableTacticCommand<TTableModelObject>()
								{
									UseBatchScopeIdentificationSemantics = this.UseBatchScopeIdentificationSemantics,
									CommandBehavior = COMMAND_BEHAVIOR,
									TacticParameters = tacticParameters.Values,
									CommandPrepare = COMMAND_PREPARE,
									CommandText = commandText,
									CommandTimeout = (int?)COMMAND_TIMEOUT,
									CommandType = COMMAND_TYPE,
									ExpectedRecordsAffected = expectedRecordsAffected,
									IsNullipotent = IS_NULLIPOTENT,
									RecordToTableModelMappingCallback = tableToModelMappingCallback
								};

			return tableTacticCommand;
		}

		public ITableTacticCommand<TTableModelObject> GetUpdateTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject modelValue, ITableModelQuery tableModelQuery)
			where TTableModelObject : class, ITableModelObject, new()
		{
			ITableTacticCommand<TTableModelObject> tableTacticCommand;
			Type modelType;
			TableMappingAttribute tableMappingAttribute;
			bool isUpdateOne;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)modelValue == null)
				throw new ArgumentNullException("modelValue");

			modelType = typeof(TTableModelObject);
			tableMappingAttribute = this.GetTableMapping(modelType);

			AssertValidMapping(modelType, tableMappingAttribute);

			isUpdateOne = (object)tableModelQuery == null;

			if (isUpdateOne)
				tableTacticCommand = this.GetUpdateOneTacticCommand<TTableModelObject>(unitOfWork, modelValue, tableMappingAttribute);
			else
				tableTacticCommand = this.GetUpdateAllTacticCommand<TTableModelObject>(unitOfWork, modelValue, tableModelQuery, tableMappingAttribute);

			return tableTacticCommand;
		}

		public string GetWherePredicateFragment(TableMappingAttribute tableMappingAttribute, IUnitOfWork unitOfWork, IDictionary<string, ITacticParameter> tacticParameters, IExpression expression)
		{
			SqlExpressionVisitor expressionVisitor;
			string wherePredicate;

			if ((object)tableMappingAttribute == null)
				throw new ArgumentNullException("tableMappingAttribute");

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tacticParameters == null)
				throw new ArgumentNullException("tacticParameters");

			if ((object)expression == null)
				throw new ArgumentNullException("expression");

			expressionVisitor = new SqlExpressionVisitor(tableMappingAttribute, this, unitOfWork, tacticParameters);
			expressionVisitor.Visit(expression);
			wherePredicate = expressionVisitor.Strings.ToString();

			return wherePredicate;
		}

		private void MapOutputToReturnProcedureModel<TResultProcedureModelObject, TReturnProcedureModelObject>(ProcedureMappingAttribute procedureMappingAttribute, TReturnProcedureModelObject returnProcedureModelObject, IDictionary<string, object> output)
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new()
		{
			ParameterMappingAttribute[] parameterMappingAttributes;

			if ((object)procedureMappingAttribute == null)
				throw new ArgumentNullException("procedureMappingAttribute");

			if ((object)returnProcedureModelObject == null)
				throw new ArgumentNullException("returnProcedureModelObject");

			if ((object)output == null)
				throw new ArgumentNullException("output");

			parameterMappingAttributes = procedureMappingAttribute._OutputParameterMappingAttributes.OrderBy(pma => pma.ParameterOrdinal).ToArray();
			for (int index = 0; index < parameterMappingAttributes.Length; index++)
			{
				string parameterName;
				object parameterValue, propertyValue;

				parameterName = this.GetParameterName(parameterMappingAttributes[index].ParameterName);

				if (output.TryGetValue(parameterName, out parameterValue))
				{
					propertyValue = parameterValue.ChangeType(parameterMappingAttributes[index]._TargetProperty.PropertyType);

					if (!ReflectionFascade.Instance.SetLogicalPropertyValue(returnProcedureModelObject, parameterMappingAttributes[index]._TargetProperty.Name, propertyValue))
						throw new InvalidOperationException(string.Format("Ah snap."));
				}
			}
		}

		private void MapRecordToResultModel<TResultProcedureModelObject>(IList<ColumnMappingAttribute> columnMappingAttributes, TResultProcedureModelObject resultProcedureModelObject, IDictionary<string, object> record)
			where TResultProcedureModelObject : class, IRecordModelObject
		{
			string columnName;
			object columnValue = null, propertyValue;
			bool result;
			object[] values;

			if ((object)columnMappingAttributes == null)
				throw new ArgumentNullException("columnMappingAttributes");

			if ((object)resultProcedureModelObject == null)
				throw new ArgumentNullException("resultProcedureModelObject");

			if ((object)record == null)
				throw new ArgumentNullException("record");

			values = record.Values.ToArray();

			if (columnMappingAttributes.Count > 0)
			{
				for (int index = 0; index < columnMappingAttributes.Count; index++)
				{
					if (columnMappingAttributes[index].ColumnIsAnonymous)
					{
						if (result = ((object)values != null && values.Length > index))
							columnValue = values[index /*columnMappingAttributes[index].ColumnOrdinal*/];
					}
					else
					{
						columnName = columnMappingAttributes[index].ColumnName;
						result = record.TryGetValue(columnName, out columnValue);
					}

					if (result)
					{
						propertyValue = columnValue.ChangeType(columnMappingAttributes[index]._TargetProperty.PropertyType);

						if (!ReflectionFascade.Instance.SetLogicalPropertyValue(resultProcedureModelObject, columnMappingAttributes[index]._TargetProperty.Name, propertyValue))
							throw new InvalidOperationException(string.Format("Ah snap."));
					}
				}
			}
			else
			{
				foreach (string key in record.Keys)
				{
					if (record.TryGetValue(key, out propertyValue))
						ReflectionFascade.Instance.SetLogicalPropertyValue(resultProcedureModelObject, key, propertyValue);
				}
			}
		}

		#endregion
	}
}