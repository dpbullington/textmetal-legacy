/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Cerealization;

namespace TextMetal.HostImpl.SsisGenPkg.ConsoleTool.Config
{
	public class Configuration
	{
		#region Constructors/Destructors

		public Configuration()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<DataTransfer> objects = new List<DataTransfer>();
		private bool? validateExternalMetadata;

		#endregion

		#region Properties/Indexers/Events

		public List<DataTransfer> Objects
		{
			get
			{
				return this.objects;
			}
		}

		public bool? ValidateExternalMetadata
		{
			get
			{
				return this.validateExternalMetadata;
			}
			set
			{
				this.validateExternalMetadata = value;
			}
		}

		#endregion

		#region Methods/Operators

		public static Configuration FromJsonFile(string jsonFile)
		{
			Configuration configuration;

			configuration = new JsonSerializationStrategy().GetObjectFromFile<Configuration>(jsonFile);

			return configuration;
		}

		public static void ToJsonFile(Configuration configuration, string jsonFile)
		{
			new JsonSerializationStrategy().SetObjectToFile<Configuration>(jsonFile, configuration);
		}

		#endregion
	}
}