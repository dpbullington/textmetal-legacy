// ****************************************************************
// Copyright 2002-2003, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;

namespace NUnit.Util
{
	/// <summary>
	/// 	A simple collection to hold VSProjectConfigs. Originally,
	/// 	we used the (NUnit) ProjectConfigCollection, but the
	/// 	classes have since diverged.
	/// </summary>
	public class VSProjectConfigCollection : CollectionBase
	{
		#region Properties/Indexers/Events

		public VSProjectConfig this[int index]
		{
			get
			{
				return this.List[index] as VSProjectConfig;
			}
		}

		public VSProjectConfig this[string name]
		{
			get
			{
				foreach (VSProjectConfig config in this.InnerList)
				{
					if (config.Name == name)
						return config;
				}

				return null;
			}
		}

		#endregion

		#region Methods/Operators

		public void Add(VSProjectConfig config)
		{
			this.List.Add(config);
		}

		public bool Contains(string name)
		{
			foreach (VSProjectConfig config in this.InnerList)
			{
				if (config.Name == name)
					return true;
			}

			return false;
		}

		#endregion
	}
}