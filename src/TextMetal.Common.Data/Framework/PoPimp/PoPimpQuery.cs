/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Syntax.Expressions;

namespace TextMetal.Common.Data.Framework.PoPimp
{
	public sealed class PoPimpQuery : IModelQuery
	{
		#region Constructors/Destructors

		public PoPimpQuery(IExpression filterExpression, IEnumerable<ISequence> sortSequences)
		{
			if ((object)filterExpression == null)
				throw new ArgumentNullException("filterExpression");

			if ((object)sortSequences == null)
				throw new ArgumentNullException("sortSequences");

			this.filterExpression = filterExpression;
			this.sortSequences = sortSequences;
		}

		#endregion

		#region Fields/Constants

		private readonly IExpression filterExpression;
		private readonly IEnumerable<ISequence> sortSequences;

		#endregion

		#region Properties/Indexers/Events

		public IExpression FilterExpression
		{
			get
			{
				return this.filterExpression;
			}
		}

		public IEnumerable<ISequence> SortSequences
		{
			get
			{
				return this.sortSequences;
			}
		}

		#endregion

		#region Methods/Operators

		public object GetUnderlyingQuery()
		{
			return this;
		}

		#endregion
	}
}