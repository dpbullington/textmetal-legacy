// ****************************************************************
// Copyright 2002-2003, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections.Specialized;

namespace NUnit.Util
{
	/// <summary>
	/// 	Originally, we used the same ProjectConfig class for both
	/// 	NUnit and Visual Studio projects. Since we really do very
	/// 	little with VS Projects, this class has been created to 
	/// 	hold the name and the collection of assembly paths.
	/// </summary>
	public class VSProjectConfig
	{
		#region Constructors/Destructors

		public VSProjectConfig(string name)
		{
			this.name = name;
		}

		#endregion

		#region Fields/Constants

		private StringCollection assemblies = new StringCollection();
		private string name;

		#endregion

		#region Properties/Indexers/Events

		public StringCollection Assemblies
		{
			get
			{
				return this.assemblies;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		#endregion
	}
}