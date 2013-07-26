// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;

using NUnit.Core.Extensibility;

namespace NUnit.Core.Builders
{
	public abstract class CombiningStrategy
	{
		#region Constructors/Destructors

		public CombiningStrategy(IEnumerable[] sources)
		{
			this.sources = sources;
		}

		#endregion

		#region Fields/Constants

		protected IDataPointProvider dataPointProvider =
			(IDataPointProvider)CoreExtensions.Host.GetExtensionPoint("DataPointProviders");

		private IEnumerator[] enumerators;
		private IEnumerable[] sources;

		#endregion

		#region Properties/Indexers/Events

		public IEnumerator[] Enumerators
		{
			get
			{
				if (this.enumerators == null)
				{
					this.enumerators = new IEnumerator[this.Sources.Length];
					for (int i = 0; i < this.Sources.Length; i++)
						this.enumerators[i] = this.Sources[i].GetEnumerator();
				}

				return this.enumerators;
			}
		}

		public IEnumerable[] Sources
		{
			get
			{
				return this.sources;
			}
		}

		#endregion

		#region Methods/Operators

		public abstract IEnumerable GetTestCases();

		#endregion
	}
}