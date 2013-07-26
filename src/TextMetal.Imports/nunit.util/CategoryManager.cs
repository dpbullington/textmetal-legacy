// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;

using NUnit.Core;

namespace NUnit.Util
{
	public class CategoryManager
	{
		#region Fields/Constants

		private Hashtable categories = new Hashtable();

		#endregion

		#region Properties/Indexers/Events

		public ICollection Categories
		{
			get
			{
				return this.categories.Values;
			}
		}

		#endregion

		#region Methods/Operators

		public void Add(string name)
		{
			this.categories[name] = name;
		}

		public void AddAllCategories(ITest test)
		{
			this.AddCategories(test);
			if (test.IsSuite)
			{
				foreach (ITest child in test.Tests)
					this.AddAllCategories(child);
			}
		}

		public void AddCategories(ITest test)
		{
			if (test.Categories != null)
			{
				foreach (string name in test.Categories)
				{
					if (NUnitFramework.IsValidCategoryName(name))
						this.Add(name);
				}
			}
		}

		public void Clear()
		{
			this.categories = new Hashtable();
		}

		#endregion
	}
}