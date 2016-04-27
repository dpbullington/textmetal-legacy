/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using TextMetal.Framework.Source;
using TextMetal.Framework.Template;
using TextMetal.Framework.XmlDialect;
using TextMetal.Middleware.Solder.Runtime;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Framework.InputOutput
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
				throw new ArgumentNullException(nameof(baseDirectoryPath));

			if ((object)xpe == null)
				throw new ArgumentNullException(nameof(xpe));

			if ((object)sourceStrategy == null)
				throw new ArgumentNullException(nameof(sourceStrategy));

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
			AssemblyName _assemblyName;

			//Console.WriteLine(this.GetType().GetTypeInfo().Assembly.GetName().Name);

			if ((object)assemblyName == null)
				throw new ArgumentNullException(nameof(assemblyName));

			if (ExtensionMethods.DataTypeFascadeLegacyInstance.IsWhiteSpace(assemblyName))
				throw new ArgumentOutOfRangeException(nameof(assemblyName));

			//assemblyName = Path.GetFullPath(assemblyName);
			_assemblyName = new AssemblyName(assemblyName);
			assembly = AssemblyLoaderContainerContext.TheOnlyAllowedInstance.PlatformServices.AssemblyLoadContextAccessor.Default.Load(_assemblyName);

			return assembly;
		}

		protected override string CoreLoadContent(string contentName)
		{
			string fullFilePath;
			string value;

			if ((object)contentName == null)
				throw new ArgumentNullException(nameof(contentName));

			if (ExtensionMethods.DataTypeFascadeLegacyInstance.IsWhiteSpace(contentName))
				throw new ArgumentOutOfRangeException(nameof(contentName));

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
				throw new ArgumentNullException(nameof(sourceName));

			if ((object)properties == null)
				throw new ArgumentNullException(nameof(properties));

			if (ExtensionMethods.DataTypeFascadeLegacyInstance.IsWhiteSpace(sourceName))
				throw new ArgumentOutOfRangeException(nameof(sourceName));

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
				throw new ArgumentNullException(nameof(templateName));

			if (ExtensionMethods.DataTypeFascadeLegacyInstance.IsWhiteSpace(templateName))
				throw new ArgumentOutOfRangeException(nameof(templateName));

			fullFilePath = Path.GetFullPath(Path.Combine(this.BaseDirectoryPath, templateName));
			//Console.Error.WriteLine(fullFilePath);

			value = (ITemplateXmlObject)this.Xpe.DeserializeFromXml(fullFilePath);

			return value;
		}

		#endregion
	}
}