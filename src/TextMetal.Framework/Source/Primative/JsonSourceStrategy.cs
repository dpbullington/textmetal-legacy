/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;

using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Serialization;

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
				throw new ArgumentNullException(nameof(sourceFilePath));

			if ((object)properties == null)
				throw new ArgumentNullException(nameof(properties));

			if (SolderFascadeAccessor.DataTypeFascade.IsWhiteSpace(sourceFilePath))
				throw new ArgumentOutOfRangeException(nameof(sourceFilePath));

			sourceFilePath = Path.GetFullPath(sourceFilePath);

			retval = new JsonSerializationStrategy().GetObjectFromFile<object>(sourceFilePath);

			return retval;
		}

		#endregion
	}
}