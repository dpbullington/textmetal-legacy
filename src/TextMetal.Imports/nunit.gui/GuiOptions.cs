// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using Codeblast;

namespace NUnit.Gui
{
	using System;

	public class GuiOptions : CommandLineOptions
	{
		#region Constructors/Destructors

		public GuiOptions(String[] args)
			: base(args)
		{
		}

		#endregion

		#region Fields/Constants

		[Option(Description = "Erase any leftover cache files and exit")]
		public bool cleanup;

		[Option(Description = "Project configuration to load")]
		public string config;

		[Option(Description = "Create console display for viewing any unmanaged output")]
		public bool console;

		[Option(Description = "List of categories to exclude")]
		public string exclude;

		[Option(Description = "Fixture to test")]
		public string fixture;

		[Option(Short = "?", Description = "Display help")]
		public bool help = false;

		[Option(Description = "List of categories to include")]
		public string include;

		[Option(Description = "Language to use for the NUnit GUI")]
		public string lang;

		[Option(Description = "Suppress loading of last project")]
		public bool noload;

		[Option(Description = "Automatically run the loaded project")]
		public bool run;

		[Option(Description = "Automatically run selected tests or all tests if none are selected")]
		public bool runselected;

		#endregion

		#region Properties/Indexers/Events

		private bool HasExclude
		{
			get
			{
				return this.exclude != null && this.exclude.Length != 0;
			}
		}

		private bool HasInclude
		{
			get
			{
				return this.include != null && this.include.Length != 0;
			}
		}

		#endregion

		#region Methods/Operators

		public override string GetHelpText()
		{
			return
				"NUNIT [inputfile] [options]\r\r" +
				"Runs a set of NUnit tests from the console. You may specify\r" +
				"an assembly or a project file of type .nunit as input.\r\r" +
				"Options:\r" +
				base.GetHelpText() +
				"\rOptions that take values may use an equal sign, a colon\r" +
				"or a space to separate the option from its value.";
		}

		public bool Validate()
		{
			if (this.isInvalid)
				return false;

			if (this.HasInclude && this.HasExclude)
				return false;

			return this.NoArgs || this.ParameterCount <= 1;
		}

		#endregion
	}
}