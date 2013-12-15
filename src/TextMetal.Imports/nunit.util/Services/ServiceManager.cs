// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;

using NUnit.Core;

namespace NUnit.Util
{
	/// <summary>
	/// Summary description for ServiceManger.
	/// </summary>
	public class ServiceManager
	{
		#region Constructors/Destructors

		private ServiceManager()
		{
		}

		#endregion

		#region Fields/Constants

		private static ServiceManager defaultServiceManager = new ServiceManager();

		private static Logger log = InternalTrace.GetLogger(typeof(ServiceManager));
		private Hashtable serviceIndex = new Hashtable();
		private ArrayList services = new ArrayList();

		#endregion

		#region Properties/Indexers/Events

		public static ServiceManager Services
		{
			get
			{
				return defaultServiceManager;
			}
		}

		#endregion

		#region Methods/Operators

		public void AddService(IService service)
		{
			this.services.Add(service);
			log.Debug("Added " + service.GetType().Name);
		}

		public void ClearServices()
		{
			log.Info("Clearing Service list");
			this.services.Clear();
		}

		public IService GetService(Type serviceType)
		{
			IService theService = (IService)this.serviceIndex[serviceType];
			if (theService == null)
			{
				foreach (IService service in this.services)
				{
					// TODO: Does this work on Mono?
					if (serviceType.IsInstanceOfType(service))
					{
						this.serviceIndex[serviceType] = service;
						theService = service;
						break;
					}
				}
			}

			if (theService == null)
				log.Error(string.Format("Requested service {0} was not found", serviceType.FullName));
			else
				log.Debug(string.Format("Request for service {0} satisfied by {1}", serviceType.Name, theService.GetType().Name));

			return theService;
		}

		public void InitializeServices()
		{
			foreach (IService service in this.services)
			{
				log.Info("Initializing " + service.GetType().Name);
				try
				{
					service.InitializeService();
				}
				catch (Exception ex)
				{
					log.Error("Failed to initialize service", ex);
				}
			}
		}

		public void StopAllServices()
		{
			// Stop services in reverse of initialization order
			// TODO: Deal with dependencies explicitly
			int index = this.services.Count;
			while (--index >= 0)
			{
				IService service = this.services[index] as IService;
				log.Info("Stopping " + service.GetType().Name);
				try
				{
					service.UnloadService();
				}
				catch (Exception ex)
				{
					log.Error("Failure stopping service", ex);
				}
			}
		}

		#endregion
	}
}