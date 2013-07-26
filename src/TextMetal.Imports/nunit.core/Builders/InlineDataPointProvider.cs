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
	public class InlineDataPointProvider : IDataPointProvider
	{
		#region Fields/Constants

		private static readonly string GetDataMethod = "GetData";
		private static readonly string NUnitLiteDataAttribute = "NUnit.Framework.DataAttribute";
		private static readonly string ParameterDataAttribute = "NUnit.Framework.ParameterDataAttribute";

		#endregion

		#region Methods/Operators

		public IEnumerable GetDataFor(ParameterInfo parameter)
		{
			Attribute attr = Reflect.GetAttribute(parameter, ParameterDataAttribute, false);
			if (attr == null)
				attr = Reflect.GetAttribute(parameter, NUnitLiteDataAttribute, false);
			if (attr == null)
				return null;

			MethodInfo getData = attr.GetType().GetMethod(
				GetDataMethod,
				new Type[] { typeof(ParameterInfo) });
			if (getData == null)
				return null;

			return getData.Invoke(attr, new object[] { parameter }) as IEnumerable;
		}

		public bool HasDataFor(ParameterInfo parameter)
		{
			return Reflect.HasAttribute(parameter, ParameterDataAttribute, false)
			       || Reflect.HasAttribute(parameter, NUnitLiteDataAttribute, false);
		}

		#endregion
	}
}