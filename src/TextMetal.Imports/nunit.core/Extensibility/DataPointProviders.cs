// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;
using System.Reflection;

namespace NUnit.Core.Extensibility
{
	internal class DataPointProviders : ExtensionPoint, IDataPointProvider2
	{
		#region Constructors/Destructors

		public DataPointProviders(ExtensionHost host)
			: base("DataPointProviders", host)
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Return an IEnumerable providing data for use with the
		/// 	supplied parameter.
		/// </summary>
		/// <param name="parameter"> A ParameterInfo representing one argument to a parameterized test </param>
		/// <returns> An IEnumerable providing the required data </returns>
		public IEnumerable GetDataFor(ParameterInfo parameter)
		{
			ArrayList list = new ArrayList();

			foreach (IDataPointProvider provider in this.Extensions)
			{
				if (provider.HasDataFor(parameter))
				{
					foreach (object o in provider.GetDataFor(parameter))
						list.Add(o);
				}
			}

			return list;
		}

		/// <summary>
		/// 	Return an IEnumerable providing data for use with the
		/// 	supplied parameter.
		/// </summary>
		/// <param name="parameter"> A ParameterInfo representing one argument to a parameterized test </param>
		/// <returns> An IEnumerable providing the required data </returns>
		public IEnumerable GetDataFor(ParameterInfo parameter, Test suite)
		{
			ArrayList list = new ArrayList();

			foreach (IDataPointProvider provider in this.Extensions)
			{
				if (provider is IDataPointProvider2)
				{
					IDataPointProvider2 provider2 = (IDataPointProvider2)provider;
					if (provider2.HasDataFor(parameter, suite))
					{
						foreach (object o in provider2.GetDataFor(parameter, suite))
							list.Add(o);
					}
				}
				else if (provider.HasDataFor(parameter))
				{
					foreach (object o in provider.GetDataFor(parameter))
						list.Add(o);
				}
			}

			return list;
		}

		/// <summary>
		/// 	Determine whether any data is available for a parameter.
		/// </summary>
		/// <param name="parameter"> A ParameterInfo representing one argument to a parameterized test </param>
		/// <returns> True if any data is available, otherwise false. </returns>
		public bool HasDataFor(ParameterInfo parameter)
		{
			foreach (IDataPointProvider provider in this.Extensions)
			{
				if (provider.HasDataFor(parameter))
					return true;
			}

			return false;
		}

		/// <summary>
		/// 	Determine whether any data is available for a parameter.
		/// </summary>
		/// <param name="parameter"> A ParameterInfo representing one argument to a parameterized test </param>
		/// <returns> True if any data is available, otherwise false. </returns>
		public bool HasDataFor(ParameterInfo parameter, Test suite)
		{
			foreach (IDataPointProvider provider in this.Extensions)
			{
				if (provider is IDataPointProvider2)
				{
					IDataPointProvider2 provider2 = (IDataPointProvider2)provider;
					if (provider2.HasDataFor(parameter, suite))
						return true;
				}
				else if (provider.HasDataFor(parameter))
					return true;
			}

			return false;
		}

		protected override bool IsValidExtension(object extension)
		{
			return extension is IDataPointProvider;
		}

		#endregion
	}
}