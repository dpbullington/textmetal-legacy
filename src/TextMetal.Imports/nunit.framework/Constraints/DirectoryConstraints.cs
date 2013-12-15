// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.IO;

namespace NUnit.Framework.Constraints
{
	/// <summary>
	/// EmptyDirectoryConstraint is used to test that a directory is empty
	/// </summary>
	public class EmptyDirectoryContraint : Constraint
	{
		#region Fields/Constants

		private int files = 0;
		private int subdirs = 0;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Test whether the constraint is satisfied by a given value
		/// </summary>
		/// <param name="actual"> The value to be tested </param>
		/// <returns> True for success, false for failure </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;

			DirectoryInfo dirInfo = actual as DirectoryInfo;
			if (dirInfo == null)
				throw new ArgumentException("The actual value must be a DirectoryInfo", "actual");

			this.files = dirInfo.GetFiles().Length;
			this.subdirs = dirInfo.GetDirectories().Length;

			return this.files == 0 && this.subdirs == 0;
		}

		/// <summary>
		/// Write the actual value for a failing constraint test to a
		/// MessageWriter. The default implementation simply writes
		/// the raw value of actual, leaving it to the writer to
		/// perform any formatting.
		/// </summary>
		/// <param name="writer"> The writer on which the actual value is displayed </param>
		public override void WriteActualValueTo(MessageWriter writer)
		{
			DirectoryInfo dir = this.actual as DirectoryInfo;
			if (dir == null)
				base.WriteActualValueTo(writer);
			else
			{
				writer.WriteActualValue(dir);
				writer.Write(" with {0} files and {1} directories", this.files, this.subdirs);
			}
		}

		/// <summary>
		/// Write the constraint description to a MessageWriter
		/// </summary>
		/// <param name="writer"> The writer on which the description is displayed </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("An empty directory");
		}

		#endregion
	}
}