// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

namespace NUnit.Core.Extensibility
{
	/// <summary>
	/// 	The IExtensionHost interface is implemented by each
	/// 	of NUnit's Extension hosts. Currently, there is
	/// 	only one host, which resides in the test domain.
	/// </summary>
	public interface IExtensionHost
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// 	Get a list of the ExtensionPoints provided by this host.
		/// </summary>
		IExtensionPoint[] ExtensionPoints
		{
			get;
		}

		/// <summary>
		/// 	Gets the ExtensionTypes supported by this host
		/// </summary>
		/// <returns> An enum indicating the ExtensionTypes supported </returns>
		ExtensionType ExtensionTypes
		{
			get;
		}

		/// <summary>
		/// 	Get an interface to the framework registry
		/// </summary>
		[Obsolete("Use the FrameworkRegistry extension point instead")]
		IFrameworkRegistry FrameworkRegistry
		{
			get;
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Return an extension point by name, if present
		/// </summary>
		/// <param name="name"> The name of the extension point </param>
		/// <returns> The extension point, if found, otherwise null </returns>
		IExtensionPoint GetExtensionPoint(string name);

		#endregion
	}
}