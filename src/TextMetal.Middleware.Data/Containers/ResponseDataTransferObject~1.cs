/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Middleware.Data.Containers
{
	public abstract class ResponseDataTransferObject<TResultServiceModelObject> : ResponseDataTransferObject, IResponseDataTransferObject<TResultServiceModelObject>
		where TResultServiceModelObject : ResultDataTransferObject, new()
	{
		#region Constructors/Destructors

		protected ResponseDataTransferObject()
		{
		}

		#endregion

		#region Fields/Constants

		private List<TResultServiceModelObject> results;

		#endregion

		#region Properties/Indexers/Events

		public List<TResultServiceModelObject> Results
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