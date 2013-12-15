// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;

using NUnit.Core;
using NUnit.Core.Extensibility;

namespace NUnit.Util
{
	/// <summary>
	/// Summary description for AddinRegistry.
	/// </summary>
	public class AddinRegistry : MarshalByRefObject, IAddinRegistry, IService
	{
		#region Fields/Constants

		private ArrayList addins = new ArrayList();

		#endregion

		#region Properties/Indexers/Events

		public IList Addins
		{
			get
			{
				return this.addins;
			}
		}

		#endregion

		#region Methods/Operators

		private Addin FindAddinByName(string name)
		{
			foreach (Addin addin in this.addins)
			{
				if (addin.Name == name)
					return addin;
			}

			return null;
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public void InitializeService()
		{
		}

		public bool IsAddinRegistered(string name)
		{
			return this.FindAddinByName(name) != null;
		}

		public void Register(Addin addin)
		{
			this.addins.Add(addin);
		}

		public void SetStatus(string name, AddinStatus status, string message)
		{
			Addin addin = this.FindAddinByName(name);
			if (addin != null)
			{
				addin.Status = status;
				addin.Message = message;
			}
		}

		public void UnloadService()
		{
		}

		#endregion
	}
}