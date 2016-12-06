/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Middleware.Solder.Primitives
{
	public sealed class Record : Dictionary<string, object>, IRecord
	{
		#region Constructors/Destructors

		public Record()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}

		#endregion

		#region Fields/Constants

		private object context;

		#endregion

		#region Properties/Indexers/Events

		public object Context
		{
			get
			{
				return this.context;
			}
			set
			{
				this.context = value;
			}
		}

		#endregion
	}
}