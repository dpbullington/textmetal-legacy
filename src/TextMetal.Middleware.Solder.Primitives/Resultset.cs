/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Middleware.Solder.Primitives
{
	public sealed class Resultset : IResultset
	{
		#region Constructors/Destructors

		public Resultset(int index)
		{
			this.index = index;
		}

		#endregion

		#region Fields/Constants

		private readonly int index;
		private IEnumerable<IRecord> records;
		private int? recordsAffected;

		#endregion

		#region Properties/Indexers/Events

		public int Index
		{
			get
			{
				return this.index;
			}
		}

		public IEnumerable<IRecord> Records
		{
			get
			{
				return this.records;
			}
			set
			{
				this.records = value;
			}
		}

		public int? RecordsAffected
		{
			get
			{
				return this.recordsAffected;
			}
			set
			{
				this.recordsAffected = value;
			}
		}

		#endregion
	}
}