/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace TextMetal.Middleware.Datazoid.Models.Functional
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
		private IEnumerable<TResultProcedureModelObject> records;

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

		public IEnumerable<TResultProcedureModelObject> Records
		{
			get
			{
				return this.records;
			}
			set
			{
				this.records = value;
			}
		}

		#endregion
	}
}