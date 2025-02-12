﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Framework.Source.Primative
{
	public class NullSourceStrategy : SourceStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the NullSourceStrategy class.
		/// </summary>
		public NullSourceStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		protected override object CoreGetSourceObject(string sourceFilePath, IDictionary<string, IList<string>> properties)
		{
			if ((object)sourceFilePath == null)
				throw new ArgumentNullException(nameof(sourceFilePath));

			if ((object)properties == null)
				throw new ArgumentNullException(nameof(properties));

			return string.Empty;
		}

		#endregion
	}
}