/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Common.Data.Framework
{
	public abstract class ResponseModelObject<TResultModel> : BasicObject, IResponseModelObject<TResultModel>
		where TResultModel : class, IResultModelObject
	{
		#region Fields/Constants

		private bool enumerationComplete;
		private IEnumerable<TResultModel> results;

		#endregion

		#region Properties/Indexers/Events

		public virtual bool EnumerationComplete
		{
			get
			{
				return this.enumerationComplete;
			}
			protected set
			{
				this.enumerationComplete = value;
			}
		}

		public IEnumerable<TResultModel> Results
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

		#region Methods/Operators

		public virtual void SignalEnumerationComplete()
		{
			this.EnumerationComplete = true;
		}

		#endregion
	}
}