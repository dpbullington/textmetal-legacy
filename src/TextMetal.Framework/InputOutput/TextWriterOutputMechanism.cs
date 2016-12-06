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
	public class TextWriterOutputMechanism : OutputMechanism
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the TextWriterOutputMechanism class.
		/// </summary>
		public TextWriterOutputMechanism(TextWriter textWriter, IXmlPersistEngine xpe)
		{
			if ((object)textWriter == null)
				throw new ArgumentNullException(nameof(textWriter));

			if ((object)xpe == null)
				throw new ArgumentNullException(nameof(xpe));

			this.TextWriters.Push(textWriter);
			this.xpe = xpe;
		}

		#endregion

		#region Fields/Constants

		private readonly IXmlPersistEngine xpe;

		#endregion

		#region Properties/Indexers/Events

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
		}

		protected override void CoreLeave(string scopeName)
		{
		}

		protected override void CoreWriteObject(object obj, string objectName)
		{
			IXmlObject xmlObject;
			ITextSerializationStrategy serializationStrategy;

			if ((object)obj == null)
				throw new ArgumentNullException(nameof(obj));

			xmlObject = obj as IXmlObject;

			// this should support XPE, XML, JSON
			/*
				BACKLOG(dpbullington@gmail.com / 2015 - 12 - 18):
				Refactor this logic that is common between this File and TextWriter Outputs.
			*/
			if ((object)xmlObject != null)
				serializationStrategy = new XpeSerializationStrategy(this.Xpe);
			else if ((object)SolderLegacyInstanceAccessor.ReflectionFascadeLegacyInstance.GetOneAttribute<XmlRootAttribute>(obj.GetType()) != null)
				serializationStrategy = new XmlSerializationStrategy();
			else
				serializationStrategy = new JsonSerializationStrategy();

			serializationStrategy.SetObjectToWriter(this.CurrentTextWriter, obj);
		}

		#endregion
	}
}