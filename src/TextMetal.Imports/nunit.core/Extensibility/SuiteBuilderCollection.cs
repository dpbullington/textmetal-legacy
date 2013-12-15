// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;

namespace NUnit.Core.Extensibility
{
	/// <summary>
	/// SuiteBuilderCollection is an ExtensionPoint for SuiteBuilders and
	/// implements the ISuiteBuilder interface itself, passing calls
	/// on to the individual builders.
	/// The builders are added to the collection by inserting them at
	/// the start, as to take precedence over those added earlier.
	/// </summary>
	public class SuiteBuilderCollection : ExtensionPoint, ISuiteBuilder
	{
		#region Constructors/Destructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public SuiteBuilderCollection(IExtensionHost host)
			: base("SuiteBuilders", host)
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Build a TestSuite from type provided.
		/// </summary>
		/// <param name="type"> The type of the fixture to be used </param>
		/// <returns> A TestSuite or null </returns>
		public Test BuildFrom(Type type)
		{
			foreach (ISuiteBuilder builder in this.Extensions)
			{
				if (builder.CanBuildFrom(type))
					return builder.BuildFrom(type);
			}
			return null;
		}

		/// <summary>
		/// Examine the type and determine if it is suitable for
		/// any SuiteBuilder to use in building a TestSuite
		/// </summary>
		/// <param name="type"> The type of the fixture to be used </param>
		/// <returns> True if the type can be used to build a TestSuite </returns>
		public bool CanBuildFrom(Type type)
		{
			foreach (ISuiteBuilder builder in this.Extensions)
			{
				if (builder.CanBuildFrom(type))
					return true;
			}
			return false;
		}

		protected override bool IsValidExtension(object extension)
		{
			return extension is ISuiteBuilder;
		}

		#endregion
	}
}