#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using NMock.Monitoring;

#endregion

namespace NMock.Matchers
{
	/// <summary>
	/// Matcher that checks whether parameters of a method match with the specified list of matchers.
	/// </summary>
	public class GenericMethodTypeParametersMatcher : Matcher
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericMethodTypeParametersMatcher" /> class and specifies no generic types.
		/// </summary>
		public GenericMethodTypeParametersMatcher()
		{
			this._typeMatchers = new Matcher[0];
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericMethodTypeParametersMatcher" /> class.
		/// </summary>
		/// <param name="typeMatchers"> The value matchers. This is an ordered list of matchers, each matching a single method argument. </param>
		public GenericMethodTypeParametersMatcher(params Matcher[] typeMatchers)
		{
			this._typeMatchers = typeMatchers;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericMethodTypeParametersMatcher" /> class and specifies the types to match.
		/// </summary>
		/// <param name="genericTypeParameters"> The types to match.  (Typically a result of the <see cref="MethodInfo.GetGenericArguments" /> method. </param>
		public GenericMethodTypeParametersMatcher(Type[] genericTypeParameters)
		{
			List<Matcher> typeMatchers = new List<Matcher>();

			foreach (Type type in genericTypeParameters)
				typeMatchers.Add(new TypeMatcher(type));

			this._typeMatchers = typeMatchers.ToArray();
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// An ordered list of type <see cref="Matcher" />'s each matching a single method argument.
		/// </summary>
		private readonly Matcher[] _typeMatchers;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Describes this matcher.
		/// </summary>
		/// <param name="writer"> The text writer to which the description is added. </param>
		public override void DescribeTo(TextWriter writer)
		{
			if (this._typeMatchers.Length > 0)
			{
				writer.Write("<");
				this.WriteListOfMatchers(this._typeMatchers.Length, writer);
				writer.Write(">");
			}
		}

		/// <summary>
		/// Matches the specified object to this matcher and returns whether it matches.
		/// </summary>
		/// <param name="o"> The object to match. </param>
		/// <returns> Whether the object is an <see cref="Invocation" /> and all method arguments match their corresponding matcher. </returns>
		public override bool Matches(object o)
		{
			return o is Invocation && this.MatchesTypes(((Invocation)o).Method);
		}

		/// <summary>
		/// Determines whether the arguments of the invocation matches the initial arguments.
		/// </summary>
		/// <param name="methodInfo"> The invocation to match against the initial arguments. </param>
		/// <returns>
		/// Returns true if invocation matches the initial arguments; false otherwise.
		/// </returns>
		internal bool MatchesTypes(MethodInfo methodInfo)
		{
			return methodInfo.GetGenericArguments().Length == this._typeMatchers.Length && this.MatchesTypeValues(methodInfo);
		}

		/// <summary>
		/// Determines whether all argument types of the generic method matches the invocation.
		/// </summary>
		/// <param name="methodInfo"> The invocation to match against the initial argument types. </param>
		/// <returns>
		/// Returns true if invocation types matches the inital argument types; false otherwise.
		/// </returns>
		private bool MatchesTypeValues(MethodBase methodInfo)
		{
			var types = methodInfo.GetGenericArguments();

			return !types.Where((t, i) => !this._typeMatchers[i].Matches(t)).Any();
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

				this._typeMatchers[i].DescribeTo(writer);
			}
		}

		#endregion
	}
}