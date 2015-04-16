/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;
using System.IO;
using System.Reflection;

using TextMetal.Middleware.Common.Fascades.AdoNet.UoW;
using TextMetal.Middleware.Common.Fascades.Utilities;
using TextMetal.Middleware.Common.Strategies.ContextualStorage;

namespace TextMetal.Middleware.Data.Models
{
	public abstract class ModelRepository : IModelRepository
	{
		#region Constructors/Destructors

		protected ModelRepository()
		{
		}

		#endregion

		#region Fields/Constants

		private const string CONNECTION_STRING_NAME_FORMAT = "{0}::ConnectionString";
		private const string DATABASE_DIRECTORY_PATH_FORMAT = "{0}::DatabaseDirectoryPath";
		private const string DATABASE_FILE_NAME_FORMAT = "{0}::DatabaseFileName";
		private const string KILL_DATABASE_FILE_FORMAT = "{0}::KillDatabaseFile";
		private const string USE_DATABASE_FILE_FORMAT = "{0}::UseDatabaseFile";
		private bool forceEagerLoading;

		#endregion

		#region Properties/Indexers/Events

		public string ConnectionString
		{
			get
			{
				string connectionString;

				connectionString = AppConfigFascade.Instance.GetConnectionString(this.ConnectionStringName);

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
				return Type.GetType(AppConfigFascade.Instance.GetConnectionProvider(this.ConnectionStringName), true);
			}
		}

		public string DatabaseDirectoryPath
		{
			get
			{
				return AppConfigFascade.Instance.GetAppSetting<string>(string.Format(DATABASE_DIRECTORY_PATH_FORMAT, this.GetType().Namespace));
			}
		}

		public string DatabaseFileName
		{
			get
			{
				return AppConfigFascade.Instance.GetAppSetting<string>(string.Format(DATABASE_FILE_NAME_FORMAT, this.GetType().Namespace));
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

		public bool KillDatabaseFile
		{
			get
			{
				bool value;

				if (!AppConfigFascade.Instance.HasAppSetting(string.Format(KILL_DATABASE_FILE_FORMAT, this.GetType().Namespace)))
					return false;

				value = AppConfigFascade.Instance.GetAppSetting<bool>(string.Format(KILL_DATABASE_FILE_FORMAT, this.GetType().Namespace));

				return value;
			}
		}

		public bool UseDatabaseFile
		{
			get
			{
				bool value;

				value = AppConfigFascade.Instance.GetAppSetting<bool>(string.Format(USE_DATABASE_FILE_FORMAT, this.GetType().Namespace));

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
			Assembly assembly;
			AssemblyInformationFascade assemblyInformationFascade;
			string userSpecificDirectoryPath;

			if (HttpContextContextualStorageStrategy.IsInHttpContext)
				userSpecificDirectoryPath = Path.GetFullPath(HttpContextContextualStorageStrategy.GetApplicationRootPhysicalPath());
			else
			{
				assembly = Assembly.GetExecutingAssembly();
				assemblyInformationFascade = new AssemblyInformationFascade(assembly);

				if ((object)assemblyInformationFascade.Company != null &&
					(object)assemblyInformationFascade.Product != null &&
					(object)assemblyInformationFascade.Win32FileVersion != null)
				{
					userSpecificDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
					userSpecificDirectoryPath = Path.Combine(userSpecificDirectoryPath, assemblyInformationFascade.Company);
					userSpecificDirectoryPath = Path.Combine(userSpecificDirectoryPath, assemblyInformationFascade.Product);
					userSpecificDirectoryPath = Path.Combine(userSpecificDirectoryPath, assemblyInformationFascade.Win32FileVersion);
				}
				else
					userSpecificDirectoryPath = Path.GetFullPath(".");
			}

			return userSpecificDirectoryPath;
		}

		private bool EnsureDatabaseFile()
		{
			string databaseFilePath;
			string databaseDirectoryPath;
			bool retval = false;

			if (this.UseDatabaseFile)
			{
				if (!DataTypeFascade.Instance.IsNullOrWhiteSpace(this.DatabaseFilePath))
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
				throw new ArgumentNullException("connectionString");

			// {0} = this.DatabaseFilePath
			connectionString = string.Format(connectionString ?? string.Empty, this.DatabaseFilePath);
		}

		#endregion
	}
}