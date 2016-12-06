/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Framework.Core;

namespace TextMetal.Framework.Source
{
	public abstract class SourceStrategy : ISourceStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the SourceStrategy class.
		/// </summary>
		protected SourceStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		protected abstract object CoreGetSourceObject(string sourceFilePath, IDictionary<string, IList<string>> properties);

		public object GetSourceObject(/*ITemplatingContext templatingContext, */string sourceFilePath, IDictionary<string, IList<string>> properties)
		{
			/*if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));*/

			try
			{
				return this.CoreGetSourceObject(sourceFilePath, properties);
			}
			finally
			{
			}
			/*catch (Exception ex)
			{
				throw new InvalidOperationException(string.Format("The source strategy failed (see inner exception)."), ex);
			}*/
		}

		#endregion
	}
}