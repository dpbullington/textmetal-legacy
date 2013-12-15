// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace NUnit.UiKit
{
	/// <summary>
	/// AppContainer acts as the container nunit components, providing
	/// them with a Site from which services may be acquired.
	/// </summary>
	public class AppContainer : Container
	{
		#region Constructors/Destructors

		public AppContainer()
		{
			// We use a single root-level service container for the app
			this._services = new ServiceContainer();
			// Add the service containter to itself as a service!
			// This allows components to get access to the ServiceContainer
			// service in order to add services themselves.
			this._services.AddService(typeof(IServiceContainer), this._services);
		}

		#endregion

		#region Fields/Constants

		private ServiceContainer _services;

		#endregion

		#region Properties/Indexers/Events

		public IServiceContainer Services
		{
			get
			{
				return this._services;
			}
		}

		#endregion

		#region Methods/Operators

		public static AppContainer GetAppContainer(Control control)
		{
			return GetContainer(control) as AppContainer;
		}

		// Override GetService so that any components in this
		// container get access to services in the associated
		// ServiceContainer.

		public static IContainer GetContainer(Control control)
		{
			ISite site = GetSite(control);
			return site == null ? null : site.Container;
		}

		public static object GetService(Control control, Type service)
		{
			ISite site = GetSite(control);
			return site == null ? null : site.GetService(service);
		}

		public static ISite GetSite(Control control)
		{
			while (control != null && control.Site == null)
				control = control.Parent;
			return control == null ? null : control.Site;
		}

		protected override object GetService(Type service)
		{
			object s = this._services.GetService(service);
			if (s == null)
				s = base.GetService(service);
			return s;
		}

		#endregion
	}
}