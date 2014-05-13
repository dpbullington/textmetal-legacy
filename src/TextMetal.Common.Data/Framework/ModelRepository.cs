/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;

using TextMetal.Common.Core;
using TextMetal.Common.Solder.DependencyManagement;

namespace TextMetal.Common.Data.Framework
{
	public abstract class ModelRepository : IModelRepository, IUnitOfWorkFactory
	{
		#region Constructors/Destructors

		protected ModelRepository(Func<string, bool> createNativeDatabaseFileCallback)
		{
			if ((object)createNativeDatabaseFileCallback == null)
				throw new ArgumentNullException("createNativeDatabaseFileCallback");

			this.createNativeDatabaseFileCallback = createNativeDatabaseFileCallback;

			if(this.UseDatabaseFile)
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
		private readonly Func<string, bool> createNativeDatabaseFileCallback;

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

		private Func<string, bool> CreateNativeDatabaseFileCallback
		{
			get
			{
				return this.createNativeDatabaseFileCallback;
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
			return DependencyManager.AppDomainInstance.ResolveDependency<TModel>("");
		}

		public virtual TRequestModel CreateRequestModel<TRequestModel>()
			where TRequestModel : class, IRequestModelObject
		{
			return DependencyManager.AppDomainInstance.ResolveDependency<TRequestModel>("");
		}

		public virtual TResponseModel CreateResponseModel<TResponseModel, TResultModel>()
			where TResponseModel : class, IResponseModelObject<TResultModel>
			where TResultModel : class, IResultModelObject
		{
			return DependencyManager.AppDomainInstance.ResolveDependency<TResponseModel>("");
		}

		public virtual TResultModel CreateResultModel<TResultModel>()
			where TResultModel : class, IResultModelObject
		{
			return DependencyManager.AppDomainInstance.ResolveDependency<TResultModel>("");
		}

		public abstract TModel Discard<TModel>(TModel model) where TModel : class, IModelObject;

		public abstract TModel Discard<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

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
				retval = this.CreateNativeDatabaseFileCallback(databaseFilePath);

			return retval;
		}

		public abstract TResponseModel ExecuteImperative<TRequestModel, TResultModel, TResponseModel>(TRequestModel requestModel)
			where TRequestModel : class, IRequestModelObject
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>;

		public abstract TResponseModel ExecuteImperative<TRequestModel, TResultModel, TResponseModel>(IUnitOfWork unitOfWork, TRequestModel requestModel)
			where TRequestModel : class, IRequestModelObject
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>;

		public abstract TModel Fill<TModel>(TModel model) where TModel : class, IModelObject;

		public abstract TModel Fill<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

		public abstract IEnumerable<TModel> Find<TModel>(IModelQuery query) where TModel : class, IModelObject;

		public abstract IEnumerable<TModel> Find<TModel>(IUnitOfWork unitOfWork, IModelQuery query) where TModel : class, IModelObject;

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

		public abstract TModel Load<TModel>(TModel prototype) where TModel : class, IModelObject;

		public abstract TModel Load<TModel>(IUnitOfWork unitOfWork, TModel prototype) where TModel : class, IModelObject;

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
			if ((object)connectionString == null)
				throw new ArgumentNullException("connectionString");

			// do nothing
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

		public abstract TModel Save<TModel>(TModel model) where TModel : class, IModelObject;

		public abstract TModel Save<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

		#endregion
	}
}