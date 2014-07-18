/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Text;

using TextMetal.Common.Cerealization;
using TextMetal.Common.Core;
using TextMetal.Common.Xml;

namespace TextMetal.Framework.InputOutputModel
{
	public class FileOutputMechanism : OutputMechanism
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the FileOutputMechanism class.
		/// </summary>
		/// <param name="baseDirectoryPath"> The base output directory path. </param>
		/// <param name="logFileName"> The file name of the log file (relative to base directory path) or empty string for console output. </param>
		/// <param name="xpe"> The XML persist engine in-effect. </param>
		public FileOutputMechanism(string baseDirectoryPath, string logFileName, IXmlPersistEngine xpe)
		{
			if ((object)baseDirectoryPath == null)
				throw new ArgumentNullException("baseDirectoryPath");

			if ((object)logFileName == null)
				throw new ArgumentNullException("logFileName");

			if ((object)xpe == null)
				throw new ArgumentNullException("xpe");

			this.baseDirectoryPath = baseDirectoryPath;
			this.logFileName = logFileName;
			this.xpe = xpe;

			this.SetupLogger();
		}

		#endregion

		#region Fields/Constants

		private readonly string baseDirectoryPath;
		private readonly string logFileName;
		private readonly IXmlPersistEngine xpe;

		#endregion

		#region Properties/Indexers/Events

		private string BaseDirectoryPath
		{
			get
			{
				return this.baseDirectoryPath;
			}
		}

		private string LogFileName
		{
			get
			{
				return this.logFileName;
			}
		}

		private IXmlPersistEngine Xpe
		{
			get
			{
				return this.xpe;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreEnter(string scopeName, bool appendMode)
		{
			FileStream stream;
			TextWriter textWriter;
			string fullFilePath;
			string fullDirectoryPath;

			if ((object)scopeName == null)
				throw new ArgumentNullException("scopeName");

			if (DataType.IsWhiteSpace(scopeName))
				throw new ArgumentOutOfRangeException("scopeName");

			fullFilePath = Path.GetFullPath(Path.Combine(this.BaseDirectoryPath, scopeName));
			fullDirectoryPath = Path.GetDirectoryName(fullFilePath);
			//Console.Error.WriteLine(fullFilePath);

			if (!Directory.Exists(fullDirectoryPath))
				Directory.CreateDirectory(fullDirectoryPath);

			// do not dispose here!
			stream = new FileStream(fullFilePath, appendMode ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.None);
			textWriter = new StreamWriter(stream, Encoding.UTF8);

			base.TextWriters.Push(textWriter);
		}

		protected override void CoreLeave(string scopeName)
		{
			TextWriter textWriter;

			if ((object)scopeName == null)
				throw new ArgumentNullException("scopeName");

			if (DataType.IsWhiteSpace(scopeName))
				throw new ArgumentOutOfRangeException("scopeName");

			textWriter = base.TextWriters.Pop();
			textWriter.Flush();
			textWriter.Dispose();
		}

		protected override void CoreWriteObject(object obj, string objectName)
		{
			string fullFilePath;
			IXmlObject xmlObject;
			ISerializationStrategy serializationStrategy;

			if ((object)obj == null)
				throw new ArgumentNullException("obj");

			if ((object)objectName == null)
				throw new ArgumentNullException("objectName");

			if (DataType.IsWhiteSpace(objectName))
				throw new ArgumentOutOfRangeException("objectName");

			fullFilePath = Path.GetFullPath(Path.Combine(this.BaseDirectoryPath, objectName));
			xmlObject = obj as IXmlObject;

			// this should support XPE, XML, JSON
			if ((object)xmlObject != null)
				serializationStrategy = new XpeSerializationStrategy(this.Xpe);
			else if ((object)Reflexion.GetOneAttribute<SerializableAttribute>(obj.GetType()) != null)
				serializationStrategy = new XmlSerializationStrategy();
			else
				serializationStrategy = new JsonSerializationStrategy();

			serializationStrategy.SetObjectToFile(fullFilePath, obj);
		}

		private void SetupLogger()
		{
			FileStream stream;
			TextWriter textWriter = null;
			string fullFilePath;
			string fullDirectoryPath;

			if (!DataType.IsWhiteSpace(this.LogFileName))
			{
				fullFilePath = Path.GetFullPath(Path.Combine(this.BaseDirectoryPath, this.LogFileName));
				fullDirectoryPath = Path.GetDirectoryName(fullFilePath);
				//Console.Error.WriteLine(fullFilePath);

				if (!Directory.Exists(fullDirectoryPath))
					Directory.CreateDirectory(fullDirectoryPath);

				// do not dispose here!
				stream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
				textWriter = new StreamWriter(stream, Encoding.UTF8);
			}

			base.SetLogTextWriter(textWriter);
		}

		#endregion
	}
}