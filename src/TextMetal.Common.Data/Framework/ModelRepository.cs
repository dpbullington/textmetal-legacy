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
using TextMetal.Common.Data.Framework.Strategy;

namespace TextMetal.Common.Data.Framework
{
	public class ModelRepository : IModelRepository
	{
		#region Constructors/Destructors

		protected ModelRepository()
		{
			if (this.UseDatabaseFile)
				this.InitializeFromRevisionHistoryResource();
		}

		#endregion

		#region Fields/Constants

		private const string CONNECTION_STRING_NAME_FORMAT = "{0}::ConnectionString";
		private const string DATABASE_DIRECTORY_PATH_FORMAT = "{0}::DatabaseDirectoryPath";
		private const string DATABASE_FILE_NAME_FORMAT = "{0}::DatabaseFileName";
		private const string DATA_SOURCE_TAG_FORMAT = "{0}::DataSourceTag";
		private const string KILL_DATABASE_FILE_FORMAT = "{0}::KillDatabaseFile";
		private const string RESOURCE_NAME_FORMAT = "{0}.SQL.RevisionHistory({1}).xml";
		private const string USE_DATABASE_FILE_FORMAT = "{0}::UseDatabaseFile";

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
				value = Path.Combine(string.Format(this.DatabaseDirectoryPath ?? "", GetApplicationUserSpecificDirectoryPath()), this.DatabaseFileName);

				return value;
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

		private static string GetApplicationUserSpecificDirectoryPath()
		{
			Assembly assembly;
			AssemblyInformation assemblyInformation;
			string userSpecificDirectoryPath;

			if (ExecutionPathStorage.IsInHttpContext)
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

		public virtual TModel CreateModel<TModel>()
			where TModel : class, IModelObject
		{
			return (TModel)Activator.CreateInstance(typeof(TModel), true);
		}

		public virtual TRequestModel CreateRequestModel<TRequestModel>()
			where TRequestModel : class, IRequestModelObject
		{
			return (TRequestModel)Activator.CreateInstance(typeof(TRequestModel), true);
		}

		public virtual TResponseModel CreateResponseModel<TResponseModel, TResultModel>()
			where TResponseModel : class, IResponseModelObject<TResultModel>
			where TResultModel : class, IResultModelObject
		{
			return (TResponseModel)Activator.CreateInstance(typeof(TResponseModel), true);
		}

		public virtual TResultModel CreateResultModel<TResultModel>()
			where TResultModel : class, IResultModelObject
		{
			return (TResultModel)Activator.CreateInstance(typeof(TResultModel), true);
		}

		public virtual bool Discard<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject
		{
			bool wasNew;

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
				this.OnPreDeleteModel<TModel>(unitOfWork, model);

				try
				{
					// do some shit
				}
				catch (Exception ex)
				{
					this.OnDiscardConflictModel<TModel>(unitOfWork, model);

					return false;
				}

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

		public virtual TResponseModel ExecuteImperative<TRequestModel, TResultModel, TResponseModel>(IUnitOfWork unitOfWork, TRequestModel requestModel)
			where TRequestModel : class, IRequestModelObject
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>
		{
			throw new NotImplementedException();
		}

		public virtual TResponseModel ExecuteImperative<TRequestModel, TResultModel, TResponseModel>(TRequestModel requestModel)
			where TRequestModel : class, IRequestModelObject
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>
		{
			TResponseModel responseModel;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					responseModel = this.ExecuteImperative<TRequestModel, TResultModel, TResponseModel>(unitOfWork, requestModel);

					unitOfWork.Complete();
				}
			}
			else
				responseModel = this.ExecuteImperative<TRequestModel, TResultModel, TResponseModel>(UnitOfWork.Current, requestModel);

			return responseModel;
		}

		public virtual bool Fill<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject
		{
			dynamic table;
			Action<TModel, dynamic> tableToModelMappingCallback;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			table = null;
			tableToModelMappingCallback = (m, t) => {};

			using (GarbageDisposable.Instance)
			{
				if ((object)table == null)
					return false;

				// map to POCO model from L2S table (destination, source)
				tableToModelMappingCallback(model, table);

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
			TModel model;
			dynamic tables;
			Action<TModel, dynamic> tableToModelMappingCallback;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)modelQuery == null)
				throw new ArgumentNullException("modelQuery");

			tables = null;
			tableToModelMappingCallback = (m, t) => {};

			using (GarbageDisposable.Instance)
			{
				// DOES NOT FORCE EXECUTION AGAINST STORE
				foreach (dynamic table in tables)
				{
					model = this.CreateModel<TModel>();

					// map to POCO model from L2S table (destination, source)
					tableToModelMappingCallback(model, table);

					this.OnSelectModel<TModel>(unitOfWork, model);

					yield return model; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}
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
				if (!DataType.IsNullOrWhiteSpace(this.DatabaseFilePath))
				{
					if (this.KillDatabaseFile)
					{
						if (File.Exists(this.DatabaseFilePath))
							File.Delete(this.DatabaseFilePath);
					}

					this.EnsureDatabaseFile();
				}

				if (!Cerealization.Cerealization.TryGetFromAssemblyResource<DatabaseHistory>(this.GetType(), resourceName, out databaseHistory))
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
			TModel model;
			dynamic table;
			Action<TModel, dynamic> tableToModelMappingCallback;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)prototype == null)
				throw new ArgumentNullException("prototype");

			table = null;
			tableToModelMappingCallback = (m, t) => {};

			using (GarbageDisposable.Instance)
			{
				model = this.CreateModel<TModel>();

				// map to POCO model from L2S table (destination, source)
				tableToModelMappingCallback(model, table);

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
			return DataSourceTagStrategyFactory.Instance.GetDataSourceTagStrategy(DataSourceTag).CreateNativeDatabaseFile(databaseFilePath);
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
			connectionString = string.Format(connectionString ?? "", this.DatabaseFilePath);
		}

		protected virtual void OnPreUpdateModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			model.Mark();
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
			dynamic table;
			Action<TModel, dynamic> tableToModelMappingCallback;
			Action<dynamic, TModel> modelToTableMappingCallback;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			tableToModelMappingCallback = (m, t) => {};
			modelToTableMappingCallback = (m, t) => {};

			wasNew = model.IsNew;
			model.Mark();

			using (GarbageDisposable.Instance)
			{
				if (wasNew)
				{
					this.OnPreInsertModel<TModel>(unitOfWork, model);

					// INSERT
					table = null;
				}
				else
				{
					this.OnPreUpdateModel<TModel>(unitOfWork, model);

					table = null;

					if ((object)table == null)
						return false;
				}

				// map to L2S table from POCO model (destination, source)
				modelToTableMappingCallback(table, model);

				try
				{
					// do some shit
				}
				catch (Exception ex)
				{
					this.OnSaveConflictModel<TModel>(unitOfWork, model);

					return false;
				}

				// map to POCO model from L2S table (destination, source)
				tableToModelMappingCallback(model, table);

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

		[Conditional("DEBUG")]
		protected virtual void OnProfileCommand(Type modelType, CommandType commandType, string commandText, IList<IDataParameter> commandParameters, bool executeAsCud, int thisOrThatRecordsAffected)
		{
			/* THIS METHOD SHOULD NOT BE DEFINED IN RELEASE/PRODUCTION BUILDS */

			// these are by convention in the ExecuteDictionary(...) API
			const bool COMMAND_PREPARE = false;
			/* const */
			int? COMMAND_TIMEOUT = null;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			string value = "";
			int i;

			value += "\r\n[+++ begin trace +++]\r\n";

			value += string.Format("[Command]: Type = '{0}'; Text = '{1}'; Prepare = '{2}'; Timeout = '{3}'; Behavior = '{4}'.",
				commandType, commandText, COMMAND_PREPARE, COMMAND_TIMEOUT, COMMAND_BEHAVIOR);

			i = 0;
			foreach (IDbDataParameter commandParameter in commandParameters)
			{
				value += string.Format("\r\n\t[Parameter{0:00}]: Direction = '{1}'; ParameterName = '{2}'; IsNullable = '{3}'; Precision = '{4}'; Scale = '{5}'; Size = '{6}'; DbType = '{7}'; Value = '{8}'.",
					i++, commandParameter.Direction, commandParameter.ParameterName, commandParameter.IsNullable, commandParameter.Precision, commandParameter.Scale, commandParameter.Size, commandParameter.DbType, (object)commandParameter != null ? commandParameter.Value : "<<null>>");
			}

			value += "\r\n[+++ end trace +++]\r\n";

			Trace.WriteLine(value);
		}

		#endregion
	}
}