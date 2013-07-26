// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;
using System.Text;

namespace NUnit.Core.Filters
{
	/// <summary>
	/// 	CategoryFilter is able to select or exclude tests
	/// 	based on their categories.
	/// </summary>
	[Serializable]
	public class CategoryFilter : TestFilter
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Construct an empty CategoryFilter
		/// </summary>
		public CategoryFilter()
		{
			this.categories = new ArrayList();
		}

		/// <summary>
		/// 	Construct a CategoryFilter using a single category name
		/// </summary>
		/// <param name="name"> A category name </param>
		public CategoryFilter(string name)
		{
			this.categories = new ArrayList();
			if (name != null && name != string.Empty)
				this.categories.Add(name);
		}

		/// <summary>
		/// 	Construct a CategoryFilter using an array of category names
		/// </summary>
		/// <param name="names"> An array of category names </param>
		public CategoryFilter(string[] names)
		{
			this.categories = new ArrayList();
			if (names != null)
				this.categories.AddRange(names);
		}

		#endregion

		#region Fields/Constants

		private ArrayList categories;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Gets the list of categories from this filter
		/// </summary>
		public IList Categories
		{
			get
			{
				return this.categories;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Add a category name to the filter
		/// </summary>
		/// <param name="name"> A category name </param>
		public void AddCategory(string name)
		{
			this.categories.Add(name);
		}

		/// <summary>
		/// 	Check whether the filter matches a test
		/// </summary>
		/// <param name="test"> The test to be matched </param>
		/// <returns> </returns>
		public override bool Match(ITest test)
		{
			if (test.Categories == null)
				return false;

			foreach (string cat in this.categories)
			{
				if (test.Categories.Contains(cat))
					return true;
			}

			return false;
		}

		/// <summary>
		/// 	Return the string representation of a category filter
		/// </summary>
		/// <returns> </returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < this.categories.Count; i++)
			{
				if (i > 0)
					sb.Append(',');
				sb.Append(this.categories[i]);
			}
			return sb.ToString();
		}

		#endregion
	}
}