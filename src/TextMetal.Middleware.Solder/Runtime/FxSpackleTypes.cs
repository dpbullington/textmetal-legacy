using System;

namespace TextMetal.Middleware.Solder.Runtime
{
	public static class FxSpackleTypes
	{
		#region Classes/Structs/Interfaces/Enums/Delegates

		internal sealed class AppSettings
		{
			#region Properties/Indexers/Events

			public string this[string key]
			{
				get
				{
					return null;
				}
			}

			#endregion
		}

		/// <summary>
		/// The exception thrown when a configuration error occurs.
		/// </summary>
		public sealed class ConfigurationErrorsException : Exception
		{
			#region Constructors/Destructors

			/// <summary>
			/// Initializes a new instance of the ConfigurationErrorsException class.
			/// </summary>
			public ConfigurationErrorsException()
			{
			}

			/// <summary>
			/// Initializes a new instance of the ConfigurationErrorsException class.
			/// </summary>
			/// <param name="message"> The message that describes the error. </param>
			public ConfigurationErrorsException(string message)
				: base(message)
			{
			}

			/// <summary>
			/// Initializes a new instance of the ConfigurationErrorsException class.
			/// </summary>
			/// <param name="message"> The message that describes the error. </param>
			/// <param name="innerException"> The inner exception. </param>
			public ConfigurationErrorsException(string message, Exception innerException)
				: base(message, innerException)
			{
			}

			#endregion
		}

		internal sealed class ConfigurationManager
		{
			#region Fields/Constants

			private static AppSettings appSettings = new AppSettings();
			private static ConnectionStrings connectionStrings = new ConnectionStrings();

			#endregion

			#region Properties/Indexers/Events

			public static AppSettings AppSettings
			{
				get
				{
					return appSettings;
				}
			}

			public static ConnectionStrings ConnectionStrings
			{
				get
				{
					return connectionStrings;
				}
			}

			#endregion
		}

		internal sealed class ConnectionStrings
		{
			#region Properties/Indexers/Events

			public ConnectionStringSettings this[string key]
			{
				get
				{
					return null;
				}
			}

			#endregion
		}

		internal sealed class ConnectionStringSettings
		{
			#region Fields/Constants

			private string connectionString;
			private string providerName;

			#endregion

			#region Properties/Indexers/Events

			public string ConnectionString
			{
				get
				{
					return this.connectionString;
				}
			}

			public string ProviderName
			{
				get
				{
					return this.providerName;
				}
			}

			#endregion
		}

		[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
		public sealed class DescriptionAttribute : Attribute
		{
			#region Constructors/Destructors

			/// <summary>
			/// Initializes a new instance of the DescriptionAttribute class.
			/// </summary>
			public DescriptionAttribute(string value)
			{
			}

			#endregion
		}

		public static class SchemaTableColumn
		{
			#region Properties/Indexers/Events

			public static string AllowDBNull
			{
				get;
				set;
			}

			public static string ColumnName
			{
				get;
				set;
			}

			public static string ColumnOrdinal
			{
				get;
				set;
			}

			public static string ColumnSize
			{
				get;
				set;
			}

			public static string DataType
			{
				get;
				set;
			}

			public static string NumericPrecision
			{
				get;
				set;
			}

			public static string NumericScale
			{
				get;
				set;
			}

			#endregion
		}

		[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
		public sealed class SerializableAttribute : Attribute
		{
			#region Constructors/Destructors

			/// <summary>
			/// Initializes a new instance of the SerializableAttribute class.
			/// </summary>
			public SerializableAttribute()
			{
			}

			#endregion
		}

		#endregion
	}
}