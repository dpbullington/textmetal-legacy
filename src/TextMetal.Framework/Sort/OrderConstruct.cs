﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TextMetal.Framework.Core;
using TextMetal.Framework.Expression;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Sort
{
	public abstract class OrderConstruct : SortXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the OrderConstruct class.
		/// </summary>
		/// <param name="ascending"> A value indicating whether the order is ascending (true) or descending (false). </param>
		protected OrderConstruct(bool ascending)
		{
			this.ascending = ascending;
		}

		#endregion

		#region Fields/Constants

		private readonly bool ascending;
		private IExpressionContainerConstruct compare;

		#endregion

		#region Properties/Indexers/Events

		private bool Ascending
		{
			get
			{
				return this.ascending;
			}
		}

		public override bool? SortDirection
		{
			get
			{
				return this.Ascending;
			}
		}

		[XmlChildElementMapping(ChildElementType = ChildElementType.ParentQualified, LocalName = "Compare", NamespaceUri = "http://www.textmetal.com/api/v6.0.0")]
		public IExpressionContainerConstruct Compare
		{
			get
			{
				return this.compare;
			}
			set
			{
				this.compare = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override IEnumerable CoreEvaluateSort(ITemplatingContext templatingContext, IEnumerable values)
		{
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;
			object obj;
			Type objType;
			Dictionary<object, IComparable> temp;
			IComparable comparable;

			if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));

			if ((object)values == null)
				throw new ArgumentNullException(nameof(values));

			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy();

			if ((object)this.Compare != null)
			{
				temp = new Dictionary<object, IComparable>();

				foreach (object value in values)
				{
					templatingContext.IteratorModels.Push(value);

					obj = this.Compare.EvaluateExpression(templatingContext);

					if ((object)obj == null)
						continue;

					templatingContext.IteratorModels.Pop();

					objType = obj.GetType();

					if (!typeof(IComparable).IsAssignableFrom(objType))
						throw new InvalidOperationException(string.Format("The target expression of ordering is not assignable to type '{0}'.", typeof(IComparable).FullName));

					comparable = (IComparable)obj;

					temp.Add(value, comparable);
				}

				if (this.Ascending)
					values = values.Cast<object>().OrderBy(o => temp[o], new ComparableComparer());
				else
					values = values.Cast<object>().OrderByDescending(o => temp[o], new ComparableComparer());
			}

			return values;
		}

		#endregion
	}
}