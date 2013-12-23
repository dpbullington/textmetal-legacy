/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;

namespace TextMetal.Common.Cerealization
{
	/// <summary>
	/// Provides a strategy pattern around serializing and deserializing objects using binary semantics.
	/// </summary>
	public interface IBinarySerializationStrategy : ISerializationStrategy
	{
		#region Methods/Operators

		/// <summary>
		/// Deserializes an object from the specified binary reader.
		/// </summary>
		/// <param name="binaryReader"> The binary reader to deserialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the deserialized object graph. </param>
		/// <returns> An object of the target type or null. </returns>
		object GetObjectFromReader(BinaryReader binaryReader, Type targetType);

		/// <summary>
		/// Deserializes an object from the specified binary reader. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target run-time type of the root of the deserialized object graph. </typeparam>
		/// <param name="binaryReader"> The binary reader to deserialize. </param>
		/// <returns> An object of the target type or null. </returns>
		TObject GetObjectFromReader<TObject>(BinaryReader binaryReader);

		/// <summary>
		/// Deserializes an object from the specified string value.
		/// </summary>
		/// <param name="value"> The string value to deserialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the deserialized object graph. </param>
		/// <returns> An object of the target type or null. </returns>
		object GetObjectFromString(string value, Type targetType);

		/// <summary>
		/// Deserializes an object from the specified binary value. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target run-time type of the root of the deserialized object graph. </typeparam>
		/// <param name="value"> The string value to deserialize. </param>
		/// <returns> An object of the target type or null. </returns>
		TObject GetObjectFromString<TObject>(string value);

		/// <summary>
		/// Serializes an object to a string value.
		/// </summary>
		/// <param name="obj"> The object graph to serialize. </param>
		/// <returns> A string representation of the object graph. </returns>
		string SetObjectToString(object obj);

		/// <summary>
		/// Serializes an object to a string value. This is the generic overload.
		/// </summary>
		/// <param name="obj"> The object graph to serialize. </param>
		/// <returns> A string representation of the object graph. </returns>
		object SetObjectToString<TObject>(TObject obj);

		/// <summary>
		/// Serializes an object to the specified binary writer.
		/// </summary>
		/// <param name="binaryWriter"> The binary writer to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		void SetObjectToWriter(BinaryWriter binaryWriter, object obj);

		/// <summary>
		/// Serializes an object to the specified binary writer. This is the generic overload.
		/// </summary>
		/// <param name="binaryWriter"> The binary writer to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		void SetObjectToWriter<TObject>(BinaryWriter binaryWriter, TObject obj);

		#endregion
	}
}