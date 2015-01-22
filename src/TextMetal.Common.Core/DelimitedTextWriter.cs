/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Text;

namespace TextMetal.Common.Core
{
	public sealed class DelimitedTextWriter : TextWriter
	{
		#region Constructors/Destructors

		public DelimitedTextWriter(TextWriter innerTextWriter, DelimitedTextSpec delimitedTextSpec)
		{
			if ((object)innerTextWriter == null)
				throw new ArgumentNullException("innerTextWriter");

			if ((object)delimitedTextSpec == null)
				throw new ArgumentNullException("delimitedTextSpec");

			this.innerTextWriter = innerTextWriter;
			this.delimitedTextSpec = delimitedTextSpec;
		}

		#endregion

		#region Fields/Constants

		private readonly DelimitedTextSpec delimitedTextSpec;
		private readonly TextWriter innerTextWriter;

		#endregion

		#region Properties/Indexers/Events

		public DelimitedTextSpec DelimitedTextSpec
		{
			get
			{
				return this.delimitedTextSpec;
			}
		}

		public override Encoding Encoding
		{
			get
			{
				return this.InnerTextWriter.Encoding;
			}
		}

		public TextWriter InnerTextWriter
		{
			get
			{
				return this.innerTextWriter;
			}
		}

		#endregion

		#region Methods/Operators

		public override void Close()
		{
			if ((object)this.InnerTextWriter != null)
				this.InnerTextWriter.Close();

			base.Close();
		}

		#endregion
	}
}