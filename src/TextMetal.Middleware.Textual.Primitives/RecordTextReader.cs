/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;
using System.IO;

using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Textual.Primitives
{
	public abstract class RecordTextReader : WrappedTextReader
	{
		#region Constructors/Destructors

		protected RecordTextReader(TextReader innerTextReader)
			: base(innerTextReader)
		{
		}

		#endregion

		#region Methods/Operators

		public abstract IEnumerable<ITextHeaderSpec> ReadHeaderSpecs();

		public abstract IEnumerable<IRecord> ReadRecords();

		#endregion
	}
}