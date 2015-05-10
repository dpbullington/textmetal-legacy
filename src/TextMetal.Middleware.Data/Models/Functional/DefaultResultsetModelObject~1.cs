/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace TextMetal.Middleware.Data.Models.Functional
{
	public sealed class DefaultResultsetModelObject<TResultProcedureModelObject> : IResultsetModelObject<TResultProcedureModelObject>
		where TResultProcedureModelObject : class, IResultProcedureModelObject
	{
		#region Constructors/Destructors

		public DefaultResultsetModelObject()
		{
		}

		#endregion

		#region Fields/Constants

		private int index;
		private IEnumerable<TResultProcedureModelObject> results;

		#endregion

		#region Properties/Indexers/Events

		public int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		public IEnumerable<TResultProcedureModelObject> Results
		{
			get
			{
				return this.results;
			}
			set
			{
				this.results = value;
			}
		}

		#endregion
	}
}