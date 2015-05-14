/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Xml;

using TextMetal.Middleware.Common.Utilities;
using TextMetal.Middleware.Solder.Serialization;

namespace TextMetal.Framework.XmlDialect
{
	public class XpeSerializationStrategy : IXmlSerializationStrategy, ITextSerializationStrategy
	{
		#region Constructors/Destructors

		public XpeSerializationStrategy()
			: this(new XmlPersistEngine())
		{
		}

		public XpeSerializationStrategy(IXmlPersistEngine xpe)
		{
			if ((object)xpe == null)
				throw new ArgumentNullException("xpe");

			this.xpe = xpe;
		}

		#endregion

		#region Fields/Constants

		private static readonly XpeSerializationStrategy instance = new XpeSerializationStrategy();
		private readonly IXmlPersistEngine xpe;

		#endregion

		#region Properties/Indexers/Events

		public static XpeSerializationStrategy Instance
		{
			get
			{
				return instance;
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

		/// <summary>
		/// Deserializes an object from the specified input file.
		/// </summary>
		/// <param name="inputFilePath"> The input file path to deserialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the deserialized object graph. </param>
		/// <returns> An object of the target type or null. </returns>
		public object GetObjectFromFile(string inputFilePath, Type targetType)
		{
			return this.Xpe.DeserializeFromXml(inputFilePath);
		}

		/// <summary>
		/// Deserializes an object from the specified input file. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target run-time type of the root of the deserialized object graph. </typeparam>
		/// <param name="inputFilePath"> The input file path to deserialize. </param>
		/// <returns> An object of the target type or null. </returns>
		public TObject GetObjectFromFile<TObject>(string inputFilePath)
		{
			TObject obj;
			Type targetType;

			if ((object)inputFilePath == null)
				throw new ArgumentNullException("inputFilePath");

			if (DataTypeFascade.Instance.IsWhiteSpace(inputFilePath))
				throw new ArgumentOutOfRangeException("inputFilePath");

			targetType = typeof(TObject);
			obj = (TObject)this.GetObjectFromFile(inputFilePath, targetType);

			return obj;
		}

		/// <summary>
		/// Deserializes an object from the specified xml reader.
		/// </summary>
		/// <param name="xmlReader"> The xml reader to deserialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the deserialized object graph. </param>
		/// <returns> An object of the target type or null. </returns>
		public object GetObjectFromReader(XmlReader xmlReader, Type targetType)
		{
			return this.Xpe.DeserializeFromXml(xmlReader);
		}

		/// <summary>
		/// Deserializes an object from the specified xml reader. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target run-time type of the root of the deserialized object graph. </typeparam>
		/// <param name="xmlReader"> The xml reader to deserialize. </param>
		/// <returns> An object of the target type or null. </returns>
		public TObject GetObjectFromReader<TObject>(XmlReader xmlReader)
		{
			TObject obj;
			Type targetType;

			if ((object)xmlReader == null)
				throw new ArgumentNullException("xmlReader");

			targetType = typeof(TObject);
			obj = (TObject)this.GetObjectFromReader(xmlReader, targetType);

			return obj;
		}

		/// <summary>
		/// Deserializes an object from the specified text reader.
		/// </summary>
		/// <param name="textReader"> The text reader to deserialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the deserialized object graph. </param>
		/// <returns> An object of the target type or null. </returns>
		public object GetObjectFromReader(TextReader textReader, Type targetType)
		{
			return this.Xpe.DeserializeFromXml(textReader);
		}

		/// <summary>
		/// Deserializes an object from the specified text reader. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target run-time type of the root of the deserialized object graph. </typeparam>
		/// <param name="textReader"> The text reader to deserialize. </param>
		/// <returns> An object of the target type or null. </returns>
		public TObject GetObjectFromReader<TObject>(TextReader textReader)
		{
			TObject obj;
			Type targetType;

			if ((object)textReader == null)
				throw new ArgumentNullException("textReader");

			targetType = typeof(TObject);
			obj = (TObject)this.GetObjectFromReader(textReader, targetType);

			return obj;
		}

		/// <summary>
		/// Deserializes an object from the specified readable stream.
		/// </summary>
		/// <param name="stream"> The readable stream to deserialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the deserialized object graph. </param>
		/// <returns> An object of the target type or null. </returns>
		public object GetObjectFromStream(Stream stream, Type targetType)
		{
			return this.Xpe.DeserializeFromXml(stream);
		}

		/// <summary>
		/// Deserializes an object from the specified readable stream. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target run-time type of the root of the deserialized object graph. </typeparam>
		/// <param name="stream"> The readable stream to deserialize. </param>
		/// <returns> An object of the target type or null. </returns>
		public TObject GetObjectFromStream<TObject>(Stream stream)
		{
			TObject obj;
			Type targetType;

			if ((object)stream == null)
				throw new ArgumentNullException("stream");

			targetType = typeof(TObject);
			obj = (TObject)this.GetObjectFromStream(stream, targetType);

			return obj;
		}

		/// <summary>
		/// Deserializes an object from the specified string value.
		/// </summary>
		/// <param name="value"> The string value to deserialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the deserialized object graph. </param>
		/// <returns> An object of the target type or null. </returns>
		public object GetObjectFromString(string value, Type targetType)
		{
			object obj;
			StringReader stringReader;

			using (stringReader = new StringReader(value))
			{
				obj = this.GetObjectFromReader(stringReader, targetType);
				return obj;
			}
		}

		/// <summary>
		/// Deserializes an object from the specified text value. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target run-time type of the root of the deserialized object graph. </typeparam>
		/// <param name="value"> The string value to deserialize. </param>
		/// <returns> An object of the target type or null. </returns>
		public TObject GetObjectFromString<TObject>(string value)
		{
			TObject obj;
			Type targetType;

			if ((object)value == null)
				throw new ArgumentNullException("value");

			targetType = typeof(TObject);
			obj = (TObject)this.GetObjectFromString(value, targetType);

			return obj;
		}

		/// <summary>
		/// Serializes an object to the specified output file.
		/// </summary>
		/// <typeparam name="TObject"> The target run-time type of the root of the object graph to serialize. </typeparam>
		/// <param name="outputFilePath"> The output file path to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		public void SetObjectToFile<TObject>(string outputFilePath, TObject obj)
		{
			Type targetType;

			if ((object)outputFilePath == null)
				throw new ArgumentNullException("outputFilePath");

			if ((object)obj == null)
				throw new ArgumentNullException("obj");

			if (DataTypeFascade.Instance.IsWhiteSpace(outputFilePath))
				throw new ArgumentOutOfRangeException("outputFilePath");

			targetType = obj.GetType();

			this.SetObjectToFile(outputFilePath, targetType, (object)obj);
		}

		/// <summary>
		/// Serializes an object to the specified output file.
		/// </summary>
		/// <param name="outputFilePath"> The output file path to serialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the object graph to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		public void SetObjectToFile(string outputFilePath, Type targetType, object obj)
		{
			if ((object)outputFilePath == null)
				throw new ArgumentNullException("outputFilePath");

			if ((object)obj == null)
				throw new ArgumentNullException("obj");

			if (DataTypeFascade.Instance.IsWhiteSpace(outputFilePath))
				throw new ArgumentOutOfRangeException("outputFilePath");

			using (Stream stream = File.Open(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
				this.SetObjectToStream(stream, targetType, obj);
		}

		/// <summary>
		/// Serializes an object to the specified writable stream.
		/// </summary>
		/// <typeparam name="TObject"> The target run-time type of the root of the object graph to serialize. </typeparam>
		/// <param name="stream"> The writable stream to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		public void SetObjectToStream<TObject>(Stream stream, TObject obj)
		{
			Type targetType;

			if ((object)stream == null)
				throw new ArgumentNullException("stream");

			if ((object)obj == null)
				throw new ArgumentNullException("obj");

			targetType = obj.GetType();

			this.SetObjectToStream(stream, targetType, (object)obj);
		}

		/// <summary>
		/// Serializes an object to the specified writable stream.
		/// </summary>
		/// <param name="stream"> The writable stream to serialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the object graph to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		public void SetObjectToStream(Stream stream, Type targetType, object obj)
		{
			this.Xpe.SerializeToXml((IXmlObject)obj, stream);
		}

		/// <summary>
		/// Serializes an object to a string value.
		/// </summary>
		/// <param name="targetType"> The target run-time type of the root of the object graph to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		/// <returns> A string representation of the object graph. </returns>
		public string SetObjectToString(Type targetType, object obj)
		{
			StringWriter stringWriter;

			if ((object)obj == null)
				throw new ArgumentNullException("obj");

			using (stringWriter = new StringWriter())
			{
				this.SetObjectToWriter(stringWriter, targetType, obj);
				return stringWriter.ToString();
			}
		}

		/// <summary>
		/// Serializes an object to a string value. This is the generic overload.
		/// </summary>
		/// <param name="obj"> The object graph to serialize. </param>
		/// <returns> A string representation of the object graph. </returns>
		public string SetObjectToString<TObject>(TObject obj)
		{
			Type targetType;

			if ((object)obj == null)
				throw new ArgumentNullException("obj");

			targetType = obj.GetType();

			return this.SetObjectToString(targetType, (object)obj);
		}

		/// <summary>
		/// Serializes an object to the specified xml writer.
		/// </summary>
		/// <param name="xmlWriter"> The xml writer to serialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the object graph to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		public void SetObjectToWriter(XmlWriter xmlWriter, Type targetType, object obj)
		{
			this.Xpe.SerializeToXml((IXmlObject)obj, xmlWriter);
		}

		/// <summary>
		/// Serializes an object to the specified xml writer. This is the generic overload.
		/// </summary>
		/// <param name="xmlWriter"> The xml writer to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		public void SetObjectToWriter<TObject>(XmlWriter xmlWriter, TObject obj)
		{
			Type targetType;

			if ((object)xmlWriter == null)
				throw new ArgumentNullException("xmlWriter");

			if ((object)obj == null)
				throw new ArgumentNullException("obj");

			targetType = obj.GetType();

			this.SetObjectToWriter(xmlWriter, targetType, (object)obj);
		}

		/// <summary>
		/// Serializes an object to the specified text writer.
		/// </summary>
		/// <param name="textWriter"> The text writer to serialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the object graph to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		public void SetObjectToWriter(TextWriter textWriter, Type targetType, object obj)
		{
			this.Xpe.SerializeToXml((IXmlObject)obj, textWriter);
		}

		/// <summary>
		/// Serializes an object to the specified text writer. This is the generic overload.
		/// </summary>
		/// <param name="textWriter"> The text writer to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		public void SetObjectToWriter<TObject>(TextWriter textWriter, TObject obj)
		{
			Type targetType;

			if ((object)textWriter == null)
				throw new ArgumentNullException("textWriter");

			if ((object)obj == null)
				throw new ArgumentNullException("obj");

			targetType = obj.GetType();

			this.SetObjectToWriter(textWriter, targetType, (object)obj);
		}

		#endregion
	}
}