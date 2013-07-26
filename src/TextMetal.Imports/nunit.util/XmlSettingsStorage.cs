// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace NUnit.Util
{
	/// <summary>
	/// 	Summary description for XmlSettingsStorage.
	/// </summary>
	public class XmlSettingsStorage : MemorySettingsStorage
	{
		#region Constructors/Destructors

		public XmlSettingsStorage(string filePath)
			: this(filePath, true)
		{
		}

		public XmlSettingsStorage(string filePath, bool writeable)
		{
			this.filePath = filePath;
			this.writeable = writeable;
		}

		#endregion

		#region Fields/Constants

		private string filePath;
		private bool writeable;

		#endregion

		#region Methods/Operators

		public override void LoadSettings()
		{
			FileInfo info = new FileInfo(this.filePath);
			if (!info.Exists || info.Length == 0)
				return;

			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(this.filePath);

				foreach (XmlElement element in doc.DocumentElement["Settings"].ChildNodes)
				{
					if (element.Name != "Setting")
						throw new ApplicationException("Unknown element in settings file: " + element.Name);

					if (!element.HasAttribute("name"))
						throw new ApplicationException("Setting must have 'name' attribute");

					if (!element.HasAttribute("value"))
						throw new ApplicationException("Setting must have 'value' attribute");

					this.settings[element.GetAttribute("name")] = element.GetAttribute("value");
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error loading settings file", ex);
			}
		}

		public override void SaveSettings()
		{
			if (!this.writeable)
				throw new InvalidOperationException("Attempted to write to a non-writeable Settings Storage");

			string dirPath = Path.GetDirectoryName(this.filePath);
			if (!Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);

			XmlTextWriter writer = new XmlTextWriter(this.filePath, Encoding.UTF8);
			writer.Formatting = Formatting.Indented;

			writer.WriteProcessingInstruction("xml", "version=\"1.0\"");
			writer.WriteStartElement("NUnitSettings");
			writer.WriteStartElement("Settings");

			ArrayList keys = new ArrayList(this.settings.Keys);
			keys.Sort();

			foreach (string name in keys)
			{
				object val = this.settings[name];
				if (val != null)
				{
					writer.WriteStartElement("Setting");
					writer.WriteAttributeString("name", name);
					writer.WriteAttributeString("value", val.ToString());
					writer.WriteEndElement();
				}
			}

			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.Close();
		}

		#endregion
	}
}