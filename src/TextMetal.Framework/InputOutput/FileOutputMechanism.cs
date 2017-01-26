/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using TextMetal.Framework.XmlDialect;
using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Serialization;

namespace TextMetal.Framework.InputOutput
{
	public class FileOutputMechanism : OutputMechanism
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the FileOutputMechanism class.
		/// </summary>
		/// <param name="baseDirectoryPath"> The base output directory path. </param>
		/// <param name="logFileName"> The file name of the log file (relative to base directory path) or empty string for console output. </param>
		/// <param name="logFileEncoding"> The encoding of the log file. </param>
		/// <param name="xpe"> The XML persist engine in-effect. </param>
		public FileOutputMechanism(string baseDirectoryPath, string logFileName, Encoding logFileEncoding, IXmlPersistEngine xpe)
		{
			if ((object)baseDirectoryPath == null)
				throw new ArgumentNullException(nameof(baseDirectoryPath));

			if ((object)logFileName == null)
				throw new ArgumentNullException(nameof(logFileName));

			if ((object)logFileEncoding == null)
				throw new ArgumentNullException(nameof(logFileEncoding));

			if ((object)xpe == null)
				throw new ArgumentNullException(nameof(xpe));

			if (!Path.HasExtension(baseDirectoryPath) &&
				!baseDirectoryPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
				baseDirectoryPath = baseDirectoryPath + Path.DirectorySeparatorChar;

			this.baseDirectoryPath = Path.GetDirectoryName(Path.GetFullPath(baseDirectoryPath));
			this.logFileName = logFileName;
			this.logFileEncoding = logFileEncoding;
			this.xpe = xpe;

			this.SetupLogger();
			this.EnsureOutputDirectory(false);
		}

		#endregion

		#region Fields/Constants

		private readonly string baseDirectoryPath;
		private readonly Encoding logFileEncoding;
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

		private Encoding LogFileEncoding
		{
			get
			{
				return this.logFileEncoding;
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

		protected override void CoreEnter(string scopeName, bool appendMode, Encoding encoding)
		{
			FileStream stream;
			TextWriter textWriter;
			string fullFilePath;
			string fullDirectoryPath;

			if ((object)scopeName == null)
				throw new ArgumentNullException(nameof(scopeName));

			if (SolderFascadeAccessor.DataTypeFascade.IsWhiteSpace(scopeName))
				throw new ArgumentOutOfRangeException(nameof(scopeName));

			fullFilePath = Path.GetFullPath(Path.Combine(this.BaseDirectoryPath, scopeName));
			fullDirectoryPath = Path.GetDirectoryName(fullFilePath);
			//Console.Error.WriteLine(fullFilePath);

			if (!Directory.Exists(fullDirectoryPath))
				Directory.CreateDirectory(fullDirectoryPath);

			// do not dispose here!
			stream = new FileStream(fullFilePath, appendMode ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.None);
			textWriter = new StreamWriter(stream, encoding ?? this.LogFileEncoding);

			this.TextWriters.Push(textWriter);
		}

		protected override void CoreLeave(string scopeName)
		{
			TextWriter textWriter;

			if ((object)scopeName == null)
				throw new ArgumentNullException(nameof(scopeName));

			if (SolderFascadeAccessor.DataTypeFascade.IsWhiteSpace(scopeName))
				throw new ArgumentOutOfRangeException(nameof(scopeName));

			textWriter = this.TextWriters.Pop();
			textWriter.Flush();
			textWriter.Dispose();
		}

		protected override void CoreWriteObject(object obj, string objectName)
		{
			string fullFilePath;
			IXmlObject xmlObject;
			ISerializationStrategy serializationStrategy;

			if ((object)obj == null)
				throw new ArgumentNullException(nameof(obj));

			if ((object)objectName == null)
				throw new ArgumentNullException(nameof(objectName));

			if (SolderFascadeAccessor.DataTypeFascade.IsWhiteSpace(objectName))
				throw new ArgumentOutOfRangeException(nameof(objectName));

			fullFilePath = Path.GetFullPath(Path.Combine(this.BaseDirectoryPath, objectName));
			xmlObject = obj as IXmlObject;

			// this should support XPE, XML, JSON
			/*
				BACKLOG(dpbullington@gmail.com / 2015 - 12 - 18):
				Refactor this logic that is common between this File and TextWriter Outputs.
			*/
			if ((object)xmlObject != null)
				serializationStrategy = new XpeSerializationStrategy(this.Xpe);
			else if ((object)SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlRootAttribute>(obj.GetType()) != null)
				serializationStrategy = new XmlSerializationStrategy();
			else
				serializationStrategy = new JsonSerializationStrategy();

			serializationStrategy.SetObjectToFile(fullFilePath, obj);
		}

		private void EnsureOutputDirectory(bool kill)
		{
			if (!kill)
			{
				if (!Directory.Exists(this.baseDirectoryPath))
					Directory.CreateDirectory(this.baseDirectoryPath);
			}
			else
			{
				if (!Directory.Exists(this.baseDirectoryPath))
					Directory.Delete(this.baseDirectoryPath, true);

				Directory.CreateDirectory(this.baseDirectoryPath);
			}
		}

		private void SetupLogger()
		{
			FileStream stream;
			TextWriter textWriter = null;
			string fullFilePath;
			string fullDirectoryPath;

			if (!SolderFascadeAccessor.DataTypeFascade.IsWhiteSpace(this.LogFileName))
			{
				fullFilePath = Path.GetFullPath(Path.Combine(this.BaseDirectoryPath, this.LogFileName));
				fullDirectoryPath = Path.GetDirectoryName(fullFilePath);
				//Console.Error.WriteLine(fullFilePath);

				if (!Directory.Exists(fullDirectoryPath))
					Directory.CreateDirectory(fullDirectoryPath);

				// do not dispose here!
				stream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
				textWriter = new StreamWriter(stream, this.LogFileEncoding);
			}

			this.SetLogTextWriter(textWriter);
		}

		#endregion
	}
}