/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Datazoid.Extensions
{
	/// <summary>
	/// Provides extension methods for unit of work instances.
	/// </summary>
	public static class EnumerableExtensionMethods
	{
		#region Methods/Operators

		/// <summary>
		/// An extension method to extract outputs from a record dictionary.
		/// </summary>
		/// <param name="dbParameters"> The target enumerable of data paramters. </param>
		/// <returns> A dictionary with record key/value pairs of OUTPUT data. </returns>
		public static IRecord GetOutputAsRecord(this IEnumerable<DbParameter> dbParameters)
		{
			IRecord output;

			if ((object)dbParameters == null)
				throw new ArgumentNullException(nameof(dbParameters));

			output = new Record();

			foreach (DbParameter dbParameter in dbParameters)
			{
				if (dbParameter.Direction != ParameterDirection.InputOutput &&
					dbParameter.Direction != ParameterDirection.Output &&
					dbParameter.Direction != ParameterDirection.ReturnValue)
					continue;

				output.Add(dbParameter.ParameterName, dbParameter.Value);
			}

			return output;
		}

		#endregion
	}
}