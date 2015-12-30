using System;
using System.Collections.Generic;

namespace NMock
{
	public class Expectations : List<IVerifyableExpectation>
	{
		#region Constructors/Destructors

		public Expectations()
			: base()
		{
		}

		public Expectations(int capacity)
			: base(capacity)
		{
			for (var i = 0; i < capacity; i++)
				this.Add(null);
		}

		#endregion
	}
}