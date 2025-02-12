﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;

using Newtonsoft.Json;

namespace TextMetal.Middleware.Solder.Serialization
{
	public class JsonSerializationStrategy : TextSerializationStrategy, IJsonSerializationStrategy
	{
		#region Constructors/Destructors

		public JsonSerializationStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Deserializes an object from the specified text reader.
		/// </summary>
		/// <param name="textReader"> The text reader to deserialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the deserialized object graph. </param>
		/// <returns> An object of the target type or null. </returns>
		public override object GetObjectFromReader(TextReader textReader, Type targetType)
		{
			object obj;

			if ((object)textReader == null)
				throw new ArgumentNullException(nameof(textReader));

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			using (JsonReader jsonReader = new JsonTextReader(textReader))
				obj = this.GetObjectFromReader(jsonReader, targetType);

			return obj;
		}

		/// <summary>
		/// Deserializes an object from the specified json reader.
		/// </summary>
		/// <param name="jsonReader"> The json reader to deserialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the deserialized object graph. </param>
		/// <returns> An object of the target type or null. </returns>
		public object GetObjectFromReader(JsonReader jsonReader, Type targetType)
		{
			JsonSerializer jsonSerializer;
			object obj;

			if ((object)jsonReader == null)
				throw new ArgumentNullException(nameof(jsonReader));

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			jsonSerializer = GetJsonSerializer();
			obj = jsonSerializer.Deserialize(jsonReader, targetType);

			return obj;
		}

		/// <summary>
		/// Deserializes an object from the specified json reader. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target run-time type of the root of the deserialized object graph. </typeparam>
		/// <param name="jsonReader"> The json reader to deserialize. </param>
		/// <returns> An object of the target type or null. </returns>
		public TObject GetObjectFromReader<TObject>(JsonReader jsonReader)
		{
			TObject obj;
			Type targetType;

			if ((object)jsonReader == null)
				throw new ArgumentNullException(nameof(jsonReader));

			targetType = typeof(TObject);
			obj = (TObject)this.GetObjectFromReader(jsonReader, targetType);

			return obj;
		}

		/// <summary>
		/// Deserializes an object from the specified readable stream.
		/// </summary>
		/// <param name="stream"> The readable stream to deserialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the deserialized object graph. </param>
		/// <returns> An object of the target type or null. </returns>
		public override object GetObjectFromStream(Stream stream, Type targetType)
		{
			object obj;

			if ((object)stream == null)
				throw new ArgumentNullException(nameof(stream));

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			using (StreamReader streamReader = new StreamReader(stream))
			{
				using (JsonReader jsonReader = new JsonTextReader(streamReader))
					obj = this.GetObjectFromReader(jsonReader, targetType);
			}

			return obj;
		}

		/// <summary>
		/// Serializes an object to the specified writable stream.
		/// </summary>
		/// <param name="stream"> The writable stream to serialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the object graph to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		public override void SetObjectToStream(Stream stream, Type targetType, object obj)
		{
			if ((object)stream == null)
				throw new ArgumentNullException(nameof(stream));

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			if ((object)obj == null)
				throw new ArgumentNullException(nameof(obj));

			using (StreamWriter streamWriter = new StreamWriter(stream))
			{
				using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
					this.SetObjectToWriter(jsonWriter, targetType, obj);
			}
		}

		/// <summary>
		/// Serializes an object to the specified text writer.
		/// </summary>
		/// <param name="textWriter"> The text writer to serialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the object graph to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		public override void SetObjectToWriter(TextWriter textWriter, Type targetType, object obj)
		{
			if ((object)textWriter == null)
				throw new ArgumentNullException(nameof(textWriter));

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			if ((object)obj == null)
				throw new ArgumentNullException(nameof(obj));

			using (JsonWriter jsonWriter = new JsonTextWriter(textWriter))
				this.SetObjectToWriter(jsonWriter, targetType, obj);
		}

		private static JsonSerializer GetJsonSerializer()
		{
			return JsonSerializer.Create(new JsonSerializerSettings()
										{
											Formatting = Formatting.Indented,
											TypeNameHandling = TypeNameHandling.None,
											ReferenceLoopHandling = ReferenceLoopHandling.Ignore
										});
		}

		/// <summary>
		/// Serializes an object to the specified json writer.
		/// </summary>
		/// <param name="jsonWriter"> The json writer to serialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the object graph to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		public void SetObjectToWriter(JsonWriter jsonWriter, Type targetType, object obj)
		{
			JsonSerializer jsonSerializer;

			if ((object)jsonWriter == null)
				throw new ArgumentNullException(nameof(jsonWriter));

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			jsonSerializer = GetJsonSerializer();
			jsonSerializer.Serialize(jsonWriter, obj, targetType);
		}

		/// <summary>
		/// Serializes an object to the specified json writer. This is the generic overload.
		/// </summary>
		/// <param name="jsonWriter"> The json writer to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		public void SetObjectToWriter<TObject>(JsonWriter jsonWriter, TObject obj)
		{
			Type targetType;

			if ((object)jsonWriter == null)
				throw new ArgumentNullException(nameof(jsonWriter));

			if ((object)obj == null)
				throw new ArgumentNullException(nameof(obj));

			targetType = obj.GetType();

			this.SetObjectToWriter(jsonWriter, targetType, (object)obj);
		}

		#endregion
	}
}