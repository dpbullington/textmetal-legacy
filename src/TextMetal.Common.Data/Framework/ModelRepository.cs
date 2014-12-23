/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

using TextMetal.Common.Core;
using TextMetal.Common.Core.Cerealization;
using TextMetal.Common.Data.Framework.Mapping;
using TextMetal.Common.Data.Framework.Strategy;
using TextMetal.Common.Solder.AmbientExecutionContext;
using TextMetal.Common.Solder.DependencyManagement;

namespace TextMetal.Common.Data.Framework
{
	public class ModelRepository : IModelRepository
	{
		#region Constructors/Destructors

		protected ModelRepository()
		{
			this.dataSourceTagStrategy = DataSourceTagStrategyFactory.Instance.GetDataSourceTagStrategy(this.DataSourceTag);

			if (this.UseDatabaseFile)
				this.InitializeFromRevisionHistoryResource();
		}

		#endregion

		#region Fields/Constants

		private const string CONNECTION_STRING_NAME_FORMAT = "{0}::ConnectionString";
		private const string DATA_SOURCE_TAG_FORMAT = "{0}::DataSourceTag";
		private const string DATABASE_DIRECTORY_PATH_FORMAT = "{0}::DatabaseDirectoryPath";
		private const string DATABASE_FILE_NAME_FORMAT = "{0}::DatabaseFileName";
		private const string KILL_DATABASE_FILE_FORMAT = "{0}::KillDatabaseFile";
		private const string RESOURCE_NAME_FORMAT = "{0}.SQL.RevisionHistory.({1}).xml";
		private const string USE_DATABASE_FILE_FORMAT = "{0}::UseDatabaseFile";
		private readonly IDataSourceTagStrategy dataSourceTagStrategy;

		#endregion

		#region Properties/Indexers/Events

		public string ConnectionString
		{
			get
			{
				string connectionString;

				connectionString = AppConfig.GetConnectionString(this.ConnectionStringName);

				this.OnPreProcessConnectionString(ref connectionString);

				return connectionString;
			}
		}

		public string ConnectionStringName
		{
			get
			{
				string connectionStringName;

				connectionStringName = string.Format(CONNECTION_STRING_NAME_FORMAT, this.GetType().Namespace);

				return connectionStringName;
			}
		}

		public Type ConnectionType
		{
			get
			{
				return Type.GetType(AppConfig.GetConnectionProvider(this.ConnectionStringName), true);
			}
		}

		public string DatabaseDirectoryPath
		{
			get
			{
				return AppConfig.GetAppSetting<string>(string.Format(DATABASE_DIRECTORY_PATH_FORMAT, this.GetType().Namespace));
			}
		}

		public string DatabaseFileName
		{
			get
			{
				return AppConfig.GetAppSetting<string>(string.Format(DATABASE_FILE_NAME_FORMAT, this.GetType().Namespace));
			}
		}

		public string DatabaseFilePath
		{
			get
			{
				string value;

				// {0} == GetApplicationUserSpecificDirectoryPath()
				value = Path.Combine(string.Format(this.DatabaseDirectoryPath ?? string.Empty, GetApplicationUserSpecificDirectoryPath()), this.DatabaseFileName);

				return value;
			}
		}

		public string DataSourceTag
		{
			get
			{
				string dataSourceTag;
				string value;

				dataSourceTag = string.Format(DATA_SOURCE_TAG_FORMAT, this.GetType().Namespace);

				if (!AppConfig.HasAppSetting(dataSourceTag))
					return null;

				value = AppConfig.GetAppSetting<string>(dataSourceTag);

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

		public bool KillDatabaseFile
		{
			get
			{
				bool value;

				if (!AppConfig.HasAppSetting(string.Format(KILL_DATABASE_FILE_FORMAT, this.GetType().Namespace)))
					return false;

				value = AppConfig.GetAppSetting<bool>(string.Format(KILL_DATABASE_FILE_FORMAT, this.GetType().Namespace));

				return value;
			}
		}

		public bool UseDatabaseFile
		{
			get
			{
				bool value;

				value = AppConfig.GetAppSetting<bool>(string.Format(USE_DATABASE_FILE_FORMAT, this.GetType().Namespace));

				return value;
			}
		}

		#endregion

		#region Methods/Operators

		public virtual TModel CreateModel<TModel>()
			where TModel : class, IModelObject
		{
			return this.CreateModel<TModel>((m) =>
											{
											});
		}

		public virtual TModel CreateModel<TModel>(Action<TModel> initializionCallback)
			where TModel : class, IModelObject
		{
			TModel model;

			if ((object)initializionCallback == null)
				throw new ArgumentNullException("initializionCallback");

			model = DependencyManager.AppDomainInstance.ResolveDependency<TModel>(string.Empty);
			initializionCallback(model);

			return model;
		}

		public virtual TRequestModel CreateRequestModel<TRequestModel>()
			where TRequestModel : class, IRequestModelObject
		{
			return DependencyManager.AppDomainInstance.ResolveDependency<TRequestModel>(string.Empty);
		}

		public virtual TResponseModel CreateResponseModel<TResultModel, TResponseModel>()
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>
		{
			return DependencyManager.AppDomainInstance.ResolveDependency<TResponseModel>(string.Empty);
		}

		public virtual TResultModel CreateResultModel<TResultModel>()
			where TResultModel : class, IResultModelObject
		{
			return DependencyManager.AppDomainInstance.ResolveDependency<TResultModel>(string.Empty);
		}

		public virtual bool Discard<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject
		{
			bool wasNew;
			IEnumerable<IDictionary<string, object>> rows;
			IDictionary<string, object> table;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			wasNew = model.IsNew;
			model.Mark();

			if (wasNew)
				return true;

			using (GarbageDisposable.Instance)
			{
				TacticCommand<TModel> tacticCommand;
				int actualRecordsAffected;

				this.OnPreDeleteModel<TModel>(unitOfWork, model);

				tacticCommand = this.DataSourceTagStrategy.GetDeleteTacticCommand<TModel>(unitOfWork, model, null);

				this.OnProfileTacticCommand(RepositoryOperation.Discard, tacticCommand);

				rows = unitOfWork.ExecuteDictionary(tacticCommand.CommandType, tacticCommand.CommandText, tacticCommand.CommandParameters, out actualRecordsAffected);

				if (actualRecordsAffected <= tacticCommand.ExpectedRecordsAffected)
				{
					// concurrency failure
					unitOfWork.Divergent();

					this.OnSaveConflictModel<TModel>(unitOfWork, model);

					//throw new InvalidOperationException(string.Format("Data concurrency failure occurred during model save; actual records affected '{0}' was less than or equal to the expected records affected '{1}'.", tacticCommand.ExpectedRecordsAffected, actualRecordsAffected));
					return false;
				}

				if ((object)rows == null)
					throw new InvalidOperationException(string.Format("Rows were invalid."));

				table = rows.SingleOrDefault();

				this.OnProfileTacticCommand(RepositoryOperation.DiscardYield, tacticCommand);

				// map to model from table (destination, source)
				if ((object)table != null)
					tacticCommand.TableToModelMappingCallback(model, table);

				this.OnPostDeleteModel<TModel>(unitOfWork, model);

				model.IsNew = false;

				return true;
			}
		}

		public virtual bool Discard<TModel>(TModel model)
			where TModel : class, IModelObject
		{
			bool retval;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					retval = this.Discard<TModel>(unitOfWork, model);

					unitOfWork.Complete();
				}
			}
			else
				retval = this.Discard<TModel>(UnitOfWork.Current, model);

			return retval;
		}

		private bool EnsureDatabaseFile()
		{
			string databaseFilePath;
			string databaseDirectoryPath;
			bool retval = false;

			databaseFilePath = Path.GetFullPath(this.DatabaseFilePath);
			databaseDirectoryPath = Path.GetDirectoryName(databaseFilePath);

			if (!Directory.Exists(databaseDirectoryPath))
				Directory.CreateDirectory(databaseDirectoryPath);

			if (!File.Exists(databaseFilePath))
				retval = this.OnCreateNativeDatabaseFile(databaseFilePath);

			return retval;
		}

		public virtual TResponseModel Execute<TRequestModel, TResultModel, TResponseModel>(IUnitOfWork unitOfWork, TRequestModel requestModel)
			where TRequestModel : class, IRequestModelObject
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>
		{
			Type requestModelType;
			Type resultModelType;
			Type responseModelType;
			TResponseModel responseModel;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)requestModel == null)
				throw new ArgumentNullException("requestModel");

			requestModelType = typeof(TRequestModel);
			resultModelType = typeof(TResultModel);
			responseModelType = typeof(TResponseModel);

			using (GarbageDisposable.Instance)
			{
				TacticCommand<TRequestModel, TResultModel, TResponseModel> tacticCommand;

				this.OnPreExecuteRequestModel<TRequestModel>(unitOfWork, requestModel);

				tacticCommand = this.DataSourceTagStrategy.GetExecuteTacticCommand<TRequestModel, TResultModel, TResponseModel>(unitOfWork, requestModel);

				this.OnProfileTacticCommand(RepositoryOperation.Execute, tacticCommand);

				responseModel = this.CreateResponseModel<TResultModel, TResponseModel>();
				responseModel.Results = this.GetResultsLazy<TRequestModel, TResultModel, TResponseModel>(unitOfWork, tacticCommand, responseModel);

				return responseModel;
			}
		}

		public virtual TResponseModel Execute<TRequestModel, TResultModel, TResponseModel>(TRequestModel requestModel)
			where TRequestModel : class, IRequestModelObject
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>
		{
			TResponseModel responseModel;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					responseModel = this.Execute<TRequestModel, TResultModel, TResponseModel>(unitOfWork, requestModel);

					unitOfWork.Complete();
				}
			}
			else
				responseModel = this.Execute<TRequestModel, TResultModel, TResponseModel>(UnitOfWork.Current, requestModel);

			return responseModel;
		}

		public virtual bool Fill<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject
		{
			Type modelType;
			IEnumerable<IDictionary<string, object>> rows;
			IDictionary<string, object> table;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			modelType = typeof(TModel);

			using (GarbageDisposable.Instance)
			{
				TacticCommand<TModel> tacticCommand;
				int actualRecordsAffected;

				tacticCommand = this.DataSourceTagStrategy.GetSelectTacticCommand<TModel>(unitOfWork, model, null);

				this.OnProfileTacticCommand(RepositoryOperation.Fill, tacticCommand);

				rows = unitOfWork.ExecuteDictionary(tacticCommand.CommandType, tacticCommand.CommandText, tacticCommand.CommandParameters, out actualRecordsAffected);

				if (actualRecordsAffected != tacticCommand.ExpectedRecordsAffected)
				{
					// idempotency failure
					unitOfWork.Divergent();

					throw new InvalidOperationException(string.Format("Data idempotency failure occurred during model fill; actual records affected '{0}' did not equal expected records affected '{1}'.", tacticCommand.ExpectedRecordsAffected, actualRecordsAffected));
				}

				if ((object)rows == null)
					throw new InvalidOperationException(string.Format("Rows were invalid."));

				table = rows.SingleOrDefault();

				this.OnProfileTacticCommand(RepositoryOperation.FillYield, tacticCommand);

				if ((object)table == null)
					return false;

				// map to model from table (destination, source)
				tacticCommand.TableToModelMappingCallback(model, table);

				this.OnSelectModel(unitOfWork, model);

				return true;
			}
		}

		public virtual bool Fill<TModel>(TModel model)
			where TModel : class, IModelObject
		{
			bool retval;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					retval = this.Fill<TModel>(unitOfWork, model);

					unitOfWork.Complete();
				}
			}
			else
				retval = this.Fill<TModel>(UnitOfWork.Current, model);

			return retval;
		}

		public virtual IEnumerable<TModel> Find<TModel>(IUnitOfWork unitOfWork, IModelQuery modelQuery) where TModel : class, IModelObject
		{
			Type modelType;
			TModel model, dummy;
			IEnumerable<IDictionary<string, object>> rows;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)modelQuery == null)
				throw new ArgumentNullException("modelQuery");

			modelType = typeof(TModel);
			dummy = this.CreateModel<TModel>();

			using (GarbageDisposable.Instance)
			{
				TacticCommand<TModel> tacticCommand;
				int actualRecordsAffected = int.MaxValue;

				tacticCommand = this.DataSourceTagStrategy.GetSelectTacticCommand<TModel>(unitOfWork, dummy, modelQuery);

				this.OnProfileTacticCommand(RepositoryOperation.Find, tacticCommand);

				// enumerator overload usage
				rows = unitOfWork.ExecuteDictionary(tacticCommand.CommandType, tacticCommand.CommandText, tacticCommand.CommandParameters, (ra) => actualRecordsAffected = ra);

				if ((object)rows == null)
					throw new InvalidOperationException(string.Format("Rows were invalid."));

				Trace.WriteLine("[+++ before yield: Find +++]");

				// DOES NOT FORCE EXECUTION AGAINST STORE
				foreach (IDictionary<string, object> table in rows)
				{
					model = this.CreateModel<TModel>();

					// map to model from table (destination, source)
					tacticCommand.TableToModelMappingCallback(model, table);

					this.OnSelectModel<TModel>(unitOfWork, model);

					yield return model; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}

				this.OnProfileTacticCommand(RepositoryOperation.FindYield, tacticCommand);

				Trace.WriteLine("[+++ after yield: Find +++]");

				// special case for enumerator
				if (actualRecordsAffected != tacticCommand.ExpectedRecordsAffected)
				{
					// idempotency failure
					unitOfWork.Divergent();

					throw new InvalidOperationException(string.Format("Data idempotency failure occurred during model load; actual records affected '{0}' did not equal expected records affected '{1}'.", tacticCommand.ExpectedRecordsAffected, actualRecordsAffected));
				}

				this.OnProfileTacticCommand(RepositoryOperation.Find, tacticCommand);
			}
		}

		public virtual IEnumerable<TModel> Find<TModel>(IModelQuery modelQuery)
			where TModel : class, IModelObject
		{
			IEnumerable<TModel> models;

			if ((object)modelQuery == null)
				throw new ArgumentNullException("modelQuery");

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					models = this.Find<TModel>(unitOfWork, modelQuery);

					models = models.ToList(); // FORCE EAGER LOAD

					unitOfWork.Complete();
				}
			}
			else
			{
				models = this.Find<TModel>(UnitOfWork.Current, modelQuery);

				// DO NOT FORCE EAGER LOAD
			}

			return models;
		}

		private IEnumerable<TResultModel> GetResultsLazy<TRequestModel, TResultModel, TResponseModel>(IUnitOfWork unitOfWork, TacticCommand<TRequestModel, TResultModel, TResponseModel> tacticCommand, TResponseModel responseModel)
			where TRequestModel : class, IRequestModelObject
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>
		{
			IEnumerable<IDictionary<string, object>> rows;
			TResultModel resultModel;

			int actualRecordsAffected = int.MaxValue;
			IDictionary<string, object> tablix;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tacticCommand == null)
				throw new ArgumentNullException("tacticCommand");

			if ((object)responseModel == null)
				throw new ArgumentNullException("responseModel");

			// enumerator overload usage
			rows = unitOfWork.ExecuteDictionary(tacticCommand.CommandType, tacticCommand.CommandText, tacticCommand.CommandParameters, (ra) => actualRecordsAffected = ra);

			if ((object)rows == null)
				throw new InvalidOperationException(string.Format("Rows were invalid."));

			Trace.WriteLine("[+++ before yield: GetResultsLazy +++]");

			// DOES NOT FORCE EXECUTION AGAINST STORE
			foreach (IDictionary<string, object> table in rows)
			{
				resultModel = this.CreateResultModel<TResultModel>();

				// map to model from table (destination, source)
				tacticCommand.TableToResultModelMappingCallback(resultModel, table);

				this.OnPostExecuteResultModel<TResultModel>(unitOfWork, resultModel);

				yield return resultModel; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
			}

			this.OnProfileTacticCommand(RepositoryOperation.ExecuteYield, tacticCommand);

			Trace.WriteLine("[+++ after yield: GetResultsLazy +++]");

			// alert the response model that we are done enumerating the lazy river...
			responseModel.SignalEnumerationComplete();

			// ***** SUPER special case for enumerator *****
			if (actualRecordsAffected != tacticCommand.ExpectedRecordsAffected)
			{
				// idempotency failure
				unitOfWork.Divergent();

				throw new InvalidOperationException(string.Format("Data idempotency failure occurred during model execute; actual records affected '{0}' did not equal expected records affected '{1}'.", tacticCommand.ExpectedRecordsAffected, actualRecordsAffected));
			}

			tablix = new Dictionary<string, object>();

			foreach (IDbDataParameter commandParameter in tacticCommand.CommandParameters)
			{
				if (commandParameter.Direction != ParameterDirection.InputOutput &&
					commandParameter.Direction != ParameterDirection.Output &&
					commandParameter.Direction != ParameterDirection.ReturnValue)
					continue;

				tablix.Add(commandParameter.ParameterName, commandParameter.Value);
			}

			// map to model from table (destination, source)
			tacticCommand.TableToResponseModelMappingCallback(responseModel, tablix);

			this.OnPostExecuteResponseModel<TResultModel, TResponseModel>(unitOfWork, responseModel);
		}

		public virtual IUnitOfWork GetUnitOfWork()
		{
			return UnitOfWork.Create(this.ConnectionType, this.ConnectionString, true);
		}

		private void InitializeFromRevisionHistoryResource()
		{
			string resourceName;
			DatabaseHistory databaseHistory;

			resourceName = string.Format(RESOURCE_NAME_FORMAT, this.GetType().Namespace, this.DataSourceTag.SafeToString().ToLower());

			if (this.UseDatabaseFile)
			{
				if (!DataType.Instance.IsNullOrWhiteSpace(this.DatabaseFilePath))
				{
					if (this.KillDatabaseFile)
					{
						if (File.Exists(this.DatabaseFilePath))
							File.Delete(this.DatabaseFilePath);
					}

					this.EnsureDatabaseFile();
				}

				if (!Cerealization.TryGetFromAssemblyResource<DatabaseHistory>(this.GetType(), resourceName, out databaseHistory))
					throw new InvalidOperationException(string.Format("Unable to deserialize instance of '{0}' from the manifest resource name '{1}' in the assembly '{2}'.", typeof(DatabaseHistory).FullName, resourceName, this.GetType().Assembly.FullName));

				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					databaseHistory.PerformSchemaUpgrade(unitOfWork);

					unitOfWork.Complete();
				}
			}
		}

		public virtual TModel Load<TModel>(IUnitOfWork unitOfWork, TModel prototype) where TModel : class, IModelObject
		{
			Type modelType;
			TModel model;
			IEnumerable<IDictionary<string, object>> rows;
			IDictionary<string, object> table;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)prototype == null)
				throw new ArgumentNullException("prototype");

			modelType = typeof(TModel);

			using (GarbageDisposable.Instance)
			{
				TacticCommand<TModel> tacticCommand;
				int actualRecordsAffected;

				tacticCommand = this.DataSourceTagStrategy.GetSelectTacticCommand<TModel>(unitOfWork, prototype, null);

				this.OnProfileTacticCommand(RepositoryOperation.Load, tacticCommand);

				rows = unitOfWork.ExecuteDictionary(tacticCommand.CommandType, tacticCommand.CommandText, tacticCommand.CommandParameters, out actualRecordsAffected);

				if (actualRecordsAffected != tacticCommand.ExpectedRecordsAffected)
				{
					// idempotency failure
					unitOfWork.Divergent();

					throw new InvalidOperationException(string.Format("Data idempotency failure occurred during model load; actual records affected '{0}' did not equal expected records affected '{1}'.", tacticCommand.ExpectedRecordsAffected, actualRecordsAffected));
				}

				if ((object)rows == null)
					throw new InvalidOperationException(string.Format("Rows were invalid."));

				table = rows.SingleOrDefault();

				this.OnProfileTacticCommand(RepositoryOperation.LoadYield, tacticCommand);

				if ((object)table == null)
					return null;

				model = this.CreateModel<TModel>();

				// map to model from table (destination, source)
				tacticCommand.TableToModelMappingCallback(model, table);

				this.OnSelectModel(unitOfWork, model);

				return model;
			}
		}

		public virtual TModel Load<TModel>(TModel prototype)
			where TModel : class, IModelObject
		{
			TModel model;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					model = this.Load<TModel>(unitOfWork, prototype);

					unitOfWork.Complete();
				}
			}
			else
				model = this.Load<TModel>(UnitOfWork.Current, prototype);

			return model;
		}

		protected virtual bool OnCreateNativeDatabaseFile(string databaseFilePath)
		{
			if (this.DataSourceTagStrategy.CanCreateNativeDatabaseFile)
				return this.DataSourceTagStrategy.CreateNativeDatabaseFile(databaseFilePath);
			else
				return false;
		}

		protected virtual void OnDiscardConflictModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			// do nothing
		}

		protected virtual void OnPostDeleteModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			// do nothing
		}

		protected virtual void OnPostExecuteResponseModel<TResultModel, TResponseModel>(IUnitOfWork unitOfWork, TResponseModel responseModel)
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)responseModel == null)
				throw new ArgumentNullException("responseModel");

			// do nothing
		}

		protected virtual void OnPostExecuteResultModel<TResultModel>(IUnitOfWork unitOfWork, TResultModel resultModel) where TResultModel : class, IResultModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)resultModel == null)
				throw new ArgumentNullException("resultModel");

			// do nothing
		}

		protected virtual void OnPostInsertModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			// do nothing
		}

		protected virtual void OnPostUpdateModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			// do nothing
		}

		protected virtual void OnPreDeleteModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			// do nothing
		}

		protected virtual void OnPreExecuteRequestModel<TRequestModel>(IUnitOfWork unitOfWork, TRequestModel requestModel) where TRequestModel : class, IRequestModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)requestModel == null)
				throw new ArgumentNullException("requestModel");

			// do nothing
		}

		public virtual void OnPreInsertModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			model.Mark();
		}

		protected virtual void OnPreProcessConnectionString(ref string connectionString)
		{
			if (!this.UseDatabaseFile)
				return;

			if ((object)connectionString == null)
				throw new ArgumentNullException("connectionString");

			// {0} = this.DatabaseFilePath
			connectionString = string.Format(connectionString ?? string.Empty, this.DatabaseFilePath);
		}

		protected virtual void OnPreUpdateModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			model.Mark();
		}

		[Conditional("DEBUG")]
		protected virtual void OnProfileTacticCommand(RepositoryOperation repositoryOperation, ITacticCommand tacticCommand)
		{
			/* THIS METHOD SHOULD NOT BE DEFINED IN RELEASE/PRODUCTION BUILDS */
			string value = string.Empty;
			int i;
			const string INDENT_CHAR = "\t";

			if ((object)tacticCommand == null)
				throw new ArgumentNullException("tacticCommand");

			value += string.Format(Environment.NewLine + "[+++ begin OnProfileTacticCommand({0}) +++]" + Environment.NewLine, repositoryOperation);

			value += string.Format("[TacticCommand]: ModelType = '{0}', IsNullipotent = '{6}'; ExpectedRecordsAffected = '{7}'; CommandType = '{1}'; CommandText = '{2}'; CommandPrepare = '{3}'; CommandTimeout = '{4}'; CommandBehavior = '{5}'.",
				string.Join("|", tacticCommand.GetModelTypes().Select(t => t.FullName).ToArray()),
				tacticCommand.CommandType,
				tacticCommand.CommandText,
				tacticCommand.CommandPrepare,
				tacticCommand.CommandTimeout,
				tacticCommand.CommandBehavior,
				tacticCommand.IsNullipotent,
				tacticCommand.ExpectedRecordsAffected);

			i = 0;
			foreach (IDbDataParameter commandParameter in tacticCommand.CommandParameters)
			{
				value += string.Format(Environment.NewLine + INDENT_CHAR + "[Parameter{0:00}]: Direction = '{1}'; ParameterName = '{2}'; IsNullable = '{3}'; Precision = '{4}'; Scale = '{5}'; Size = '{6}'; DbType = '{7}'; Value = '{8}'.",
					i++,
					commandParameter.Direction,
					commandParameter.ParameterName,
					commandParameter.IsNullable,
					commandParameter.Precision,
					commandParameter.Scale,
					commandParameter.Size,
					commandParameter.DbType,
					(object)commandParameter != null ? commandParameter.Value.SafeToString(null, "<null>") : "[[null]]");
			}

			value += string.Format(Environment.NewLine + "[+++ end OnProfileTacticCommand({0}) +++]" + Environment.NewLine, repositoryOperation);

			Trace.WriteLine(value);
		}

		protected virtual void OnSaveConflictModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			// do nothing
		}

		protected virtual void OnSelectModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			// do nothing
		}

		public virtual bool Save<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject
		{
			bool wasNew;
			IEnumerable<IDictionary<string, object>> rows;
			IDictionary<string, object> table;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			wasNew = model.IsNew;
			model.Mark();

			using (GarbageDisposable.Instance)
			{
				TacticCommand<TModel> tacticCommand;
				int actualRecordsAffected;

				if (wasNew)
				{
					this.OnPreInsertModel<TModel>(unitOfWork, model);

					tacticCommand = this.DataSourceTagStrategy.GetInsertTacticCommand<TModel>(unitOfWork, model, null);
				}
				else
				{
					this.OnPreUpdateModel<TModel>(unitOfWork, model);

					tacticCommand = this.DataSourceTagStrategy.GetUpdateTacticCommand<TModel>(unitOfWork, model, null);
				}

				this.OnProfileTacticCommand(RepositoryOperation.Save, tacticCommand);

				rows = unitOfWork.ExecuteDictionary(tacticCommand.CommandType, tacticCommand.CommandText, tacticCommand.CommandParameters, out actualRecordsAffected);

				if (actualRecordsAffected <= tacticCommand.ExpectedRecordsAffected)
				{
					// concurrency failure
					unitOfWork.Divergent();

					this.OnSaveConflictModel<TModel>(unitOfWork, model);

					//throw new InvalidOperationException(string.Format("Data concurrency failure occurred during model save; actual records affected '{0}' was less than or equal to the expected records affected '{1}'.", tacticCommand.ExpectedRecordsAffected, actualRecordsAffected));
					return false;
				}

				if ((object)rows == null)
					throw new InvalidOperationException(string.Format("Rows were invalid."));

				table = rows.SingleOrDefault();

				// map to model from table (destination, source)
				if ((object)table != null)
					tacticCommand.TableToModelMappingCallback(model, table);

				// ***------------------------***

				if (wasNew && !tacticCommand.UseBatchScopeIdentificationSemantics)
				{
					// this is optional
					tacticCommand = this.DataSourceTagStrategy.GetIdentifyTacticCommand<TModel>(unitOfWork);

					this.OnProfileTacticCommand(RepositoryOperation.Identify, tacticCommand);

					rows = unitOfWork.ExecuteDictionary(tacticCommand.CommandType, tacticCommand.CommandText, tacticCommand.CommandParameters, out actualRecordsAffected);

					if (actualRecordsAffected != tacticCommand.ExpectedRecordsAffected)
					{
						// idempotency failure
						unitOfWork.Divergent();

						throw new InvalidOperationException(string.Format("Data idempotency failure occurred during model fill; actual records affected '{0}' did not equal expected records affected '{1}'.", tacticCommand.ExpectedRecordsAffected, actualRecordsAffected));
					}

					if ((object)rows == null)
						throw new InvalidOperationException(string.Format("Rows were invalid."));

					table = rows.SingleOrDefault();

					this.OnProfileTacticCommand(RepositoryOperation.IdentifyYield, tacticCommand);

					if ((object)table == null)
						return false;

					// map to model from table (destination, source)
					tacticCommand.TableToModelMappingCallback(model, table);
				}

				// ***------------------------***

				if (wasNew)
					this.OnPostInsertModel<TModel>(unitOfWork, model);
				else
					this.OnPostUpdateModel<TModel>(unitOfWork, model);

				return true;
			}
		}

		public virtual bool Save<TModel>(TModel model)
			where TModel : class, IModelObject
		{
			bool retval;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					retval = this.Save<TModel>(unitOfWork, model);

					unitOfWork.Complete();
				}
			}
			else
				retval = this.Save<TModel>(UnitOfWork.Current, model);

			return retval;
		}

		private static string GetApplicationUserSpecificDirectoryPath()
		{
			Assembly assembly;
			AssemblyInformation assemblyInformation;
			string userSpecificDirectoryPath;

			if (HttpContextExecutionPathStorage.IsInHttpContext)
				userSpecificDirectoryPath = Path.GetFullPath(HttpContext.Current.Server.MapPath("~/"));
			else
			{
				assembly = Assembly.GetExecutingAssembly();
				assemblyInformation = new AssemblyInformation(assembly);

				if ((object)assemblyInformation.Company != null &&
					(object)assemblyInformation.Product != null &&
					(object)assemblyInformation.Win32FileVersion != null)
				{
					userSpecificDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
					userSpecificDirectoryPath = Path.Combine(userSpecificDirectoryPath, assemblyInformation.Company);
					userSpecificDirectoryPath = Path.Combine(userSpecificDirectoryPath, assemblyInformation.Product);
					userSpecificDirectoryPath = Path.Combine(userSpecificDirectoryPath, assemblyInformation.Win32FileVersion);
				}
				else
					userSpecificDirectoryPath = Path.GetFullPath(".");
			}

			return userSpecificDirectoryPath;
		}

		protected static TValue GetScalar<TValue>(IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters)
		{
			int recordsAffected;
			IEnumerable<IDictionary<string, object>> results;
			IDictionary<string, object> result;
			object dbValue;

			results = unitOfWork.ExecuteDictionary(commandType, commandText, commandParameters, out recordsAffected);

			if ((object)results == null)
				return default(TValue);

			result = results.SingleOrDefault();

			if ((object)result == null)
				return default(TValue);

			if (result.Count != 1)
				return default(TValue);

			if (result.Keys.Count != 1)
				return default(TValue);

			dbValue = result[result.Keys.First()];

			return dbValue.ChangeType<TValue>();
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		protected enum RepositoryOperation
		{
			None = 0,
			Discard,
			DiscardYield,
			Execute,
			ExecuteYield,
			Fill,
			FillYield,
			Find,
			FindYield,
			Load,
			LoadYield,
			Save,
			SaveYield,
			Identify,
			IdentifyYield
		}

		#endregion
	}
}