﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Reflection;

using TextMetal.Middleware.Solder.Serialization;

namespace TextMetal.Middleware.Solder.Extensions
{
	public static class SerializationExtensionMethods
	{
		#region Methods/Operators

		/// <summary>
		/// Deserializes an object from an assembly manifest resource.
		/// </summary>
		/// <typeparam name="TObject"> The run-time type of the object root to deserialize. </typeparam>
		/// <param name="serializationStrategy"> The serialization strategy. </param>
		/// <param name="resourceType"> A type within the source assembly where the manifest resource lives. </param>
		/// <param name="resourceName"> The fully qualified manifest resource name to load. </param>
		/// <param name="result"> A valid object of the specified type or null if the manifest resource name was not found in the assembly of the resource type. </param>
		/// <returns> A value indicating whether the manifest resource name was found in the target type's assembly. </returns>
		public static bool TryGetFromAssemblyResource<TObject>(this ISerializationStrategy serializationStrategy, Type resourceType, string resourceName, out TObject result)
		{
			Type targetType;
			bool retval;

			if ((object)serializationStrategy == null)
				throw new ArgumentNullException(nameof(serializationStrategy));

			if ((object)resourceType == null)
				throw new ArgumentNullException(nameof(resourceType));

			if ((object)resourceName == null)
				throw new ArgumentNullException(nameof(resourceName));

			result = default(TObject);
			targetType = typeof(TObject);

			using (Stream stream = resourceType.GetTypeInfo().Assembly.GetManifestResourceStream(resourceName))
			{
				if (retval = ((object)stream != null))
					result = (TObject)serializationStrategy.GetObjectFromStream(stream, targetType);
			}

			return retval;
		}

		/// <summary>
		/// Deserializes a string from an assembly manifest resource.
		/// </summary>
		/// <param name="resourceType"> A type within the source assembly where the manifest resource lives. </param>
		/// <param name="resourceName"> The fully qualified manifest resource name to load. </param>
		/// <param name="result"> A valid string or null if the manifest resource name was not found in the assembly of the resource type. </param>
		/// <returns> A value indicating whether the manifest resource name was found in the target type's assembly. </returns>
		public static bool TryGetStringFromAssemblyResource(this Type resourceType, string resourceName, out string result)
		{
			bool retval;

			if ((object)resourceType == null)
				throw new ArgumentNullException(nameof(resourceType));

			if ((object)resourceName == null)
				throw new ArgumentNullException(nameof(resourceName));

			result = null;

			using (Stream stream = resourceType.GetTypeInfo().Assembly.GetManifestResourceStream(resourceName))
			{
				if (retval = (object)stream != null)
				{
					using (StreamReader streamReader = new StreamReader(stream))
						result = streamReader.ReadToEnd();
				}
			}

			return retval;
		}

		#endregion
	}
}