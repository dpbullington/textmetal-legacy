/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;
using System.IO;

using Microsoft.Extensions.PlatformAbstractions;

using TextMetal.Middleware.Datazoid.UoW;
using TextMetal.Middleware.Solder.Runtime;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Middleware.Datazoid.Repositories
{
	public abstract class ModelRepository : IModelRepository
	{
		#region Constructors/Destructors

		protected ModelRepository(IDataTypeFascade dataTypeFascade, IAppConfigFascade appConfigFascade)
		{
			if ((object)dataTypeFascade == null)
				throw new ArgumentNullException(nameof(dataTypeFascade));

			if ((object)appConfigFascade == null)
				throw new ArgumentNullException(nameof(appConfigFascade));

			this.dataTypeFascade = dataTypeFascade;
			this.appConfigFascade = appConfigFascade;
		}

		#endregion

		#region Fields/Constants

		private const string CONNECTION_STRING_NAME_FORMAT = "{0}::ConnectionString";
		private const string DATABASE_DIRECTORY_PATH_FORMAT = "{0}::DatabaseDirectoryPath";
		private const string DATABASE_FILE_NAME_FORMAT = "{0}::DatabaseFileName";
		private const string KILL_DATABASE_FILE_FORMAT = "{0}::KillDatabaseFile";
		private const string USE_DATABASE_FILE_FORMAT = "{0}::UseDatabaseFile";
		private readonly IAppConfigFascade appConfigFascade;
		private readonly IDataTypeFascade dataTypeFascade;
		private bool forceEagerLoading;

		#endregion

		#region Properties/Indexers/Events

		protected IAppConfigFascade AppConfigFascade
		{
			get
			{
				return this.appConfigFascade;
			}
		}

		public string ConnectionString
		{
			get
			{
				string connectionString;

				connectionString = this.AppConfigFascade.GetAppSetting<string>(this.ConnectionStringName);

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
				return Type.GetType(this.AppConfigFascade.GetAppSetting<string>(this.ConnectionStringName), true);
			}
		}

		public string DatabaseDirectoryPath
		{
			get
			{
				return this.AppConfigFascade.GetAppSetting<string>(string.Format(DATABASE_DIRECTORY_PATH_FORMAT, this.GetType().Namespace));
			}
		}

		public string DatabaseFileName
		{
			get
			{
				return this.AppConfigFascade.GetAppSetting<string>(string.Format(DATABASE_FILE_NAME_FORMAT, this.GetType().Namespace));
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

		protected IDataTypeFascade DataTypeFascade
		{
			get
			{
				return this.dataTypeFascade;
			}
		}

		public bool KillDatabaseFile
		{
			get
			{
				bool value;

				if (!this.AppConfigFascade.HasAppSetting(string.Format(KILL_DATABASE_FILE_FORMAT, this.GetType().Namespace)))
					return false;

				value = this.AppConfigFascade.GetAppSetting<bool>(string.Format(KILL_DATABASE_FILE_FORMAT, this.GetType().Namespace));

				return value;
			}
		}

		public bool UseDatabaseFile
		{
			get
			{
				bool value;

				value = this.AppConfigFascade.GetAppSetting<bool>(string.Format(USE_DATABASE_FILE_FORMAT, this.GetType().Namespace));

				return value;
			}
		}

		public bool ForceEagerLoading
		{
			get
			{
				return this.forceEagerLoading;
			}
			set
			{
				this.forceEagerLoading = value;
			}
		}

		#endregion

		#region Methods/Operators

		private static string GetApplicationUserSpecificDirectoryPath()
		{
			IApplicationEnvironment applicationEnvironment;
			string userSpecificDirectoryPath;

			applicationEnvironment = AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<IApplicationEnvironment>(string.Empty, false);
			userSpecificDirectoryPath = Path.GetFullPath(applicationEnvironment.ApplicationBasePath);

			return userSpecificDirectoryPath;
		}

		private bool EnsureDatabaseFile()
		{
			string databaseFilePath;
			string databaseDirectoryPath;
			bool retval = false;

			if (this.UseDatabaseFile)
			{
				if (!this.DataTypeFascade.IsNullOrWhiteSpace(this.DatabaseFilePath))
				{
					if (this.KillDatabaseFile)
					{
						if (File.Exists(this.DatabaseFilePath))
							File.Delete(this.DatabaseFilePath);
					}

					databaseFilePath = Path.GetFullPath(this.DatabaseFilePath);
					databaseDirectoryPath = Path.GetDirectoryName(databaseFilePath);

					if (!Directory.Exists(databaseDirectoryPath))
						Directory.CreateDirectory(databaseDirectoryPath);

					if (!File.Exists(databaseFilePath))
						retval = this.OnCreateNativeDatabaseFile(databaseFilePath);
				}
			}

			return retval;
		}

		public virtual IUnitOfWork GetUnitOfWork(bool transactional, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
		{
			return UnitOfWork.Create(this.ConnectionType, this.ConnectionString, transactional, isolationLevel);
		}

		protected abstract bool OnCreateNativeDatabaseFile(string databaseFilePath);

		protected virtual void OnPreProcessConnectionString(ref string connectionString)
		{
			if (!this.UseDatabaseFile)
				return;

			if ((object)connectionString == null)
				throw new ArgumentNullException(nameof(connectionString));

			// {0} = this.DatabaseFilePath
			connectionString = string.Format(connectionString ?? string.Empty, this.DatabaseFilePath);
		}

		#endregion
	}
}