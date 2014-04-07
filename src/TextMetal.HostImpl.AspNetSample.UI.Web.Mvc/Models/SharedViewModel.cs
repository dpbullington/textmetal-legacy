/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;

using TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Controllers;

using TextMetal.Common.Core;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models
{
	[Obsolete("When all view models are implemented.")]
	public sealed class SharedViewModel : TextMetalViewModel
	{
		#region Constructors/Destructors

		private SharedViewModel()
		{
		}

		#endregion

		#region Methods/Operators

		public static SharedViewModel GetInstance()
		{
			SharedViewModel model;

			model = new SharedViewModel();

			model.AssemblyInformation = new AssemblyInformation(Assembly.GetAssembly(typeof(TextMetalController)));
			model.UserWasAuthenticated = TextMetalController.UserWasAuthenticated;

			return model;
		}

		#endregion
	}
}