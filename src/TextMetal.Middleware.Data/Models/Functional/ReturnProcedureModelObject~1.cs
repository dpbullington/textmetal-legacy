/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace TextMetal.Middleware.Data.Models.Functional
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

		/// <summary>
		/// Gets an enumerable of just the first resultset result objects.
		/// </summary>
		[JsonIgnore]
		public IEnumerable<TResultProcedureModelObject> Results
		{
			get
			{
				return this.Resultsets.Where(rs => rs.Index == 0).SelectMany(rs => rs.Results);
			}
		}

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