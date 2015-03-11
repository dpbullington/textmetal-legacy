/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;

using TextMetal.Common.Core;
using TextMetal.Common.Core.Cerealization;

namespace TextMetal.Framework.Source.Primative
{
	public class JsonSourceStrategy : SourceStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the JsonSourceStrategy class.
		/// </summary>
		public JsonSourceStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		protected override object CoreGetSourceObject(string sourceFilePath, IDictionary<string, IList<string>> properties)
		{
			object retval;

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException("sourceFilePath");

			if ((object)properties == null)
				throw new ArgumentNullException("properties");

			if (DataType.Instance.IsWhiteSpace(sourceFilePath))
				throw new ArgumentOutOfRangeException("sourceFilePath");

			sourceFilePath = Path.GetFullPath(sourceFilePath);

			retval = JsonSerializationStrategy.Instance.GetObjectFromFile<object>(sourceFilePath);

			return retval;
		}

		#endregion
	}
}