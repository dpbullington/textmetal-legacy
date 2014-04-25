/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TextMetal.Framework.SourceModel.DatabaseSchema
{
	[Serializable]
	[XmlRoot(ElementName = "Server", Namespace = "http://www.textmetal.com/api/v6.0.0")]
	public class Server : DatabaseSchemaModelBase
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
		private readonly List<Schema> schemas = new List<Schema>();
		private readonly List<Trigger> triggers = new List<Trigger>();
		private string connectionString;
		private string connectionType;
		private string defaultDatabaseName;
		private string instanceName;
		private string machineName;
		private string serverEdition;
		private string serverLevel;
		private string serverVersion;

		#endregion

		#region Properties/Indexers/Events

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

		[XmlIgnore]
		public bool HasProcedures
		{
			get
			{
				return this.Schemas.Count(s => s.Procedures.Count() > 0) > 0;
			}
		}

		[XmlIgnore]
		public bool HasTables
		{
			get
			{
				return this.Schemas.Count(s => s.Tables.Count(t => !t.IsView) > 0) > 0;
			}
		}

		[XmlIgnore]
		public bool HasViews
		{
			get
			{
				return this.Schemas.Count(s => s.Tables.Count(t => t.IsView) > 0) > 0;
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

		[XmlArray(ElementName = "Schemas")]
		[XmlArrayItem(ElementName = "Schema")]
		public List<Schema> Schemas
		{
			get
			{
				return this.schemas;
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

		[XmlIgnore]
		public string ServerName
		{
			get
			{
				if (!string.IsNullOrEmpty(this.MachineName) &&
					!string.IsNullOrEmpty(this.MachineName))
					return string.Format("{0}\\{1}", this.MachineName, this.InstanceName);
				else
					return this.MachineName;
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

		[XmlArray(ElementName = "Triggers")]
		[XmlArrayItem(ElementName = "Trigger")]
		public List<Trigger> Triggers
		{
			get
			{
				return this.triggers;
			}
		}

		#endregion
	}
}