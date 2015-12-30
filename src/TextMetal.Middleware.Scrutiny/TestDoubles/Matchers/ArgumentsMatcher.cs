#region Using

using System.Collections.Generic;
using System.IO;
using System.Reflection;

using NMock.Monitoring;

#endregion

namespace NMock.Matchers
{
	/// <summary>
	/// Matcher that checks whether parameters of a method match with the specified list of matchers.
	/// </summary>
	public class ArgumentsMatcher : Matcher
	{
		#region Constructors/Destructors

		/// <summary>
		/// Creates an instance of the <see cref="ArgumentsMatcher" /> class.
		/// </summary>
		public ArgumentsMatcher()
		{
			this.valueMatchers = new List<Matcher>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ArgumentsMatcher" /> class.
		/// </summary>
		/// <param name="valueMatchers"> The value matchers. This is an ordered list of matchers, each matching a single method argument. </param>
		public ArgumentsMatcher(params Matcher[] valueMatchers)
		{
			this.valueMatchers = new List<Matcher>(valueMatchers);
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Stores the out parameter.
		/// </summary>
		private static readonly object OutParameter = new object();

		/// <summary>
		/// Stores the valuematchers given at initialization.
		/// </summary>
		private readonly List<Matcher> valueMatchers;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets a value matcher by index
		/// </summary>
		/// <param name="index"> The index of the item to return </param>
		/// <returns> A value matcher </returns>
		public Matcher this[int index]
		{
			get
			{
				return this.valueMatchers[index];
			}
		}

		/// <summary>
		/// The number of value matchers
		/// </summary>
		public int Count
		{
			get
			{
				return this.valueMatchers.Count;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Adds a matcher to the arguments matcher
		/// </summary>
		/// <param name="matcher"> </param>
		public void AddMatcher(Matcher matcher)
		{
			this.valueMatchers.Add(matcher);
		}

		/// <summary>
		/// Describes this matcher.
		/// </summary>
		/// <param name="writer"> The text writer to which the description is added. </param>
		public override void DescribeTo(TextWriter writer)
		{
			if (!(this.Count == 1 && this.valueMatchers[0] is DelegateMatcher))
				writer.Write("(");

			this.WriteListOfMatchers(this.Count, writer);

			if (!(this.Count == 1 && this.valueMatchers[0] is DelegateMatcher))
				writer.Write(")");
		}

		/// <summary>
		/// Returns the last argument matcher.
		/// </summary>
		/// <returns> Argument matcher </returns>
		protected Matcher LastMatcher()
		{
			return this.valueMatchers[this.valueMatchers.Count - 1];
		}

		/// <summary>
		/// Matches the specified object to this matcher and returns whether it matches.
		/// </summary>
		/// <param name="o"> The object to match. </param>
		/// <returns> Whether the object is an <see cref="Invocation" /> and all method arguments match their corresponding matcher. </returns>
		public override bool Matches(object o)
		{
			return o is Invocation && this.MatchesArguments((Invocation)o);
		}

		private bool MatchesArguments(Invocation invocation)
		{
			return invocation.Parameters.Count == this.valueMatchers.Count
					&& this.MatchesArgumentValues(invocation);
		}

		private bool MatchesArgumentValues(Invocation invocation)
		{
			ParameterInfo[] paramsInfo = invocation.MethodParameters;

			for (int i = 0; i < invocation.Parameters.Count; i++)
			{
				object value = paramsInfo[i].IsOut ? OutParameter : invocation.Parameters[i];

				if (!this.valueMatchers[i].Matches(value))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Writes the list of matchers to a <see cref="TextWriter" />.
		/// </summary>
		/// <param name="listLength"> Length of the list. </param>
		/// <param name="writer"> The writer. </param>
		protected void WriteListOfMatchers(int listLength, TextWriter writer)
		{
			for (int i = 0; i < listLength; i++)
			{
				if (i > 0)
					writer.Write(", ");

				this.valueMatchers[i].DescribeTo(writer);
			}
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		/// <summary>
		/// Matcher that matches method out parameters.
		/// </summary>
		public class OutMatcher : Matcher
		{
			#region Methods/Operators

			/// <summary>
			/// Describes this object.
			/// </summary>
			/// <param name="writer"> The text writer the description is added to. </param>
			public override void DescribeTo(TextWriter writer)
			{
				writer.Write("out");
			}

			/// <summary>
			/// Matches the specified object to this matcher and returns whether it matches.
			/// </summary>
			/// <param name="o"> The object to match. </param>
			/// <returns> Whether the object mached is an out parameter. </returns>
			public override bool Matches(object o)
			{
				return o == OutParameter;
			}

			#endregion
		}

		#endregion
	}
}