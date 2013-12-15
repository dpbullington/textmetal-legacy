// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;

using NUnit.Core.Extensibility;

namespace NUnit.Core
{
	/// <summary>
	/// ExtensionHost is the abstract base class used for
	/// all extension hosts. It provides an array of
	/// extension points and a FrameworkRegistry and
	/// implements the IExtensionHost interface. Derived
	/// classes must initialize the extension points.
	/// </summary>
	public abstract class ExtensionHost : IExtensionHost
	{
		#region Fields/Constants

		protected ArrayList extensions;

		protected ExtensionType supportedTypes;

		#endregion

		#region Properties/Indexers/Events

		public IExtensionPoint[] ExtensionPoints
		{
			get
			{
				return (IExtensionPoint[])this.extensions.ToArray(typeof(IExtensionPoint));
			}
		}

		public ExtensionType ExtensionTypes
		{
			get
			{
				return this.supportedTypes;
			}
		}

		public IFrameworkRegistry FrameworkRegistry
		{
			get
			{
				return (IFrameworkRegistry)this.GetExtensionPoint("FrameworkRegistry");
			}
		}

		#endregion

		#region Methods/Operators

		public IExtensionPoint GetExtensionPoint(string name)
		{
			foreach (IExtensionPoint extensionPoint in this.extensions)
			{
				if (extensionPoint.Name == name)
					return extensionPoint;
			}

			return null;
		}

		#endregion
	}
}