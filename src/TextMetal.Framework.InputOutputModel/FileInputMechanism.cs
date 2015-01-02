/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using TextMetal.Common.Core;
using TextMetal.Common.Xml;
using TextMetal.Framework.Core;

namespace TextMetal.Framework.InputOutputModel
{
	public class FileInputMechanism : InputMechanism
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the FileInputMechanism class.
		/// </summary>
		/// <param name="baseDirectoryPath"> The base input directory path. </param>
		/// <param name="xpe"> The XML persist engine in-effect. </param>
		/// <param name="sourceStrategy"> The source strategy in-effect. </param>
		public FileInputMechanism(string baseDirectoryPath, IXmlPersistEngine xpe, ISourceStrategy sourceStrategy)
		{
			if ((object)baseDirectoryPath == null)
				throw new ArgumentNullException("baseDirectoryPath");

			if ((object)xpe == null)
				throw new ArgumentNullException("xpe");

			if ((object)sourceStrategy == null)
				throw new ArgumentNullException("sourceStrategy");

			if (!Path.HasExtension(baseDirectoryPath) &&
				!baseDirectoryPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
				baseDirectoryPath = baseDirectoryPath + Path.DirectorySeparatorChar;

			this.baseDirectoryPath = Path.GetDirectoryName(Path.GetFullPath(baseDirectoryPath));
			this.xpe = xpe;
			this.sourceStrategy = sourceStrategy;
		}

		#endregion

		#region Fields/Constants

		private readonly string baseDirectoryPath;
		private readonly ISourceStrategy sourceStrategy;
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

		private ISourceStrategy SourceStrategy
		{
			get
			{
				return this.sourceStrategy;
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

		protected override Assembly CoreLoadAssembly(string assemblyName)
		{
			Assembly assembly;

			if ((object)assemblyName == null)
				throw new ArgumentNullException("assemblyName");

			if (DataType.Instance.IsWhiteSpace(assemblyName))
				throw new ArgumentOutOfRangeException("assemblyName");

			assemblyName = Path.GetFullPath(assemblyName);
			assembly = Assembly.LoadFile(assemblyName);

			return assembly;
		}

		protected override string CoreLoadContent(string contentName)
		{
			string fullFilePath;
			string value;

			if ((object)contentName == null)
				throw new ArgumentNullException("contentName");

			if (DataType.Instance.IsWhiteSpace(contentName))
				throw new ArgumentOutOfRangeException("contentName");

			fullFilePath = Path.GetFullPath(Path.Combine(this.BaseDirectoryPath, contentName));
			//Console.Error.WriteLine(fullFilePath);

			using (Stream stream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (TextReader textReader = new StreamReader(stream))
					value = textReader.ReadToEnd();
			}

			return value;
		}

		protected override object CoreLoadSource(string sourceName, IDictionary<string, IList<string>> properties)
		{
			//string fullFilePath;
			object value;

			if ((object)sourceName == null)
				throw new ArgumentNullException("sourceName");

			if ((object)properties == null)
				throw new ArgumentNullException("properties");

			if (DataType.Instance.IsWhiteSpace(sourceName))
				throw new ArgumentOutOfRangeException("sourceName");

			//fullFilePath = Path.GetFullPath(Path.Combine(this.BaseDirectoryPath, sourceName));
			//Console.Error.WriteLine(fullFilePath);

			// pass-thru the source name without a resolution...let the source strategy decide
			value = this.sourceStrategy.GetSourceObject(sourceName, properties);

			return value;
		}

		protected override ITemplateXmlObject CoreLoadTemplate(string templateName)
		{
			string fullFilePath;
			ITemplateXmlObject value;

			if ((object)templateName == null)
				throw new ArgumentNullException("templateName");

			if (DataType.Instance.IsWhiteSpace(templateName))
				throw new ArgumentOutOfRangeException("templateName");

			fullFilePath = Path.GetFullPath(Path.Combine(this.BaseDirectoryPath, templateName));
			//Console.Error.WriteLine(fullFilePath);

			value = (ITemplateXmlObject)this.Xpe.DeserializeFromXml(fullFilePath);

			return value;
		}

		#endregion
	}
}