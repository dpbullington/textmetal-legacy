/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Core;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Shared
{
	public abstract class BaseViewModel
	{
		#region Constructors/Destructors

		protected BaseViewModel()
		{
		}

		#endregion

		#region Fields/Constants

		private AssemblyInformation assemblyInformation;

		#endregion

		#region Properties/Indexers/Events

		public AssemblyInformation AssemblyInformation
		{
			get
			{
				return this.assemblyInformation;
			}
			set
			{
				this.assemblyInformation = value;
			}
		}

		#endregion
	}
}