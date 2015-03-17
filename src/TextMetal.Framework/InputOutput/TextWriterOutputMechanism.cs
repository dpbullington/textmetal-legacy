/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Text;

using LeastViable.Common.Fascades.Utilities;
using LeastViable.Common.Strategies.Serialization;

using TextMetal.Framework.XmlDialect;

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
				throw new ArgumentNullException("textWriter");

			if ((object)xpe == null)
				throw new ArgumentNullException("xpe");

			base.TextWriters.Push(textWriter);
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
				throw new ArgumentNullException("obj");

			xmlObject = obj as IXmlObject;

			// this should support XPE, XML, JSON
			if ((object)xmlObject != null)
				serializationStrategy = new XpeSerializationStrategy(this.Xpe);
			else if ((object)ReflectionFascade.Instance.GetOneAttribute<SerializableAttribute>(obj.GetType()) != null)
				serializationStrategy = new XmlSerializationStrategy();
			else
				serializationStrategy = new JsonSerializationStrategy();

			serializationStrategy.SetObjectToWriter(this.CurrentTextWriter, obj);
		}

		#endregion
	}
}