/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TextMetal.Framework.Source.DatabaseSchema
{
	[XmlRoot(ElementName = "Server", Namespace = "http://www.textmetal.com/api/v6.0.0")]
	public class Server
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the Server class.
		/// </summary>
		public Server()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<Database> databases = new List<Database>();
		private string connectionString;
		private string connectionType;
		private string defaultDatabaseName;
		private string instanceName;
		private string machineName;
		private string serverEdition;
		private string serverLevel;
		private string serverName;
		private string serverVersion;

		#endregion

		#region Properties/Indexers/Events

		[XmlArray(ElementName = "Databases")]
		[XmlArrayItem(ElementName = "Database")]
		public List<Database> Databases
		{
			get
			{
				return this.databases;
			}
		}

		[XmlIgnore]
		public Database DefaultDatabase
		{
			get
			{
				return this.Databases.FirstOrDefault(c => c.DatabaseName == this.DefaultDatabaseName);
			}
		}

		[XmlIgnore]
		public bool HasDatabases
		{
			get
			{
				return this.Databases.Count() > 0;
			}
		}

		[XmlIgnore]
		public bool HasDefaultDatabase
		{
			get
			{
				return (object)this.DefaultDatabase != null;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool HasProcedures
		{
			get
			{
				return this.Schemas.Count(s => s.Procedures.Any()) > 0;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool HasTables
		{
			get
			{
				return this.Schemas.Count(s => s._Tables.Any()) > 0;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool HasViews
		{
			get
			{
				return this.Schemas.Count(s => s.Views.Any()) > 0;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public List<Schema> Schemas
		{
			get
			{
				return this.Databases.SingleOrDefault(db => db.DatabaseName == this.DefaultDatabaseName).Schemas;
			}
		}

		[XmlAttribute]
		public string ConnectionString
		{
			get
			{
				return this.connectionString;
			}
			set
			{
				this.connectionString = value;
			}
		}

		[XmlAttribute]
		public string ConnectionType
		{
			get
			{
				return this.connectionType;
			}
			set
			{
				this.connectionType = value;
			}
		}

		[XmlAttribute]
		public string DefaultDatabaseName
		{
			get
			{
				return this.defaultDatabaseName;
			}
			set
			{
				this.defaultDatabaseName = value;
			}
		}

		[XmlAttribute]
		public string InstanceName
		{
			get
			{
				return this.instanceName;
			}
			set
			{
				this.instanceName = value;
			}
		}

		[XmlAttribute]
		public string MachineName
		{
			get
			{
				return this.machineName;
			}
			set
			{
				this.machineName = value;
			}
		}

		[XmlAttribute]
		public string ServerEdition
		{
			get
			{
				return this.serverEdition;
			}
			set
			{
				this.serverEdition = value;
			}
		}

		[XmlAttribute]
		public string ServerLevel
		{
			get
			{
				return this.serverLevel;
			}
			set
			{
				this.serverLevel = value;
			}
		}

		[XmlAttribute]
		public string ServerName
		{
			get
			{
				return this.serverName;
			}
			set
			{
				this.serverName = value;
			}
		}

		[XmlAttribute]
		public string ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
			set
			{
				this.serverVersion = value;
			}
		}

		#endregion
	}
}