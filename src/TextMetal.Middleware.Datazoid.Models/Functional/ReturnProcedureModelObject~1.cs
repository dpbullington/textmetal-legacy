/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Middleware.Datazoid.Models.Functional
{
	public abstract class ReturnProcedureModelObject<TResultsetModelObject, TResultProcedureModelObject> : ReturnProcedureModelObject, IReturnProcedureModelObject<TResultsetModelObject, TResultProcedureModelObject>
		where TResultsetModelObject : class, IResultsetModelObject<TResultProcedureModelObject>
		where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
	{
		#region Constructors/Destructors

		protected ReturnProcedureModelObject()
		{
		}

		#endregion

		#region Fields/Constants

		private IEnumerable<TResultsetModelObject> resultsets;

		#endregion

		#region Properties/Indexers/Events

		public IEnumerable<TResultsetModelObject> Resultsets
		{
			get
			{
				return this.resultsets;
			}
			set
			{
				this.resultsets = value;
			}
		}

		#endregion
	}
}