// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.Text;

namespace NUnit.Core.Filters
{
	/// <summary>
	/// Combines multiple filters so that a test must pass one
	/// of them in order to pass this filter.
	/// </summary>
	[Serializable]
	public class OrFilter : TestFilter
	{
		#region Constructors/Destructors

		/// <summary>
		/// Constructs an empty OrFilter
		/// </summary>
		public OrFilter()
		{
		}

		/// <summary>
		/// Constructs an AndFilter from an array of filters
		/// </summary>
		/// <param name="filters"> </param>
		public OrFilter(params ITestFilter[] filters)
		{
			this.filters.AddRange(filters);
		}

		#endregion

		#region Fields/Constants

		private ArrayList filters = new ArrayList();

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Return an array of the composing filters
		/// </summary>
		public ITestFilter[] Filters
		{
			get
			{
				return (ITestFilter[])this.filters.ToArray(typeof(ITestFilter));
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Adds a filter to the list of filters
		/// </summary>
		/// <param name="filter"> The filter to be added </param>
		public void Add(ITestFilter filter)
		{
			this.filters.Add(filter);
		}

		/// <summary>
		/// Checks whether the OrFilter is matched by a test
		/// </summary>
		/// <param name="test"> The test to be matched </param>
		/// <returns> True if any of the component filters match, otherwise false </returns>
		public override bool Match(ITest test)
		{
			foreach (ITestFilter filter in this.filters)
			{
				if (filter.Match(test))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Checks whether the OrFilter is matched by a test
		/// </summary>
		/// <param name="test"> The test to be matched </param>
		/// <returns> True if any of the component filters pass, otherwise false </returns>
		public override bool Pass(ITest test)
		{
			foreach (ITestFilter filter in this.filters)
			{
				if (filter.Pass(test))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Return the string representation of an or filter
		/// </summary>
		/// <returns> </returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < this.filters.Count; i++)
			{
				if (i > 0)
					sb.Append(" or ");
				sb.Append(this.filters[i]);
			}
			return sb.ToString();
		}

		#endregion
	}
}