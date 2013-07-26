// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Core.Extensibility;
#if CLR_2_0 || CLR_4_0
#endif

namespace NUnit.Core.Builders
{
	public class SequentialStrategy : CombiningStrategy
	{
		#region Constructors/Destructors

		public SequentialStrategy(IEnumerable[] sources)
			: base(sources)
		{
		}

		#endregion

		#region Methods/Operators

		public override IEnumerable GetTestCases()
		{
#if CLR_2_0 || CLR_4_0
			List<ParameterSet> testCases = new List<ParameterSet>();
#else
            ArrayList testCases = new ArrayList();
#endif

			for (;;)
			{
				bool gotData = false;
				object[] testdata = new object[this.Sources.Length];

				for (int i = 0; i < this.Sources.Length; i++)
				{
					if (this.Enumerators[i].MoveNext())
					{
						testdata[i] = this.Enumerators[i].Current;
						gotData = true;
					}
					else
						testdata[i] = null;
				}

				if (!gotData)
					break;

				ParameterSet testcase = new ParameterSet();
				testcase.Arguments = testdata;

				testCases.Add(testcase);
			}

			return testCases;
		}

		#endregion
	}
}