// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;

namespace NUnit.Core
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class TestBuilderAttribute : Attribute
	{
		#region Constructors/Destructors

		public TestBuilderAttribute(Type builderType)
		{
			this.builderType = builderType;
		}

		#endregion

		#region Fields/Constants

		private Type builderType;

		#endregion

		#region Properties/Indexers/Events

		public Type BuilderType
		{
			get
			{
				return this.builderType;
			}
		}

		#endregion
	}
}