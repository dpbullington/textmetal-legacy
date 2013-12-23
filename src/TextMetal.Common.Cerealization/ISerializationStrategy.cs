/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;

namespace TextMetal.Common.Cerealization
{
	/// <summary>
	/// Provides a strategy pattern around serializing and deserializing objects.
	/// </summary>
	public interface ISerializationStrategy
	{
		#region Methods/Operators

		/// <summary>
		/// Deserializes an object from the specified input file.
		/// </summary>
		/// <param name="inputFilePath"> The input file path to deserialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the deserialized object graph. </param>
		/// <returns> An object of the target type or null. </returns>
		object GetObjectFromFile(string inputFilePath, Type targetType);

		/// <summary>
		/// Deserializes an object from the specified input file. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target run-time type of the root of the deserialized object graph. </typeparam>
		/// <param name="inputFilePath"> The input file path to deserialize. </param>
		/// <returns> An object of the target type or null. </returns>
		TObject GetObjectFromFile<TObject>(string inputFilePath);

		/// <summary>
		/// Deserializes an object from the specified readable stream.
		/// </summary>
		/// <param name="stream"> The readable stream to deserialize. </param>
		/// <param name="targetType"> The target run-time type of the root of the deserialized object graph. </param>
		/// <returns> An object of the target type or null. </returns>
		object GetObjectFromStream(Stream stream, Type targetType);

		/// <summary>
		/// Deserializes an object from the specified readable stream. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target run-time type of the root of the deserialized object graph. </typeparam>
		/// <param name="stream"> The readable stream to deserialize. </param>
		/// <returns> An object of the target type or null. </returns>
		TObject GetObjectFromStream<TObject>(Stream stream);

		/// <summary>
		/// Serializes an object to the specified output file.
		/// </summary>
		/// <typeparam name="TObject"> The target run-time type of the root of the object graph to serialize. </typeparam>
		/// <param name="outputFilePath"> The output file path to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		void SetObjectToFile<TObject>(string outputFilePath, TObject obj);

		/// <summary>
		/// Serializes an object to the specified output file.
		/// </summary>
		/// <param name="outputFilePath"> The output file path to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		void SetObjectToFile(string outputFilePath, object obj);

		/// <summary>
		/// Serializes an object to the specified writable stream.
		/// </summary>
		/// <param name="stream"> The writable stream to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		void SetObjectToStream(Stream stream, object obj);

		/// <summary>
		/// Serializes an object to the specified writable stream.
		/// </summary>
		/// <typeparam name="TObject"> The target run-time type of the root of the object graph to serialize. </typeparam>
		/// <param name="stream"> The writable stream to serialize. </param>
		/// <param name="obj"> The object graph to serialize. </param>
		void SetObjectToStream<TObject>(Stream stream, TObject obj);

		/// <summary>
		/// Deserializes an object from an assembly manifest resource.
		/// </summary>
		/// <typeparam name="TObject"> The run-time type of the object root to deserialize. </typeparam>
		/// <param name="resourceType"> A type within the source assembly where the manifest resource lives. </param>
		/// <param name="resourceName"> The fully qualified manifest resource name to load. </param>
		/// <param name="result"> A valid object of the specified type or null if the manifest resource name was not found in the assembly of the resource type. </param>
		/// <returns> A value indicating whether the manifest resource name was found in the target type's assembly. </returns>
		bool TryGetFromAssemblyResource<TObject>(Type resourceType, string resourceName, out TObject result);

		/// <summary>
		/// Deserializes a string from an assembly manifest resource.
		/// </summary>
		/// <param name="resourceType"> A type within the source assembly where the manifest resource lives. </param>
		/// <param name="resourceName"> The fully qualified manifest resource name to load. </param>
		/// <param name="result"> A valid string or null if the manifest resource name was not found in the assembly of the resource type. </param>
		/// <returns> A value indicating whether the manifest resource name was found in the target type's assembly. </returns>
		bool TryGetStringFromAssemblyResource(Type resourceType, string resourceName, out string result);

		#endregion
	}
}