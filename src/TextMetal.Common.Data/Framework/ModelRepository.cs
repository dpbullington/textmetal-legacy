/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

using TextMetal.Common.Core;
using TextMetal.Common.Solder.DependencyManagement;

namespace TextMetal.Common.Data.Framework
{
	public abstract class ModelRepository : IModelRepository, IUnitOfWorkFactory
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

		public abstract bool Discard<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

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

		public abstract TResponseModel ExecuteImperative<TRequestModel, TResultModel, TResponseModel>(IUnitOfWork unitOfWork, TRequestModel requestModel)
			where TRequestModel : class, IRequestModelObject
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>;

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

		public abstract bool Fill<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

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

		public abstract IEnumerable<TModel> Find<TModel>(IUnitOfWork unitOfWork, IModelQuery modelQuery) where TModel : class, IModelObject;

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

		public abstract TModel Load<TModel>(IUnitOfWork unitOfWork, TModel prototype) where TModel : class, IModelObject;

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

		public abstract bool Save<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

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

		#endregion
	}
}