/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.HostImpl.AspNetSample.Common;
using TextMetal.HostImpl.AspNetSample.DomainModel;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel
{
	public class ServiceBase
	{
		#region Constructors/Destructors

		protected ServiceBase()
			: this(Stuff.Get<IRepository>(""))
		{
		}

		protected ServiceBase(IRepository repository)
		{
			if ((object)repository == null)
				throw new ArgumentNullException("repository");

			this.repository = repository;
		}

		#endregion

		#region Fields/Constants

		private readonly IRepository repository;

		#endregion

		#region Properties/Indexers/Events

		protected IRepository Repository
		{
			get
			{
				return this.repository;
			}
		}

		#endregion
	}
}