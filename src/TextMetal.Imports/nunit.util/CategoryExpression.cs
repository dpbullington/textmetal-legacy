// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

using NUnit.Core;
using NUnit.Core.Filters;

namespace NUnit.Util
{
	/// <summary>
	/// 	CategoryExpression parses strings representing boolean
	/// 	combinations of categories according to the following
	/// 	grammar:
	/// 	CategoryName ::= string not containing any of ',', '&', '+', '-'
	/// 	CategoryFilter ::= CategoryName | CategoryFilter ',' CategoryName
	/// 	CategoryPrimitive ::= CategoryFilter | '-' CategoryPrimitive
	/// 	CategoryTerm ::= CategoryPrimitive | CategoryTerm '&' CategoryPrimitive
	/// </summary>
	public class CategoryExpression
	{
		#region Constructors/Destructors

		public CategoryExpression(string text)
		{
			this.text = text;
			this.next = 0;
		}

		#endregion

		#region Fields/Constants

		private static readonly char[] ops = new char[] { ',', ';', '-', '|', '+', '(', ')' };
		private TestFilter filter;

		private int next;
		private string text;
		private string token;

		#endregion

		#region Properties/Indexers/Events

		public TestFilter Filter
		{
			get
			{
				if (this.filter == null)
				{
					this.filter = this.GetToken() == null
						              ? TestFilter.Empty
						              : this.GetExpression();
				}

				return this.filter;
			}
		}

		#endregion

		#region Methods/Operators

		private bool EndOfText()
		{
			return this.next >= this.text.Length;
		}

		private CategoryFilter GetCategoryFilter()
		{
			CategoryFilter filter = new CategoryFilter(this.token);

			while (this.GetToken() == "," || this.token == ";")
				filter.AddCategory(this.GetToken());

			return filter;
		}

		private TestFilter GetExpression()
		{
			TestFilter term = this.GetTerm();
			if (this.token != "|")
				return term;

			OrFilter filter = new OrFilter(term);

			while (this.token == "|")
			{
				this.GetToken();
				filter.Add(this.GetTerm());
			}

			return filter;
		}

		private TestFilter GetPrimitive()
		{
			if (this.token == "-")
			{
				this.GetToken();
				return new NotFilter(this.GetPrimitive());
			}
			else if (this.token == "(")
			{
				this.GetToken();
				TestFilter expr = this.GetExpression();
				this.GetToken(); // Skip ')'
				return expr;
			}

			return this.GetCategoryFilter();
		}

		private TestFilter GetTerm()
		{
			TestFilter prim = this.GetPrimitive();
			if (this.token != "+" && this.token != "-")
				return prim;

			AndFilter filter = new AndFilter(prim);

			while (this.token == "+" || this.token == "-")
			{
				string tok = this.token;
				this.GetToken();
				prim = this.GetPrimitive();
				filter.Add(tok == "-" ? new NotFilter(prim) : prim);
			}

			return filter;
		}

		public string GetToken()
		{
			this.SkipWhiteSpace();

			if (this.EndOfText())
				this.token = null;
			else if (this.NextIsOperator())
				this.token = this.text.Substring(this.next++, 1);
			else
			{
				int index2 = this.text.IndexOfAny(ops, this.next);
				if (index2 < 0)
					index2 = this.text.Length;

				this.token = this.text.Substring(this.next, index2 - this.next).TrimEnd();
				this.next = index2;
			}

			return this.token;
		}

		private bool NextIsOperator()
		{
			foreach (char op in ops)
			{
				if (op == this.text[this.next])
					return true;
			}

			return false;
		}

		private void SkipWhiteSpace()
		{
			while (this.next < this.text.Length && Char.IsWhiteSpace(this.text[this.next]))
				++this.next;
		}

		#endregion
	}
}