/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;

using TextMetal.Middleware.Common.Utilities;
using TextMetal.Middleware.Data.Impl.FreakazoidMapper.Migrations;
using TextMetal.Middleware.Data.Impl.FreakazoidMapper.Strategies;
using TextMetal.Middleware.Data.Impl.FreakazoidMapper.Tactics;
using TextMetal.Middleware.Data.Models.Functional;
using TextMetal.Middleware.Data.Models.Tabular;
using TextMetal.Middleware.Data.UoW;
using TextMetal.Middleware.Solder.Serialization;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper
{
	public class FreakazoidModelRepository : ContextModelRepository<FreakazoidContext>
	{
		#region Constructors/Destructors

		protected FreakazoidModelRepository()
		{
			this.dataSourceTagStrategy = DataSourceTagStrategyFactory.Instance.GetDataSourceTagStrategy(this.DataSourceTag);
		}

		#endregion

		#region Fields/Constants

		private const string DATA_SOURCE_TAG_FORMAT = "{0}::DataSourceTag";
		private const string DISABLE_ENUMERATION_REENTRANT_CHECK_FORMAT = "{0}::DisableEnumerationReentrantCheck";
		private const string RESOURCE_NAME_FORMAT = "{0}.SQL.RevisionHistory.({1}).xml";
		private readonly IDataSourceTagStrategy dataSourceTagStrategy;

		#endregion

		#region Properties/Indexers/Events

		public string DataSourceTag
		{
			get
			{
				string dataSourceTag;
				string value;

				dataSourceTag = string.Format(DATA_SOURCE_TAG_FORMAT, this.GetType().Namespace);

				if (!this.AppConfigFascade.HasAppSetting(dataSourceTag))
					return null;

				value = this.AppConfigFascade.GetAppSetting<string>(dataSourceTag);

				return value;
			}
		}

		public IDataSourceTagStrategy DataSourceTagStrategy
		{
			get
			{
				return this.dataSourceTagStrategy;
			}
		}

		private bool DisableEnumerationReentrantCheck
		{
			get
			{
				string disableEnumerationReentrantCheck;
				bool value;

				disableEnumerationReentrantCheck = string.Format(DISABLE_ENUMERATION_REENTRANT_CHECK_FORMAT, this.GetType().Namespace);

				if (!this.AppConfigFascade.HasAppSetting(disableEnumerationReentrantCheck))
					return false;

				value = this.AppConfigFascade.GetAppSetting<bool>(disableEnumerationReentrantCheck);

				return value;
			}
		}

		#endregion

		#region Methods/Operators

		[Conditional("DEBUG")]
		private void __DEBUG_ProfileTacticCommand(RepositoryOperation repositoryOperation, ITacticCommand tacticCommand)
		{
			/* THIS METHOD SHOULD NOT BE DEFINED IN RELEASE/PRODUCTION BUILDS */
			StringBuilder stringBuilder;
			int i;
			const string INDENT_CHAR = "\t";

			if ((object)tacticCommand == null)
				throw new ArgumentNullException("tacticCommand");

			stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format(Environment.NewLine + "[+++ begin __DEBUG_ProfileTacticCommand(InstanceId = '{0}; | RepositoryOperation = '{1}') +++]" + Environment.NewLine, tacticCommand.InstanceId, repositoryOperation));

			stringBuilder.Append(string.Format("[TacticCommand]: ModelType = '{0}', IsNullipotent = '{6}'; ExpectedRecordsAffected = '{7}'; CommandType = '{1}'; CommandText = '{2}'; CommandPrepare = '{3}'; CommandTimeout = '{4}'; CommandBehavior = '{5}'.",
				string.Join("|", tacticCommand.GetModelTypes().Select(t => t.FullName).ToArray()),
				tacticCommand.CommandType,
				tacticCommand.CommandText,
				tacticCommand.CommandPrepare,
				tacticCommand.CommandTimeout,
				tacticCommand.CommandBehavior,
				tacticCommand.IsNullipotent,
				tacticCommand.ExpectedRecordsAffected));

			i = 0;
			foreach (ITacticParameter tacticParameter in tacticCommand.TacticParameters)
			{
				stringBuilder.Append(string.Format(Environment.NewLine + INDENT_CHAR + "[TacticParameter{0:00}]: ParameterDirection = '{1}'; ParameterName = '{2}'; ParameterNullable = '{3}'; ParameterPrecision = '{4}'; ParameterScale = '{5}'; ParameterSize = '{6}'; ParameterDbType = '{7}'; ParameterValue = '{8}'.",
					i++,
					tacticParameter.ParameterDirection,
					tacticParameter.ParameterName,
					tacticParameter.ParameterNullable,
					tacticParameter.ParameterPrecision,
					tacticParameter.ParameterScale,
					tacticParameter.ParameterSize,
					tacticParameter.ParameterDbType,
					tacticParameter.ParameterValue.SafeToString(null, "<null>")));
			}

			stringBuilder.Append(string.Format(Environment.NewLine + "[+++ end __DEBUG_ProfileTacticCommand({0}) +++]" + Environment.NewLine, repositoryOperation));

			Debug.WriteLine(stringBuilder.ToString());
			stringBuilder.Clear();
		}

		public override bool Discard<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject)
		{
			bool wasNew;
			IEnumerable<IDictionary<string, object>> records;
			IDictionary<string, object> record;

			IEnumerable<IDbDataParameter> dbDataParameters;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			wasNew = tableModelObject.IsNew;
			tableModelObject.Mark();

			if (wasNew)
				return true;

			using (AmbientUnitOfWorkAwareContextWrapper<FreakazoidContext> wrapper = this.GetContext(unitOfWork))
			{
				ITableTacticCommand<TTableModelObject> tableTacticCommand;
				int actualRecordsAffected;

				this.OnPreDeleteTableModel<TTableModelObject>(unitOfWork, tableModelObject);

				tableTacticCommand = this.DataSourceTagStrategy.GetDeleteTacticCommand<TTableModelObject>(unitOfWork, tableModelObject, null);

				this.__DEBUG_ProfileTacticCommand(RepositoryOperation.Discard, tableTacticCommand);

				// get native parameters
				dbDataParameters = tableTacticCommand.GetDbDataParameters(unitOfWork);

				records = unitOfWork.ExecuteDictionary(tableTacticCommand.CommandType, tableTacticCommand.CommandText, dbDataParameters, out actualRecordsAffected);

				if (actualRecordsAffected <= tableTacticCommand.ExpectedRecordsAffected)
				{
					// concurrency or nullipotency failure
					unitOfWork.Divergent();

					this.OnSaveConflictTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					//throw new InvalidOperationException(string.Format("Data concurrency or nullipotency failure occurred during table model save; actual records affected '{0}' was less than or equal to the expected records affected '{1}'.", TableTacticCommand.ExpectedRecordsAffected, actualRecordsAffected));
					return false;
				}

				if ((object)records == null)
					throw new InvalidOperationException(string.Format("Records were invalid."));

				record = records.ToList().SingleOrDefault();

				// map to table model from record (destination, source)
				if ((object)record != null)
					tableTacticCommand.RecordToTableModelMappingCallback(tableModelObject, record);

				this.OnPostDeleteTableModel<TTableModelObject>(unitOfWork, tableModelObject);

				tableModelObject.IsNew = false;

				return true;
			}
		}

		public override TReturnProcedureModelObject Execute<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, TCallProcedureModelObject callProcedureModelObject)
		{
			TReturnProcedureModelObject returnProcedureModelObject;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)callProcedureModelObject == null)
				throw new ArgumentNullException("callProcedureModelObject");

			using (AmbientUnitOfWorkAwareContextWrapper<FreakazoidContext> wrapper = this.GetContext(unitOfWork))
			{
				IProcedureTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject> procedureTacticCommand;

				this.OnPreExecuteProcedureModel<TCallProcedureModelObject>(unitOfWork, callProcedureModelObject);

				procedureTacticCommand = this.DataSourceTagStrategy.GetExecuteTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(unitOfWork, callProcedureModelObject);

				returnProcedureModelObject = new TReturnProcedureModelObject();
				returnProcedureModelObject.Resultsets = this.GetProcedureResultsetsLazy<TCallProcedureModelObject, DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject, TReturnProcedureModelObject>(unitOfWork, procedureTacticCommand, callProcedureModelObject, returnProcedureModelObject);

				this.OnPostExecuteProcedureModel<TResultProcedureModelObject, TReturnProcedureModelObject>(unitOfWork, returnProcedureModelObject);

				return returnProcedureModelObject; // output parameters not propagated until results are fully enumerated
			}
		}

		public override bool Fill<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject)
		{
			IEnumerable<IDictionary<string, object>> records;
			IDictionary<string, object> record;

			IEnumerable<IDbDataParameter> dbDataParameters;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			using (AmbientUnitOfWorkAwareContextWrapper<FreakazoidContext> wrapper = this.GetContext(unitOfWork))
			{
				ITableTacticCommand<TTableModelObject> tableTacticCommand;
				int actualRecordsAffected;

				tableTacticCommand = this.DataSourceTagStrategy.GetSelectTacticCommand<TTableModelObject>(unitOfWork, tableModelObject, null);

				this.__DEBUG_ProfileTacticCommand(RepositoryOperation.Fill, tableTacticCommand);

				// get native parameters
				dbDataParameters = tableTacticCommand.GetDbDataParameters(unitOfWork);

				records = unitOfWork.ExecuteDictionary(tableTacticCommand.CommandType, tableTacticCommand.CommandText, dbDataParameters, out actualRecordsAffected);

				if (actualRecordsAffected != tableTacticCommand.ExpectedRecordsAffected)
				{
					// concurrency or nullipotency failure
					unitOfWork.Divergent();

					throw new InvalidOperationException(string.Format("Data concurrency or nullipotency failure occurred during table model fill; actual records affected '{0}' did not equal expected records affected '{1}'.", tableTacticCommand.ExpectedRecordsAffected, actualRecordsAffected));
				}

				if ((object)records == null)
					throw new InvalidOperationException(string.Format("Records were invalid."));

				record = records.ToList().SingleOrDefault();

				if ((object)record == null)
					return false;

				// map to table model from record (destination, source)
				tableTacticCommand.RecordToTableModelMappingCallback(tableModelObject, record);

				this.OnSelectTableModel(unitOfWork, tableModelObject);

				return true;
			}
		}

		public override IEnumerable<TTableModelObject> Find<TTableModelObject>(IUnitOfWork unitOfWork, ITableModelQuery tableModelQuery)
		{
			TTableModelObject dummyTableModel;
			IEnumerable<TTableModelObject> tableModelObjects;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelQuery == null)
				throw new ArgumentNullException("tableModelQuery");

			dummyTableModel = new TTableModelObject();

			using (AmbientUnitOfWorkAwareContextWrapper<FreakazoidContext> wrapper = this.GetContext(unitOfWork))
			{
				ITableTacticCommand<TTableModelObject> tableTacticCommand;

				this.OnPreSelectionTableModel<TTableModelObject>(unitOfWork);

				tableTacticCommand = this.DataSourceTagStrategy.GetSelectTacticCommand<TTableModelObject>(unitOfWork, dummyTableModel, tableModelQuery);

				tableModelObjects = this.GetTableResultsLazy<TTableModelObject>(unitOfWork, tableTacticCommand);

				this.OnPostSelectionTableModel<TTableModelObject>(unitOfWork);

				return tableModelObjects; // output parameters not propagated until results are fully enumerated
			}
		}

		protected override FreakazoidContext GetContext(Type contextType, DbConnection dbConnection, DbTransaction dbTransaction)
		{
			return new FreakazoidContext();
		}

		/// <summary>
		/// NOTE: This code is re-entrant if the results enumeration is re-started, for example:
		/// returnProcedureModelObject.Results.First(); // causes an 'abandoned' enumerator
		/// returnProcedureModelObject.Results.Last(); // causes a full enumeration
		/// To prevent this:
		/// var results = returnProcedureModelObject.Results.ToArray(); // causes a full enumeration
		/// results.First(); // in-memory
		/// results.Last(); // in-memory
		/// </summary>
		/// <typeparam name="TCallProcedureModelObject"> </typeparam>
		/// <typeparam name="TResultsetModelObject"> </typeparam>
		/// <typeparam name="TResultProcedureModelObject"> </typeparam>
		/// <typeparam name="TReturnProcedureModelObject"> </typeparam>
		/// <param name="unitOfWork"> </param>
		/// <param name="procedureTacticCommand"> </param>
		/// <param name="callProcedureModelObject"> </param>
		/// <param name="returnProcedureModelObject"> </param>
		/// <returns> </returns>
		private IEnumerable<TResultsetModelObject> GetProcedureResultsetsLazy<TCallProcedureModelObject, TResultsetModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, IProcedureTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject> procedureTacticCommand, TCallProcedureModelObject callProcedureModelObject, TReturnProcedureModelObject returnProcedureModelObject)
			where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
			where TResultsetModelObject : class, IResultsetModelObject<TResultProcedureModelObject>, new()
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new()
		{
			IEnumerable<IDictionary<string, object>> records;
			TResultsetModelObject resultsetModelObject;

			int actualRecordsAffected = int.MaxValue;
			IDictionary<string, object> output;

			IEnumerable<IDbDataParameter> dbDataParameters;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)procedureTacticCommand == null)
				throw new ArgumentNullException("procedureTacticCommand");

			if ((object)callProcedureModelObject == null)
				throw new ArgumentNullException("callProcedureModelObject");

			if ((object)returnProcedureModelObject == null)
				throw new ArgumentNullException("returnProcedureModelObject");

			this.OnPreResultsProcedureModel<TCallProcedureModelObject>(unitOfWork, callProcedureModelObject);

			this.__DEBUG_ProfileTacticCommand(RepositoryOperation.Execute, procedureTacticCommand);

			// starting a new delayed-execution enumeration
			procedureTacticCommand.EnterEnumeration(this.DisableEnumerationReentrantCheck);

			// get native parameters
			dbDataParameters = procedureTacticCommand.GetDbDataParameters(unitOfWork);

			// enumerator overload usage
			records = unitOfWork.ExecuteDictionary(procedureTacticCommand.CommandType, procedureTacticCommand.CommandText, dbDataParameters, (ra) => actualRecordsAffected = ra);

			if ((object)records == null)
				throw new InvalidOperationException(string.Format("Records were invalid."));

			// DOES NOT FORCE EXECUTION AGAINST STORE
			IEnumerable<IGrouping<int, IDictionary<string, object>>> groups;

			groups = records.GroupBy(record =>
									{
										int resultsetIndex = (int)record[AdoNetFascade.ResultsetIndexRecordKey];
										record.Remove(AdoNetFascade.ResultsetIndexRecordKey);
										return resultsetIndex;
									});

			foreach (IGrouping<int, IDictionary<string, object>> group in groups)
			{
				resultsetModelObject = new TResultsetModelObject();
				resultsetModelObject.Index = group.Key;
				resultsetModelObject.Results = this.GetProcedureResultsLazy<TCallProcedureModelObject, TResultsetModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(unitOfWork, procedureTacticCommand, resultsetModelObject, group);

				this.OnResultsetProcedureModel<TResultsetModelObject, TResultProcedureModelObject>(unitOfWork, resultsetModelObject);

				yield return resultsetModelObject; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
			}

			// NOTE: execution will not reach this point until enumeration is fully completed
			procedureTacticCommand.LeaveEnumeration(this.DisableEnumerationReentrantCheck);

			// ***** SUPER special case for enumerator *****
			if (actualRecordsAffected != procedureTacticCommand.ExpectedRecordsAffected)
			{
				// concurrency or nullipotency failure
				unitOfWork.Divergent();

				throw new InvalidOperationException(string.Format("Data concurrency or nullipotency failure occurred during result model execute; actual records affected '{0}' did not equal expected records affected '{1}'.", procedureTacticCommand.ExpectedRecordsAffected, actualRecordsAffected));
			}

			// right now for procedures only
			output = dbDataParameters.GetOutputAsRecord();

			// map to return model from output (destination, source)
			procedureTacticCommand.OutputToReturnProcedureModelMappingCallback(returnProcedureModelObject, output);

			// alert that we are done enumerating the lazy river...
			this.OnPostResultsProcedureModel<TResultProcedureModelObject, TReturnProcedureModelObject>(unitOfWork, returnProcedureModelObject);
		}

		/// <summary>
		/// NOTE: This code is re-entrant if the results enumeration is re-started, for example:
		/// returnProcedureModelObject.Results.First(); // causes an 'abandoned' enumerator
		/// returnProcedureModelObject.Results.Last(); // causes a full enumeration
		/// To prevent this:
		/// var results = returnProcedureModelObject.Results.ToArray(); // causes a full enumeration
		/// results.First(); // in-memory
		/// results.Last(); // in-memory
		/// </summary>
		/// <typeparam name="TCallProcedureModelObject"> </typeparam>
		/// <typeparam name="TResultsetModelObject"> </typeparam>
		/// <typeparam name="TResultProcedureModelObject"> </typeparam>
		/// <typeparam name="TReturnProcedureModelObject"> </typeparam>
		/// <param name="unitOfWork"> </param>
		/// <param name="procedureTacticCommand"> </param>
		/// <param name="resultsetModelObject"> </param>
		/// <param name="group"> </param>
		/// <returns> </returns>
		private IEnumerable<TResultProcedureModelObject> GetProcedureResultsLazy<TCallProcedureModelObject, TResultsetModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, IProcedureTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject> procedureTacticCommand, TResultsetModelObject resultsetModelObject, IGrouping<int, IDictionary<string, object>> group)
			where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
			where TResultsetModelObject : class, IResultsetModelObject<TResultProcedureModelObject>, new()
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new()
		{
			TResultProcedureModelObject resultProcedureModelObject;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)resultsetModelObject == null)
				throw new ArgumentNullException("resultsetModelObject");

			if ((object)group == null)
				throw new ArgumentNullException("group");

			// DOES NOT FORCE EXECUTION AGAINST STORE
			foreach (IDictionary<string, object> record in group)
			{
				resultProcedureModelObject = new TResultProcedureModelObject();

				// map to result model from record (destination, source)
				procedureTacticCommand.RecordToResultModelMappingCallback(resultProcedureModelObject, record);

				this.OnResultProcedureModel<TResultProcedureModelObject>(unitOfWork, resultProcedureModelObject);

				yield return resultProcedureModelObject; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
			}
		}

		/// <summary>
		/// NOTE: This code is re-entrant if the results enumeration is re-started, for example:
		/// results.First(); // causes an 'abandoned' enumerator
		/// results.Last(); // causes a full enumeration
		/// To prevent this:
		/// var results2 = results.ToArray(); // causes a full enumeration
		/// results2.First(); // in-memory
		/// results2.Last(); // in-memory
		/// </summary>
		/// <typeparam name="TTableModelObject"> </typeparam>
		/// <param name="unitOfWork"> </param>
		/// <param name="tableTacticCommand"> </param>
		/// <returns> </returns>
		private IEnumerable<TTableModelObject> GetTableResultsLazy<TTableModelObject>(IUnitOfWork unitOfWork, ITableTacticCommand<TTableModelObject> tableTacticCommand)
			where TTableModelObject : class, ITableModelObject, new()
		{
			IEnumerable<IDictionary<string, object>> records;
			TTableModelObject tableModelObject;

			int actualRecordsAffected = int.MaxValue;

			IEnumerable<IDbDataParameter> dbDataParameters;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableTacticCommand == null)
				throw new ArgumentNullException("tableTacticCommand");

			this.OnPreSelectionTableModel<TTableModelObject>(unitOfWork);

			this.__DEBUG_ProfileTacticCommand(RepositoryOperation.Find, tableTacticCommand);

			// starting a new delayed-execution enumeration
			tableTacticCommand.EnterEnumeration(this.DisableEnumerationReentrantCheck);

			// get native parameters
			dbDataParameters = tableTacticCommand.GetDbDataParameters(unitOfWork);

			// enumerator overload usage
			records = unitOfWork.ExecuteDictionary(tableTacticCommand.CommandType, tableTacticCommand.CommandText, dbDataParameters, (ra) => actualRecordsAffected = ra);

			if ((object)records == null)
				throw new InvalidOperationException(string.Format("Records were invalid."));

			// DOES NOT FORCE EXECUTION AGAINST STORE
			foreach (IDictionary<string, object> record in records)
			{
				tableModelObject = new TTableModelObject();

				// map to table model from record (destination, source)
				tableTacticCommand.RecordToTableModelMappingCallback(tableModelObject, record);

				this.OnSelectTableModel<TTableModelObject>(unitOfWork, tableModelObject);

				yield return tableModelObject; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
			}

			// NOTE: execution will not reach this point until enumeration is fully completed
			tableTacticCommand.LeaveEnumeration(this.DisableEnumerationReentrantCheck);

			// ***** SUPER special case for enumerator *****
			if (actualRecordsAffected != tableTacticCommand.ExpectedRecordsAffected)
			{
				// concurrency or nullipotency failure
				unitOfWork.Divergent();

				throw new InvalidOperationException(string.Format("Data concurrency or nullipotency failure occurred during table model load; actual records affected '{0}' did not equal expected records affected '{1}'.", tableTacticCommand.ExpectedRecordsAffected, actualRecordsAffected));
			}

			// alert that we are done enumerating the lazy river...
			this.OnPostSelectionTableModel<TTableModelObject>(unitOfWork);
		}

		public override TTableModelObject Load<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject prototypeTableModel)
		{
			TTableModelObject tableModelObject;
			IEnumerable<IDictionary<string, object>> records;
			IDictionary<string, object> record;

			IEnumerable<IDbDataParameter> dbDataParameters;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)prototypeTableModel == null)
				throw new ArgumentNullException("prototypeTableModel");

			using (AmbientUnitOfWorkAwareContextWrapper<FreakazoidContext> wrapper = this.GetContext(unitOfWork))
			{
				ITableTacticCommand<TTableModelObject> tableTacticCommand;
				int actualRecordsAffected;

				tableTacticCommand = this.DataSourceTagStrategy.GetSelectTacticCommand<TTableModelObject>(unitOfWork, prototypeTableModel, null);

				this.__DEBUG_ProfileTacticCommand(RepositoryOperation.Load, tableTacticCommand);

				// get native parameters
				dbDataParameters = tableTacticCommand.GetDbDataParameters(unitOfWork);

				records = unitOfWork.ExecuteDictionary(tableTacticCommand.CommandType, tableTacticCommand.CommandText, dbDataParameters, out actualRecordsAffected);

				if (actualRecordsAffected != tableTacticCommand.ExpectedRecordsAffected)
				{
					// concurrency or nullipotency failure
					unitOfWork.Divergent();

					throw new InvalidOperationException(string.Format("Data concurrency or nullipotency failure occurred during table model load; actual records affected '{0}' did not equal expected records affected '{1}'.", tableTacticCommand.ExpectedRecordsAffected, actualRecordsAffected));
				}

				if ((object)records == null)
					throw new InvalidOperationException(string.Format("Records were invalid."));

				record = records.ToList().SingleOrDefault();

				if ((object)record == null)
					return null;

				tableModelObject = new TTableModelObject();

				// map to table model from record (destination, source)
				tableTacticCommand.RecordToTableModelMappingCallback(tableModelObject, record);

				this.OnSelectTableModel(unitOfWork, tableModelObject);

				return tableModelObject;
			}
		}

		protected override bool OnCreateNativeDatabaseFile(string databaseFilePath)
		{
			string resourceName;
			DatabaseHistory databaseHistory;

			if (!this.DataSourceTagStrategy.CanCreateNativeDatabaseFile)
				return false;

			this.DataSourceTagStrategy.CreateNativeDatabaseFile(databaseFilePath);

			resourceName = string.Format(RESOURCE_NAME_FORMAT, this.GetType().Namespace, this.DataSourceTag.SafeToString().ToLower());

			if (!XmlSerializationStrategy.Instance.TryGetFromAssemblyResource<DatabaseHistory>(this.GetType(), resourceName, out databaseHistory))
				throw new InvalidOperationException(string.Format("Unable to deserialize instance of '{0}' from the manifest resource name '{1}' in the assembly '{2}'.", typeof(DatabaseHistory).FullName, resourceName, this.GetType().Assembly.FullName));

			using (IUnitOfWork unitOfWork = this.GetUnitOfWork(true))
			{
				databaseHistory.PerformSchemaUpgrade(unitOfWork);

				unitOfWork.Complete();
			}

			return true;
		}

		public override bool Save<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject)
		{
			bool wasNew;
			IEnumerable<IDictionary<string, object>> records;
			IDictionary<string, object> record;

			IEnumerable<IDbDataParameter> dbDataParameters;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			wasNew = tableModelObject.IsNew;
			tableModelObject.Mark();

			using (AmbientUnitOfWorkAwareContextWrapper<FreakazoidContext> wrapper = this.GetContext(unitOfWork))
			{
				ITableTacticCommand<TTableModelObject> tableTacticCommand;
				int actualRecordsAffected;

				if (wasNew)
				{
					this.OnPreInsertTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					tableTacticCommand = this.DataSourceTagStrategy.GetInsertTacticCommand<TTableModelObject>(unitOfWork, tableModelObject, null);
				}
				else
				{
					this.OnPreUpdateTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					tableTacticCommand = this.DataSourceTagStrategy.GetUpdateTacticCommand<TTableModelObject>(unitOfWork, tableModelObject, null);
				}

				this.__DEBUG_ProfileTacticCommand(RepositoryOperation.Save, tableTacticCommand);

				// get native parameters
				dbDataParameters = tableTacticCommand.GetDbDataParameters(unitOfWork);

				records = unitOfWork.ExecuteDictionary(tableTacticCommand.CommandType, tableTacticCommand.CommandText, dbDataParameters, out actualRecordsAffected);

				if (actualRecordsAffected <= tableTacticCommand.ExpectedRecordsAffected)
				{
					// concurrency or nullipotency failure
					unitOfWork.Divergent();

					this.OnSaveConflictTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					//throw new InvalidOperationException(string.Format("Data concurrency or nullipotency failure occurred during table model save; actual records affected '{0}' was less than or equal to the expected records affected '{1}'.", TableTacticCommand.ExpectedRecordsAffected, actualRecordsAffected));
					return false;
				}

				if ((object)records == null)
					throw new InvalidOperationException(string.Format("Records were invalid."));

				record = records.ToList().SingleOrDefault();

				// map to table model from record (destination, source)
				if ((object)record != null)
					tableTacticCommand.RecordToTableModelMappingCallback(tableModelObject, record);

				// ***------------------------***

				if (wasNew && !tableTacticCommand.UseBatchScopeIdentificationSemantics)
				{
					// this is optional
					tableTacticCommand = this.DataSourceTagStrategy.GetIdentifyTacticCommand<TTableModelObject>(unitOfWork);

					this.__DEBUG_ProfileTacticCommand(RepositoryOperation.Identify, tableTacticCommand);

					records = unitOfWork.ExecuteDictionary(tableTacticCommand.CommandType, tableTacticCommand.CommandText, tableTacticCommand.GetDbDataParameters(unitOfWork), out actualRecordsAffected);

					if (actualRecordsAffected != tableTacticCommand.ExpectedRecordsAffected)
					{
						// concurrency or nullipotency failure
						unitOfWork.Divergent();

						throw new InvalidOperationException(string.Format("Data concurrency or nullipotency failure occurred during table model fill; actual records affected '{0}' did not equal expected records affected '{1}'.", tableTacticCommand.ExpectedRecordsAffected, actualRecordsAffected));
					}

					if ((object)records == null)
						throw new InvalidOperationException(string.Format("Records were invalid."));

					record = records.ToList().SingleOrDefault();

					if ((object)record == null)
						return false;

					// map to table model from record (destination, source)
					tableTacticCommand.RecordToTableModelMappingCallback(tableModelObject, record);
				}

				// ***------------------------***

				if (wasNew)
					this.OnPostInsertTableModel<TTableModelObject>(unitOfWork, tableModelObject);
				else
					this.OnPostUpdateTableModel<TTableModelObject>(unitOfWork, tableModelObject);

				return true;
			}
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		protected enum RepositoryOperation
		{
			None = 0,
			Discard,
			Execute,
			Fill,
			Find,
			Load,
			Save,
			Identify,
			Query,
		}

		#endregion
	}
}