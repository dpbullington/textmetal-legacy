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
	public class CombinatorialStrategy : CombiningStrategy
	{
		#region Constructors/Destructors

		public CombinatorialStrategy(IEnumerable[] sources)
			: base(sources)
		{
		}

		#endregion

		#region Methods/Operators

		public override IEnumerable GetTestCases()
		{
			IEnumerator[] enumerators = new IEnumerator[this.Sources.Length];
			int index = -1;

#if CLR_2_0 || CLR_4_0
			List<ParameterSet> testCases = new List<ParameterSet>();
#else
			ArrayList testCases = new ArrayList();
#endif

			for (;;)
			{
				while (++index < this.Sources.Length)
				{
					enumerators[index] = this.Sources[index].GetEnumerator();
					if (!enumerators[index].MoveNext())
						return testCases;
				}

				object[] testdata = new object[this.Sources.Length];

				for (int i = 0; i < this.Sources.Length; i++)
					testdata[i] = enumerators[i].Current;

				ParameterSet testCase = new ParameterSet();
				testCase.Arguments = testdata;
				testCases.Add(testCase);

				index = this.Sources.Length;

				while (--index >= 0 && !enumerators[index].MoveNext())
					;

				if (index < 0)
					break;
			}

			return testCases;
		}

		#endregion
	}
}