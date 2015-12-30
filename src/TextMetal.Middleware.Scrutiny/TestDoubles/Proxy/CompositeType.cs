#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

#endregion

namespace NMock.Proxy
{
	/// <summary>
	/// Represents one or more types that are to be mocked. Provides operations
	/// that work over top of all the contained types, as well as a means of
	/// grouping and identifying unique combinations of types.
	/// </summary>
	/// <remarks>
	/// Duplicate types are ignored when added. Only interface and class types are
	/// supported, and there may only be a maximum of one class type per CompositeType instance.
	/// </remarks>
	public class CompositeType : IEquatable<CompositeType>
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the CompositeType class from the supplied types.
		/// </summary>
		/// <param name="types"> The types to include in the CompositeType. </param>
		public CompositeType(params Type[] types)
		{
			if (types == null)
				throw new ArgumentNullException("types");

			if (types.Length == 0)
				throw new ArgumentException("At least one type must be specified.", "types");

			if (types.Any(type => type == null))
				throw new ArgumentNullException("types", "null is not allowed as a type.");

			this.AdditionalInterfaceTypes = Type.EmptyTypes;

			this.Initialize(types);
		}

		/// <summary>
		/// Initializes a new instance of the CompositeType class from the supplied types.
		/// </summary>
		/// <param name="type"> The first type to include in the CompositeType. This cannot be null. </param>
		/// <param name="additionalTypesToMock"> Zero or more further types to include in the CompositeType. </param>
		/// <remarks> This constructor is mostly included for convenience. </remarks>
		public CompositeType(Type type, Type[] additionalTypesToMock)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (additionalTypesToMock == null)
				throw new ArgumentNullException("additionalTypesToMock");

			if (additionalTypesToMock.Any(t => t == null))
				throw new ArgumentNullException("additionalTypesToMock", "null is not allowed as a type.");

			this.All = additionalTypesToMock;

			this.AdditionalInterfaceTypes = Type.EmptyTypes;

			this.Add(type);
		}

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the 'primary' type we are mocking. This may be a class or an interface
		/// and will determine the proxy generation method that will be used.
		/// </summary>
		public Type PrimaryType
		{
			get
			{
				return this.All[0];
			}
		}

		/// <summary>
		/// Gets any additional types to be mocked. These will always be interfaces.
		/// </summary>
		public Type[] AdditionalInterfaceTypes
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets all types of this instance.
		/// </summary>
		public Type[] All
		{
			get;
			private set;
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Adds a <see cref="Type" /> to this instance
		/// </summary>
		/// <param name="type"> </param>
		public void Add(Type type)
		{
			var combinedTypes = new Type[this.All.Length + 1];

			combinedTypes[0] = type;
			Array.Copy(this.All, 0, combinedTypes, 1, this.All.Length);

			this.Initialize(combinedTypes);
		}

		/// <summary>
		/// Determines whether the specified CompositeType is equal to the current CompositeType.
		/// </summary>
		/// <param name="other"> The CompositeType to compare with the current CompositeType. </param>
		/// <returns> true if the specified CompositeType is equal to the current CompositeType; otherwise, false. </returns>
		public bool Equals(CompositeType other)
		{
			if (other == null)
				return false;

			if (this.All.Length != other.All.Length)
				return false;

			for (int i = 0; i < this.All.Length; i++)
			{
				if (!this.All[i].Equals(other.All[i]))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Determines whether the specified Object is equal to the current CompositeType.
		/// </summary>
		/// <param name="obj"> The Object to compare with the current CompositeType. </param>
		/// <returns> true if the specified Object is equal to the current CompositeType; otherwise, false. </returns>
		public override bool Equals(object obj)
		{
			var other = obj as CompositeType;

			return other != null && this.Equals(other);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns> An Int32 containing the hash code for this instance. </returns>
		public override int GetHashCode()
		{
			int hashCode = 23;

			foreach (Type type in this.All)
				hashCode = (hashCode * 37) + type.GetHashCode();

			return hashCode;
		}

		/// <summary>
		/// Gets the interfaces implemented by the specified type.
		/// </summary>
		/// <param name="type"> The interface type to inspect. </param>
		/// <returns> The interfaces implemented by the specified type. </returns>
		private Type[] GetInterfacesImplementedByType(Type type)
		{
			List<Type> implementedTypes = new List<Type>();

			foreach (Type implementedInterface in type.GetInterfaces())
				implementedTypes.AddRange(this.GetInterfacesImplementedByType(implementedInterface));

			implementedTypes.Add(type);

			return implementedTypes.ToArray();
		}

		/// <summary>
		/// Gets any methods of the contained type(s) that match the specified matcher.
		/// </summary>
		/// <param name="matcher"> The matcher. </param>
		/// <param name="firstMatchOnly"> if set to <c> true </c> then only the first match is returned. </param>
		/// <returns> The methods of the contained type(s) that match the specified matcher. </returns>
		/// <remarks> Only non-private methods can be matched. </remarks>
		public IList<MethodInfo> GetMatchingMethods(Matcher matcher, bool firstMatchOnly)
		{
			List<MethodInfo> matches = new List<MethodInfo>();

			foreach (Type type in this.All)
			{
				var _type = type.GetTypeInfo();

				if (_type.IsInterface)
				{
					foreach (Type implementedInterface in this.GetInterfacesImplementedByType(type))
					{
						foreach (MethodInfo methodInfo in implementedInterface.GetMethods().Where(matcher.Matches))
						{
							matches.Add(methodInfo);

							if (firstMatchOnly)
								return matches;
						}

						foreach (EventInfo eventInfo in implementedInterface.GetEvents().Where(matcher.Matches))
						{
							MethodInfo addMethod = eventInfo.GetAddMethod(false);
							MethodInfo removeMethod = eventInfo.GetRemoveMethod(false);

							if (addMethod != null && matcher.Matches(addMethod))
							{
								matches.Add(addMethod);

								if (firstMatchOnly)
									return matches;
							}

							if (removeMethod != null && matcher.Matches(removeMethod))
							{
								matches.Add(removeMethod);

								if (firstMatchOnly)
									return matches;
							}
						}
					}
				}
				else
				{
					foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(method => this.IsMethodVisible(method) && matcher.Matches(method)))
					{
						matches.Add(method);

						if (firstMatchOnly)
							return matches;
					}

					foreach (EventInfo eventInfo in type.GetEvents(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
					{
						MethodInfo addMethod = eventInfo.GetAddMethod(true);
						MethodInfo removeMethod = eventInfo.GetRemoveMethod(true);

						if (addMethod != null && matcher.Matches(addMethod))
						{
							matches.Add(addMethod);

							if (firstMatchOnly)
								return matches;
						}

						if (removeMethod != null && matcher.Matches(removeMethod))
						{
							matches.Add(removeMethod);

							if (firstMatchOnly)
								return matches;
						}
					}
				}
			}

			return matches;
		}

		/// <summary>
		/// Initializes the specified types.
		/// </summary>
		/// <param name="types"> The types. </param>
		private void Initialize(Type[] types)
		{
			// We want the types to be in a consistent order regardless
			// of the order they were orginally supplied in. We also want
			// to remove any duplicates, identify invalid types and decide
			// on which type will be considered the 'primary' type.
			this.All = this.RationalizeTypes(this.SortTypes(types));

			if (this.All.Length > 1)
			{
				this.AdditionalInterfaceTypes = new Type[this.All.Length - 1];
				Array.Copy(this.All, 1, this.AdditionalInterfaceTypes, 0, this.AdditionalInterfaceTypes.Length);
			}
		}

		/// <summary>
		/// Filters out private methods.
		/// </summary>
		/// <param name="methodInfo"> The method to test for visibility. </param>
		/// <returns> True if the method is not private, otherwise false. </returns>
		private bool IsMethodVisible(MethodInfo methodInfo)
		{
			return methodInfo.IsPublic
					|| methodInfo.IsFamily
					|| methodInfo.IsAssembly
					|| methodInfo.IsFamilyOrAssembly;
		}

		private Type[] RationalizeTypes(Type[] sortedTypes)
		{
			Type baseClass = null;
			List<Type> uniqueTypes = new List<Type>();

			foreach (Type type in sortedTypes)
			{
				var _type = type.GetTypeInfo();

				if (!_type.IsInterface)
				{
					if (_type.IsClass)
					{
						if (_type.IsSubclassOf(typeof(Delegate)))
							throw new ArgumentException("Cannot mock delegates.", "types");

						if (baseClass != null)
							throw new ArgumentException("Cannot mock more than one class type in a single mock instance.", "types");

						baseClass = type;
						uniqueTypes.Insert(0, type);
					}
					else
						throw new ArgumentException("Can only mock class and interface types. Invalid type was: " + type.Name, "types");
				}
				else
				{
					if (!uniqueTypes.Contains(type))
						uniqueTypes.Add(type);
				}
			}

			return uniqueTypes.ToArray();
		}

		private Type[] SortTypes(Type[] types)
		{
			Type[] orderedTypes = types.Clone() as Type[];

			Array.Sort(orderedTypes, (t1, t2) => string.CompareOrdinal(t1.AssemblyQualifiedName, t2.AssemblyQualifiedName));

			return orderedTypes;
		}

		/// <summary>
		/// Returns a String that represents the current CompositeType.
		/// </summary>
		/// <returns> A String that represents the current CompositeType. </returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("{");

			for (int i = 0; i < this.All.Length; i++)
			{
				if (i > 0)
					sb.Append(", ");

				sb.Append(this.All[i].Name);
			}

			return sb.Append("}").ToString();
		}

		#endregion
	}
}