/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using TextMetal.Middleware.Datazoid.Models.Functional;
using TextMetal.Middleware.Datazoid.Models.Tabular;
using TextMetal.Middleware.Datazoid.Repositories.Impl.Migrations;
using TextMetal.Middleware.Datazoid.Repositories.Impl.Strategies;
using TextMetal.Middleware.Datazoid.Repositories.Impl.Tactics;
using TextMetal.Middleware.Datazoid.UoW;
using TextMetal.Middleware.Solder;
using TextMetal.Middleware.Solder.Serialization;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl
{
	public class DzModelRepository : ModelRepository<DzContext>
	{
		#region Constructors/Destructors

		protected DzModelRepository()
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
		private static void __DEBUG_ProfileTacticCommand(RepositoryOperation repositoryOperation, ITacticCommand tacticCommand)
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

			OnlyWhen._PROFILE_ThenPrint(stringBuilder.ToString());
			stringBuilder.Clear();
		}

		public override bool Discard<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject)
		{
			bool wasNew;
			IEnumerable<IResultset> resultsets;
			IResultset resultset;
			IEnumerable<IRecord> records;
			IRecord record;
			IRecord output;
			int? recordsAffected = null;

			IEnumerable<IDbDataParameter> dbDataParameters;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			wasNew = tableModelObject.IsNew;
			tableModelObject.Mark();

			if (wasNew)
				return true;

			using (DzContext context = new DzContext())
			{
				ITableTacticCommand<TTableModelObject> tableTacticCommand;

				this.OnPreDeleteTableModel<TTableModelObject>(unitOfWork, tableModelObject);

				tableTacticCommand = this.DataSourceTagStrategy.GetDeleteTacticCommand<TTableModelObject>(unitOfWork, tableModelObject, null);

				__DEBUG_ProfileTacticCommand(RepositoryOperation.Discard, tableTacticCommand);

				// get native parameters
				dbDataParameters = tableTacticCommand.GetDbDataParameters(unitOfWork);

				records = unitOfWork.ExecuteRecords(tableTacticCommand.CommandType, tableTacticCommand.CommandText, dbDataParameters, (ra) => recordsAffected = ra);

				if ((object)records == null)
					throw new InvalidOperationException(string.Format("Records were invalid."));

				record = records.SingleOrDefault();

				// map to table model from record (destination, source)
				if ((object)record != null)
					tableTacticCommand.RecordToTableModelMappingCallback(tableModelObject, 0, record);

				if ((object)recordsAffected == null)
					throw new InvalidOperationException(string.Format("Records affected was invalid."));

				if (recordsAffected <= tableTacticCommand.ExpectedRecordsAffected)
				{
					// concurrency or nullipotency failure
					unitOfWork.Divergent();

					this.OnSaveConflictTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					//throw new InvalidOperationException(string.Format("Data concurrency or nullipotency failure occurred during table model save; actual records affected '{0}' was less than or equal to the expected records affected '{1}'.", TableTacticCommand.ExpectedRecordsAffected, recordsAffected));
					return false;
				}

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

			using (DzContext context = new DzContext())
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
			IEnumerable<IResultset> resultsets;
			IResultset resultset;
			IEnumerable<IRecord> records;
			IRecord record;
			IRecord output;
			int? recordsAffected = null;

			IEnumerable<IDbDataParameter> dbDataParameters;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			using (DzContext context = new DzContext())
			{
				ITableTacticCommand<TTableModelObject> tableTacticCommand;

				tableTacticCommand = this.DataSourceTagStrategy.GetSelectTacticCommand<TTableModelObject>(unitOfWork, tableModelObject, null);

				__DEBUG_ProfileTacticCommand(RepositoryOperation.Fill, tableTacticCommand);

				// get native parameters
				dbDataParameters = tableTacticCommand.GetDbDataParameters(unitOfWork);

				records = unitOfWork.ExecuteRecords(tableTacticCommand.CommandType, tableTacticCommand.CommandText, dbDataParameters, (ra) => recordsAffected = ra);

				if ((object)records == null)
					throw new InvalidOperationException(string.Format("Records were invalid."));

				record = records.SingleOrDefault();

				if ((object)record == null)
					return false;

				// map to table model from record (destination, source)
				tableTacticCommand.RecordToTableModelMappingCallback(tableModelObject, 0, record);

				if ((object)recordsAffected == null)
					throw new InvalidOperationException(string.Format("Records affected was invalid."));

				if (recordsAffected != tableTacticCommand.ExpectedRecordsAffected)
				{
					// concurrency or nullipotency failure
					unitOfWork.Divergent();

					throw new InvalidOperationException(string.Format("Data concurrency or nullipotency failure occurred during table model fill; actual records affected '{0}' did not equal expected records affected '{1}'.", tableTacticCommand.ExpectedRecordsAffected, recordsAffected));
				}

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

			using (DzContext context = new DzContext())
			{
				ITableTacticCommand<TTableModelObject> tableTacticCommand;

				this.OnPreSelectionTableModel<TTableModelObject>(unitOfWork);

				tableTacticCommand = this.DataSourceTagStrategy.GetSelectTacticCommand<TTableModelObject>(unitOfWork, dummyTableModel, tableModelQuery);

				tableModelObjects = this.GetTableResultsLazy<TTableModelObject>(unitOfWork, tableTacticCommand);

				this.OnPostSelectionTableModel<TTableModelObject>(unitOfWork);

				return tableModelObjects; // output parameters not propagated until results are fully enumerated
			}
		}

		/// <summary>
		/// NOTE: This code is re-entrant if the results enumeration is re-started, for example:
		/// resultset.Records.First(); // causes an 'abandoned' enumerator
		/// resultset.Records.Last(); // causes a full enumeration
		/// To prevent this:
		/// var records = resultset.Records.ToArray(); // causes a full enumeration
		/// records.First(); // in-memory
		/// records.Last(); // in-memory
		/// </summary>
		/// <typeparam name="TCallProcedureModelObject"> </typeparam>
		/// <typeparam name="TResultsetModelObject"> </typeparam>
		/// <typeparam name="TResultProcedureModelObject"> </typeparam>
		/// <typeparam name="TReturnProcedureModelObject"> </typeparam>
		/// <param name="unitOfWork"> </param>
		/// <param name="procedureTacticCommand"> </param>
		/// <param name="resultsetModelObject"> </param>
		/// <param name="resultset"> </param>
		/// <returns> </returns>
		private IEnumerable<TResultProcedureModelObject> GetProcedureRecordsLazy<TCallProcedureModelObject, TResultsetModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, IProcedureTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject> procedureTacticCommand, TResultsetModelObject resultsetModelObject, IResultset resultset)
			where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
			where TResultsetModelObject : class, IResultsetModelObject<TResultProcedureModelObject>, new()
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new()
		{
			TResultProcedureModelObject resultProcedureModelObject;
			IEnumerable<IRecord> records;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)resultsetModelObject == null)
				throw new ArgumentNullException("resultsetModelObject");

			if ((object)resultset == null)
				throw new ArgumentNullException("resultset");

			records = resultset.Records;

			if ((object)records == null)
				throw new InvalidOperationException(string.Format("Records were invalid."));

			// DOES NOT FORCE EXECUTION AGAINST STORE
			foreach (IRecord record in records)
			{
				resultProcedureModelObject = new TResultProcedureModelObject();

				// map to result model from record (destination, source)
				procedureTacticCommand.RecordToResultModelMappingCallback(resultProcedureModelObject, resultset.Index, record);

				this.OnResultProcedureModel<TResultProcedureModelObject>(unitOfWork, resultProcedureModelObject);

				yield return resultProcedureModelObject; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
			}
		}

		/// <summary>
		/// NOTE: This code is re-entrant if the results enumeration is re-started, for example:
		/// returnProcedureModelObject.Resultsets.First(); // causes an 'abandoned' enumerator
		/// returnProcedureModelObject.Resultsets.Last(); // causes a full enumeration
		/// To prevent this:
		/// var resultsets = returnProcedureModelObject.Resultsets.ToArray(); // causes a full enumeration
		/// resultsets.First(); // in-memory
		/// resultsets.Last(); // in-memory
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
			TResultsetModelObject resultsetModelObject;
			IEnumerable<IResultset> resultsets;
			IEnumerable<IRecord> records;
			IRecord record;
			IRecord output;

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

			__DEBUG_ProfileTacticCommand(RepositoryOperation.Execute, procedureTacticCommand);

			// starting a new delayed-execution enumeration
			procedureTacticCommand.EnterEnumeration(this.DisableEnumerationReentrantCheck);

			// get native parameters
			dbDataParameters = procedureTacticCommand.GetDbDataParameters(unitOfWork);

			resultsets = unitOfWork.ExecuteResultsets(procedureTacticCommand.CommandType, procedureTacticCommand.CommandText, dbDataParameters);

			if ((object)resultsets == null)
				throw new InvalidOperationException(string.Format("Resultsets were invalid."));

			foreach (IResultset resultset in resultsets)
			{
				resultsetModelObject = new TResultsetModelObject();
				resultsetModelObject.Index = resultset.Index;
				resultsetModelObject.Records = this.GetProcedureRecordsLazy<TCallProcedureModelObject, TResultsetModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(unitOfWork, procedureTacticCommand, resultsetModelObject, resultset);

				this.OnResultsetProcedureModel<TResultsetModelObject, TResultProcedureModelObject>(unitOfWork, resultsetModelObject);

				yield return resultsetModelObject; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
			}

			// NOTE: execution will not reach this point until enumeration is fully completed
			procedureTacticCommand.LeaveEnumeration(this.DisableEnumerationReentrantCheck);

			// cannot validate records affected in the general case with procedures

			// right now for procedures only
			output = dbDataParameters.GetOutputAsRecord();

			// map to return model from output (destination, source)
			procedureTacticCommand.OutputToReturnProcedureModelMappingCallback(returnProcedureModelObject, output);

			// alert that we are done enumerating the lazy river...
			this.OnPostResultsProcedureModel<TResultProcedureModelObject, TReturnProcedureModelObject>(unitOfWork, returnProcedureModelObject);
		}

		/// <summary>
		/// NOTE: This code is re-entrant if the results enumeration is re-started, for example:
		/// records.First(); // causes an 'abandoned' enumerator
		/// records.Last(); // causes a full enumeration
		/// To prevent this:
		/// var records2 = records.ToArray(); // causes a full enumeration
		/// records2.First(); // in-memory
		/// records2.Last(); // in-memory
		/// </summary>
		/// <typeparam name="TTableModelObject"> </typeparam>
		/// <param name="unitOfWork"> </param>
		/// <param name="tableTacticCommand"> </param>
		/// <returns> </returns>
		private IEnumerable<TTableModelObject> GetTableResultsLazy<TTableModelObject>(IUnitOfWork unitOfWork, ITableTacticCommand<TTableModelObject> tableTacticCommand)
			where TTableModelObject : class, ITableModelObject, new()
		{
			TTableModelObject tableModelObject;
			IEnumerable<IResultset> resultsets;
			IResultset resultset;
			IEnumerable<IRecord> records;
			IRecord output;
			int? recordsAffected = null;

			IEnumerable<IDbDataParameter> dbDataParameters;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableTacticCommand == null)
				throw new ArgumentNullException("tableTacticCommand");

			this.OnPreSelectionTableModel<TTableModelObject>(unitOfWork);

			__DEBUG_ProfileTacticCommand(RepositoryOperation.Find, tableTacticCommand);

			// starting a new delayed-execution enumeration
			tableTacticCommand.EnterEnumeration(this.DisableEnumerationReentrantCheck);

			// get native parameters
			dbDataParameters = tableTacticCommand.GetDbDataParameters(unitOfWork);

			// enumerator overload usage
			records = unitOfWork.ExecuteRecords(tableTacticCommand.CommandType, tableTacticCommand.CommandText, dbDataParameters, (ra) => recordsAffected = ra);

			if ((object)records == null)
				throw new InvalidOperationException(string.Format("Records were invalid."));

			foreach (IRecord record in records)
			{
				tableModelObject = new TTableModelObject();

				// map to table model from record (destination, source)
				tableTacticCommand.RecordToTableModelMappingCallback(tableModelObject, 0, record);

				this.OnSelectTableModel<TTableModelObject>(unitOfWork, tableModelObject);

				yield return tableModelObject; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
			}

			if ((object)recordsAffected == null)
				throw new InvalidOperationException(string.Format("Records affected was invalid."));

			if (recordsAffected != tableTacticCommand.ExpectedRecordsAffected)
			{
				// concurrency or nullipotency failure
				unitOfWork.Divergent();

				throw new InvalidOperationException(string.Format("Data concurrency or nullipotency failure occurred during table model load; actual records affected '{0}' did not equal expected records affected '{1}'.", tableTacticCommand.ExpectedRecordsAffected, recordsAffected));
			}

			// NOTE: execution will not reach this point until enumeration is fully completed
			tableTacticCommand.LeaveEnumeration(this.DisableEnumerationReentrantCheck);

			// alert that we are done enumerating the lazy river...
			this.OnPostSelectionTableModel<TTableModelObject>(unitOfWork);
		}

		public override TTableModelObject Load<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject prototypeTableModel)
		{
			TTableModelObject tableModelObject;
			IEnumerable<IResultset> resultsets;
			IResultset resultset;
			IEnumerable<IRecord> records;
			IRecord record;
			IRecord output;
			int? recordsAffected = null;

			IEnumerable<IDbDataParameter> dbDataParameters;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)prototypeTableModel == null)
				throw new ArgumentNullException("prototypeTableModel");

			using (DzContext context = new DzContext())
			{
				ITableTacticCommand<TTableModelObject> tableTacticCommand;

				tableTacticCommand = this.DataSourceTagStrategy.GetSelectTacticCommand<TTableModelObject>(unitOfWork, prototypeTableModel, null);

				__DEBUG_ProfileTacticCommand(RepositoryOperation.Load, tableTacticCommand);

				// get native parameters
				dbDataParameters = tableTacticCommand.GetDbDataParameters(unitOfWork);

				records = unitOfWork.ExecuteRecords(tableTacticCommand.CommandType, tableTacticCommand.CommandText, dbDataParameters, (ra) => recordsAffected = ra);

				if ((object)records == null)
					throw new InvalidOperationException(string.Format("Records were invalid."));

				record = records.SingleOrDefault();

				if ((object)record == null)
					return null;

				tableModelObject = new TTableModelObject();

				// map to table model from record (destination, source)
				tableTacticCommand.RecordToTableModelMappingCallback(tableModelObject, 0, record);

				if ((object)recordsAffected == null)
					throw new InvalidOperationException(string.Format("Records affected was invalid."));

				if (recordsAffected != tableTacticCommand.ExpectedRecordsAffected)
				{
					// concurrency or nullipotency failure
					unitOfWork.Divergent();

					throw new InvalidOperationException(string.Format("Data concurrency or nullipotency failure occurred during table model fill; actual records affected '{0}' did not equal expected records affected '{1}'.", tableTacticCommand.ExpectedRecordsAffected, recordsAffected));
				}

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

		public override TProjection Query<TProjection>(IUnitOfWork unitOfWork, Func<DzContext, TProjection> contextQueryCallback)
		{
			TProjection projection;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)contextQueryCallback == null)
				throw new ArgumentNullException("contextQueryCallback");

			using (DzContext context = new DzContext())
			{
				projection = contextQueryCallback(context);

				// do not check for null as this is a valid state for the projection
				return projection;
			}
		}

		public override bool Save<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject)
		{
			bool wasNew;
			IEnumerable<IResultset> resultsets;
			IResultset resultset;
			IEnumerable<IRecord> records;
			IRecord record;
			IRecord output;
			int? recordsAffected = null;

			IEnumerable<IDbDataParameter> dbDataParameters;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			wasNew = tableModelObject.IsNew;
			tableModelObject.Mark();

			using (DzContext context = new DzContext())
			{
				ITableTacticCommand<TTableModelObject> tableTacticCommand;

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

				__DEBUG_ProfileTacticCommand(RepositoryOperation.Save, tableTacticCommand);

				// get native parameters
				dbDataParameters = tableTacticCommand.GetDbDataParameters(unitOfWork);

				records = unitOfWork.ExecuteRecords(tableTacticCommand.CommandType, tableTacticCommand.CommandText, dbDataParameters, (ra) => recordsAffected = ra);

				if ((object)records == null)
					throw new InvalidOperationException(string.Format("Records were invalid."));

				record = records.SingleOrDefault();

				// map to table model from record (destination, source)
				if ((object)record != null)
					tableTacticCommand.RecordToTableModelMappingCallback(tableModelObject, 0, record);

				if ((object)recordsAffected == null)
					throw new InvalidOperationException(string.Format("Records affected was invalid."));

				if (recordsAffected <= tableTacticCommand.ExpectedRecordsAffected)
				{
					// concurrency or nullipotency failure
					unitOfWork.Divergent();

					this.OnSaveConflictTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					//throw new InvalidOperationException(string.Format("Data concurrency or nullipotency failure occurred during table model save; actual records affected '{0}' was less than or equal to the expected records affected '{1}'.", TableTacticCommand.ExpectedRecordsAffected, recordsAffected));
					return false;
				}

				// ***------------------------***

				if (wasNew && !tableTacticCommand.UseBatchScopeIdentificationSemantics)
				{
					// this is optional
					tableTacticCommand = this.DataSourceTagStrategy.GetIdentifyTacticCommand<TTableModelObject>(unitOfWork);

					__DEBUG_ProfileTacticCommand(RepositoryOperation.Identify, tableTacticCommand);

					records = unitOfWork.ExecuteRecords(tableTacticCommand.CommandType, tableTacticCommand.CommandText, tableTacticCommand.GetDbDataParameters(unitOfWork), (ra) => recordsAffected = ra);

					if ((object)records == null)
						throw new InvalidOperationException(string.Format("Records were invalid."));

					record = records.SingleOrDefault();

					// map to table model from record (destination, source)
					if ((object)record != null)
						tableTacticCommand.RecordToTableModelMappingCallback(tableModelObject, -1, record);

					if ((object)recordsAffected == null)
						throw new InvalidOperationException(string.Format("Records affected was invalid."));

					if (recordsAffected != tableTacticCommand.ExpectedRecordsAffected)
					{
						// concurrency or nullipotency failure
						unitOfWork.Divergent();

						throw new InvalidOperationException(string.Format("Data concurrency or nullipotency failure occurred during table model fill; actual records affected '{0}' did not equal expected records affected '{1}'.", tableTacticCommand.ExpectedRecordsAffected, recordsAffected));
					}
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