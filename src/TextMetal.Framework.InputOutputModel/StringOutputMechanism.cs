/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;

using TextMetal.Common.Core;

namespace TextMetal.Framework.InputOutputModel
{
	public class StringOutputMechanism : OutputMechanism
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the StringOutputMechanism class.
		/// </summary>
		public StringOutputMechanism()
		{
			this.RecycleOutput();
		}

		#endregion

		#region Methods/Operators

		protected override void CoreEnter(string scopeName, bool appendMode)
		{
			if ((object)scopeName == null)
				throw new ArgumentNullException("scopeName");

			if (DataType.IsWhiteSpace(scopeName))
				throw new ArgumentOutOfRangeException("scopeName");
		}

		protected override void CoreLeave(string scopeName)
		{
			if ((object)scopeName == null)
				throw new ArgumentNullException("scopeName");

			if (DataType.IsWhiteSpace(scopeName))
				throw new ArgumentOutOfRangeException("scopeName");
		}

		protected override void CoreWriteObject(object obj, string objectName)
		{
			if ((object)obj == null)
				throw new ArgumentNullException("obj");

			if ((object)objectName == null)
				throw new ArgumentNullException("objectName");

			if (DataType.IsWhiteSpace(objectName))
				throw new ArgumentOutOfRangeException("objectName");
		}

		public string RecycleOutput()
		{
			string value = null;

			if (base.TextWriters.Count > 0)
			{
				base.CurrentTextWriter.Flush();

				value = base.CurrentTextWriter.ToString();

				base.TextWriters.Pop().Dispose();
			}

			base.TextWriters.Push(new StringWriter());

			return value;
		}

		#endregion
	}
}