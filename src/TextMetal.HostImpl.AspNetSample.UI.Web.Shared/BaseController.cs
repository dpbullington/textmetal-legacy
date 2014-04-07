/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;
using System.Web.Mvc;

using TextMetal.HostImpl.AspNetSample.DomainModel;

using TextMetal.Common.Core;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Shared
{
	[HandleError]
	public abstract class BaseController : Controller
	{
		#region Constructors/Destructors

		protected BaseController(IRepository repository)
		{
			if ((object)repository == null)
				throw new ArgumentNullException("repository");

			this.repository = repository;
		}

		#endregion

		#region Fields/Constants

		private readonly AssemblyInformation assemblyInformation = new AssemblyInformation(Assembly.GetAssembly(typeof(BaseController)));
		private readonly IRepository repository;

		#endregion

		#region Properties/Indexers/Events

		public AssemblyInformation AssemblyInformation
		{
			get
			{
				return this.assemblyInformation;
			}
		}

		protected IRepository Repository
		{
			get
			{
				return this.repository;
			}
		}

		#endregion

		#region Methods/Operators

		protected void ClearSession()
		{
			Current.Exit();
		}

		protected TViewModel CreateModel<TViewModel>()
			where TViewModel : BaseViewModel, new()
		{
			TViewModel model;

			model = new TViewModel();
			model.AssemblyInformation = this.AssemblyInformation;

			this.OnCreateModel<TViewModel>(model);

			return model;
		}

		protected abstract void OnCreateModel<TViewModel>(TViewModel model)
			where TViewModel : BaseViewModel, new();

		#endregion
	}
}