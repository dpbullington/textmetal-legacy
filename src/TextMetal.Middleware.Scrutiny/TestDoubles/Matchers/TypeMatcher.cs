#region Using

using System;
using System.IO;
using System.Reflection;

using NMock.Monitoring;

#endregion

namespace NMock.Matchers
{
	/// <summary>
	/// Matcher that checks whether the actual object can be assigned to the expected type.
	/// </summary>
	public class TypeMatcher : Matcher
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeMatcher" /> class.
		/// </summary>
		/// <param name="type"> The expected type. </param>
		public TypeMatcher(Type type)
		{
			this.type = type;
		}

		#endregion

		#region Fields/Constants

		private readonly Type type;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Describes this matcher.
		/// </summary>
		/// <param name="writer"> The text writer to which the description is added. </param>
		public override void DescribeTo(TextWriter writer)
		{
			//if (!type.IsValueType && type != typeof(string))
			//    writer.Write("type assignable to ");
			writer.Write(this.type.FullName);
		}

		/// <summary>
		/// Matches the specified object to this matcher and returns whether it matches.
		/// </summary>
		/// <param name="o"> The object to match. </param>
		/// <returns> Whether the object castable to the expected type. </returns>
		public override bool Matches(object o)
		{
			if (this.type == null && o == null)
				return true;

			if (this.type == null || o == null)
				return false;

			if (o is Type && this.type == (Type)o)
				return true;

			var _type = this.type.GetTypeInfo();
			var _o = o.GetType().GetTypeInfo();

			if (_type.IsAssignableFrom(_o))
				return true;

			if (o is Invocation)
			{
				Invocation invocation = o as Invocation;

				var _invocation = invocation.MethodReturnType.GetTypeInfo();

				return _type.IsAssignableFrom(_invocation);
			}

			return false;
		}

		#endregion
	}
}