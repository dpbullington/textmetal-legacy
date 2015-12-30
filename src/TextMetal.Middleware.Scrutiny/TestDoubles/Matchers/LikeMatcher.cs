#region Using

using System;
using System.IO;

#endregion

namespace NMock.Matchers
{
	/// <summary>
	/// Matches 2 objects using IComparable
	/// </summary>
	public class ObjectMatcher : Matcher
	{
		#region Constructors/Destructors

		/// <summary>
		/// </summary>
		/// <param name="o"> </param>
		public ObjectMatcher(object o)
		{
			this._o = o;
		}

		#endregion

		#region Fields/Constants

		private readonly object _o;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Describes this matcher.
		/// </summary>
		/// <param name="writer"> The text writer to which the description is added. </param>
		public override void DescribeTo(TextWriter writer)
		{
			if (this._o == null)
				writer.Write("Object in ObjectMatcher is null.");
			else
				writer.Write(this._o.ToString());
		}

		/// <summary>
		/// Compares 2 objects using IComparable
		/// </summary>
		/// <param name="o"> </param>
		/// <returns> </returns>
		public override bool Matches(object o)
		{
			if (o == null)
				throw new ArgumentNullException("o");

			if (this._o == null)
				throw new InvalidOperationException("The object of comparison is null.");

			if (o as IComparable == null)
				throw new InvalidOperationException(string.Format("The object being compared does not support IComparable.  Type: {0}", o.GetType()));

			if (this._o as IComparable == null)
				throw new InvalidOperationException(string.Format("The object of comparison does not support IComparable.  Type: {0}", this._o.GetType()));

			return ((IComparable)this._o).CompareTo(o) == 0;
		}

		#endregion
	}
}