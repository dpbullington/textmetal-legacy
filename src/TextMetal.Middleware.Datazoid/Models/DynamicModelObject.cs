/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace TextMetal.Middleware.Datazoid.Models
{
	public abstract class DynamicModelObject : Dictionary<string, object>, IModelObject
	{
		#region Constructors/Destructors

		protected DynamicModelObject()
		{
			if (this.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).Length > 0)
				throw new NotSupportedException(string.Format("This type is semantically sealed."));
		}

		#endregion
	}
}