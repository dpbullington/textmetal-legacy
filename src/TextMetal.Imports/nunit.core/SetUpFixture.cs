// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;

namespace NUnit.Core
{
	/// <summary>
	/// SetUpFixture extends TestSuite and supports
	/// Setup and TearDown methods.
	/// </summary>
	public class SetUpFixture : TestSuite
	{
		#region Constructors/Destructors

		public SetUpFixture(Type type)
			: base(type)
		{
			this.TestName.Name = type.Namespace;
			if (this.TestName.Name == null)
				this.TestName.Name = "[default namespace]";
			int index = this.TestName.Name.LastIndexOf('.');
			if (index > 0)
				this.TestName.Name = this.TestName.Name.Substring(index + 1);

			this.fixtureSetUpMethods = Reflect.GetMethodsWithAttribute(type, NUnitFramework.SetUpAttribute, true);
			this.fixtureTearDownMethods = Reflect.GetMethodsWithAttribute(type, NUnitFramework.TearDownAttribute, true);

#if CLR_2_0 || CLR_4_0
			this.actions = ActionsHelper.GetActionsFromTypesAttributes(type);
#endif
		}

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets a string representing the kind of test
		/// that this object represents, for use in display.
		/// </summary>
		public override string TestType
		{
			get
			{
				return "SetUpFixture";
			}
		}

		#endregion

		#region Methods/Operators

		public override TestResult Run(EventListener listener, ITestFilter filter)
		{
			using (new DirectorySwapper(AssemblyHelper.GetDirectoryName(this.FixtureType.Assembly)))
				return base.Run(listener, filter);
		}

		#endregion
	}
}