/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel
{
	public abstract class ResponseBase<TResult> : ResponseBase
		where TResult : ResultBase, new()
	{
		#region Constructors/Destructors

		protected ResponseBase()
		{
		}

		#endregion

		#region Fields/Constants

		private IList<TResult> results;

		#endregion

		#region Properties/Indexers/Events

		public IList<TResult> Results
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