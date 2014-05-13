/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data.Framework
{
	public abstract class ModelRepository : IModelRepository, IUnitOfWorkFactory
	{
		#region Constructors/Destructors

		protected ModelRepository(INativeDatabaseFileFactory nativeDatabaseFileFactory)
		{
			string appConfigPrefix;

			if ((object)nativeDatabaseFileFactory == null)
				throw new ArgumentNullException("nativeDatabaseFileFactory");

			appConfigPrefix = string.Format(APP_CONFIG_PREFIX_FORMAT, this.GetType().Namespace);

			this.databaseFileManager = new DatabaseFileManager(appConfigPrefix, nativeDatabaseFileFactory);
		}

		#endregion

		#region Fields/Constants

		private const string APP_CONFIG_PREFIX_FORMAT = "{0}";
		private const string CONNECTION_STRING_NAME_FORMAT = "{0}::ConnectionString";
		private const string DATA_SOURCE_TAG_FORMAT = "{0}::DataSourceTag";
		private const string RESOURCE_NAME_FORMAT = "{0}.SQL.RevisionHistory({1}).xml";

		private readonly DatabaseFileManager databaseFileManager;

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

		public DatabaseFileManager DatabaseFileManager
		{
			get
			{
				return this.databaseFileManager;
			}
		}

		#endregion

		#region Methods/Operators

		public abstract TModel CreateModel<TModel>() where TModel : class, IModelObject;

		public abstract TRequestModel CreateRequestModel<TRequestModel>() where TRequestModel : class, IRequestModelObject;

		public abstract TResponseModel CreateResponseModel<TResponseModel, TResultModel>()
			where TResponseModel : class, IResponseModelObject<TResultModel>
			where TResultModel : class, IResultModelObject;

		public abstract TResultModel CreateResultModel<TResultModel>() where TResultModel : class, IResultModelObject;

		public abstract TModel Discard<TModel>(TModel model) where TModel : class, IModelObject;

		public abstract TModel Discard<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

		public void EnsureDatabaseFile()
		{
			string resourceName;

			resourceName = string.Format(RESOURCE_NAME_FORMAT, this.GetType().Namespace, this.DataSourceTag.SafeToString().ToLower());

			this.DatabaseFileManager.InitializeFromRevisionHistoryResource(this, this.GetType(), resourceName);
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

		public IUnitOfWork GetUnitOfWork()
		{
			return UnitOfWork.Create(this.ConnectionType, this.ConnectionString, true);
		}

		public abstract TModel Load<TModel>(TModel prototype) where TModel : class, IModelObject;

		public abstract TModel Load<TModel>(IUnitOfWork unitOfWork, TModel prototype) where TModel : class, IModelObject;

		public abstract void OnDiscardConflictModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

		public abstract void OnPostDeleteModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

		public abstract void OnPostInsertModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

		public abstract void OnPostUpdateModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

		public abstract void OnPreDeleteModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

		public abstract void OnPreInsertModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

		protected abstract void OnPreProcessConnectionString(ref string connectionString);

		public abstract void OnPreUpdateModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

		public abstract void OnSaveConflictModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

		public abstract void OnSelectModel<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

		public abstract TModel Save<TModel>(TModel model) where TModel : class, IModelObject;

		public abstract TModel Save<TModel>(IUnitOfWork unitOfWork, TModel model) where TModel : class, IModelObject;

		#endregion
	}
}