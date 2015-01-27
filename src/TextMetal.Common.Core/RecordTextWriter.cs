/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;
using System.IO;

namespace TextMetal.Common.Core
{
	public abstract class RecordTextWriter : WrappedTextWriter
	{
		#region Constructors/Destructors

		protected RecordTextWriter(TextWriter innerTextWriter)
			: base(innerTextWriter)
		{
		}

		#endregion

		#region Methods/Operators

		public abstract void WriteRecords(IEnumerable<IDictionary<string, object>> records);

		#endregion
	}
}