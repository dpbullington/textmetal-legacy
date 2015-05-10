/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Linq.Expressions;

using TextMetal.Middleware.Data.Models.Tabular;

namespace TextMetal.Middleware.Data.Impl.MicrosoftLinq
{
	public sealed class LinqTableModelQuery<TTableModelObject> : ITableModelQuery
		where TTableModelObject : class, new()
	{
		#region Constructors/Destructors

		public LinqTableModelQuery(Expression<Func<TTableModelObject, bool>> predicate)
		{
			if ((object)predicate == null)
				throw new ArgumentNullException("predicate");

			this.predicate = predicate;
		}

		#endregion

		#region Fields/Constants

		private readonly Expression<Func<TTableModelObject, bool>> predicate;

		#endregion

		#region Properties/Indexers/Events

		internal Expression<Func<TTableModelObject, bool>> Predicate
		{
			get
			{
				return this.predicate;
			}
		}

		#endregion

		#region Methods/Operators

		public Expression GetCanonicalExpression()
		{
			return this.Predicate;
		}

		#endregion
	}
}