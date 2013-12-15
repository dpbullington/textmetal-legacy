// ****************************************************************
// Copyright 2012, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

using NUnit.Core;
using NUnit.Util;

namespace NUnit.UiKit
{
	public class TextDisplayContent
	{
		#region Constructors/Destructors

		public TextDisplayContent()
		{
		}

		#endregion

		#region Fields/Constants

		private char[] content = new char[] { '0', '0', '0', '0', '0' };

		#endregion

		#region Properties/Indexers/Events

		public bool Error
		{
			get
			{
				return this.content[1] == '1';
			}
			set
			{
				this.content[1] = value ? '1' : '0';
			}
		}

		public TestLabelLevel Labels
		{
			get
			{
				return (TestLabelLevel)(this.content[4] - '0');
			}
			set
			{
				this.content[4] = (char)((int)value + '0');
			}
		}

		public LoggingThreshold LogLevel
		{
			get
			{
				return (LoggingThreshold)(this.content[3] - '0');
			}
			set
			{
				this.content[3] = (char)((int)value + '0');
			}
		}

		public bool Out
		{
			get
			{
				return this.content[0] == '1';
			}
			set
			{
				this.content[0] = value ? '1' : '0';
			}
		}

		public bool Trace
		{
			get
			{
				return this.content[2] == '1';
			}
			set
			{
				this.content[2] = value ? '1' : '0';
			}
		}

		#endregion

		#region Methods/Operators

		public static TextDisplayContent FromSettings(string name)
		{
			TextDisplayContent content = new TextDisplayContent();
			content.LoadSettings(name);
			return content;
		}

		public void LoadSettings(string name)
		{
			ISettings settings = Services.UserSettings;
			string prefix = "Gui.TextOutput." + name;

			string rep = settings.GetSetting(prefix + ".Content", "00000");

			// Assume new format but if it isn't try the old one
			if (!this.LoadUsingNewFormat(rep))
				this.LoadUsingOldFormat(rep);
		}

		private bool LoadUsingNewFormat(string rep)
		{
			if (rep.Length != 5)
				return false;

			foreach (char c in rep)
			{
				if (!char.IsDigit(c))
					return false;
			}

			this.content = rep.ToCharArray();

			return true;
		}

		private void LoadUsingOldFormat(string content)
		{
			ContentType contentType = (ContentType)Enum.Parse(typeof(ContentType), content, false);
			this.Out = (contentType & ContentType.Out) != 0;
			this.Error = (contentType & ContentType.Error) != 0;
			this.Trace = (contentType & ContentType.Trace) != 0;
			this.LogLevel = (contentType & ContentType.Log) != 0
				? LoggingThreshold.All
				: LoggingThreshold.Off;
			this.Labels = (contentType & ContentType.Labels) != 0
				? (contentType & ContentType.LabelOnlyOnOutput) != 0
					? TestLabelLevel.All
					: TestLabelLevel.On
				: TestLabelLevel.Off;
		}

		public void SaveSettings(string name)
		{
			ISettings settings = Services.UserSettings;
			string prefix = "Gui.TextOutput." + name;

			settings.SaveSetting(prefix + ".Content", new string(this.content));
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		[Flags]
		private enum ContentType
		{
			Empty = 0,
			Out = 1,
			Error = 2,
			Trace = 4,
			Log = 8,
			Labels = 64,
			LabelOnlyOnOutput = 128
		}

		#endregion
	}
}