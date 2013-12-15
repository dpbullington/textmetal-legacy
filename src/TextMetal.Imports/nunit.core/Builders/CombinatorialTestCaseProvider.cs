// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;
using System.Reflection;

using NUnit.Core.Extensibility;

namespace NUnit.Core.Builders
{
	public class CombinatorialTestCaseProvider : ITestCaseProvider2
	{
		//static readonly string CombinatorialAttribute = "NUnit.Framework.CombinatorialAttribute";

		#region Fields/Constants

		private static readonly string PairwiseAttribute = "NUnit.Framework.PairwiseAttribute";
		private static readonly string SequentialAttribute = "NUnit.Framework.SequentialAttribute";

		private static IDataPointProvider2 dataPointProvider =
			(IDataPointProvider2)CoreExtensions.Host.GetExtensionPoint("DataPointProviders");

		#endregion

		#region Methods/Operators

		private CombiningStrategy GetStrategy(MethodInfo method, Test suite)
		{
			ParameterInfo[] parameters = method.GetParameters();
			IEnumerable[] sources = new IEnumerable[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
				sources[i] = dataPointProvider.GetDataFor(parameters[i], suite);

			if (Reflect.HasAttribute(method, SequentialAttribute, false))
				return new SequentialStrategy(sources);

			if (Reflect.HasAttribute(method, PairwiseAttribute, false) &&
				method.GetParameters().Length > 2)
				return new PairwiseStrategy(sources);

			return new CombinatorialStrategy(sources);
		}

		public IEnumerable GetTestCasesFor(MethodInfo method)
		{
			return this.GetStrategy(method, null).GetTestCases();
		}

		public IEnumerable GetTestCasesFor(MethodInfo method, Test suite)
		{
			return this.GetStrategy(method, suite).GetTestCases();
		}

		public bool HasTestCasesFor(MethodInfo method)
		{
			if (method.GetParameters().Length == 0)
				return false;

			foreach (ParameterInfo parameter in method.GetParameters())
			{
				if (!dataPointProvider.HasDataFor(parameter))
					return false;
			}

			return true;
		}

		public bool HasTestCasesFor(MethodInfo method, Test suite)
		{
			return this.HasTestCasesFor(method);
		}

		#endregion
	}
}