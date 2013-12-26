#region License

// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

using Newtonsoft.Json.Utilities;

#if NET20
using Newtonsoft.Json.Utilities.LinqBridge;
#else

#endif

namespace Newtonsoft.Json.Serialization
{
	/// <summary>
	/// Contract details for a <see cref="Type" /> used by the <see cref="JsonSerializer" />.
	/// </summary>
	public class JsonArrayContract : JsonContainerContract
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonArrayContract" /> class.
		/// </summary>
		/// <param name="underlyingType"> The underlying type for the contract. </param>
		public JsonArrayContract(Type underlyingType)
			: base(underlyingType)
		{
			this.ContractType = JsonContractType.Array;

			bool canDeserialize;

			Type tempCollectionType;
			if (this.CreatedType.IsArray)
			{
				this.CollectionItemType = ReflectionUtils.GetCollectionItemType(this.UnderlyingType);
				this.IsReadOnlyOrFixedSize = true;
				this._genericCollectionDefinitionType = typeof(List<>).MakeGenericType(this.CollectionItemType);

				canDeserialize = true;
				this.IsMultidimensionalArray = (this.UnderlyingType.IsArray && this.UnderlyingType.GetArrayRank() > 1);
			}
			else if (typeof(IList).IsAssignableFrom(underlyingType))
			{
				if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(ICollection<>), out this._genericCollectionDefinitionType))
					this.CollectionItemType = this._genericCollectionDefinitionType.GetGenericArguments()[0];
				else
					this.CollectionItemType = ReflectionUtils.GetCollectionItemType(underlyingType);

				if (underlyingType == typeof(IList))
					this.CreatedType = typeof(List<object>);

				if (this.CollectionItemType != null)
					this.ParametrizedConstructor = CollectionUtils.ResolveEnumableCollectionConstructor(underlyingType, this.CollectionItemType);

				this.IsReadOnlyOrFixedSize = ReflectionUtils.InheritsGenericDefinition(underlyingType, typeof(ReadOnlyCollection<>));
				canDeserialize = true;
			}
			else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(ICollection<>), out this._genericCollectionDefinitionType))
			{
				this.CollectionItemType = this._genericCollectionDefinitionType.GetGenericArguments()[0];

				if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof(ICollection<>))
					|| ReflectionUtils.IsGenericDefinition(underlyingType, typeof(IList<>)))
					this.CreatedType = typeof(List<>).MakeGenericType(this.CollectionItemType);

#if !(NET20 || NET35 || PORTABLE40)
        if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof(ISet<>)))
          CreatedType = typeof(HashSet<>).MakeGenericType(CollectionItemType);
#endif

				this.ParametrizedConstructor = CollectionUtils.ResolveEnumableCollectionConstructor(underlyingType, this.CollectionItemType);
				canDeserialize = true;
				this.ShouldCreateWrapper = true;
			}
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
      else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof (IReadOnlyCollection<>), out tempCollectionType))
      {
        CollectionItemType = underlyingType.GetGenericArguments()[0];

        if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof (IReadOnlyCollection<>))
          || ReflectionUtils.IsGenericDefinition(underlyingType, typeof (IReadOnlyList<>)))
          CreatedType = typeof(ReadOnlyCollection<>).MakeGenericType(CollectionItemType);

        _genericCollectionDefinitionType = typeof(List<>).MakeGenericType(CollectionItemType);
        ParametrizedConstructor = CollectionUtils.ResolveEnumableCollectionConstructor(CreatedType, CollectionItemType);
        IsReadOnlyOrFixedSize = true;
        canDeserialize = (ParametrizedConstructor != null);
      }
#endif
			else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IEnumerable<>), out tempCollectionType))
			{
				this.CollectionItemType = tempCollectionType.GetGenericArguments()[0];

				if (ReflectionUtils.IsGenericDefinition(this.UnderlyingType, typeof(IEnumerable<>)))
					this.CreatedType = typeof(List<>).MakeGenericType(this.CollectionItemType);

				this.ParametrizedConstructor = CollectionUtils.ResolveEnumableCollectionConstructor(underlyingType, this.CollectionItemType);

				if (underlyingType.IsGenericType() && underlyingType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					this._genericCollectionDefinitionType = tempCollectionType;

					this.IsReadOnlyOrFixedSize = false;
					this.ShouldCreateWrapper = false;
					canDeserialize = true;
				}
				else
				{
					this._genericCollectionDefinitionType = typeof(List<>).MakeGenericType(this.CollectionItemType);

					this.IsReadOnlyOrFixedSize = true;
					this.ShouldCreateWrapper = true;
					canDeserialize = (this.ParametrizedConstructor != null);
				}
			}
			else
			{
				// types that implement IEnumerable and nothing else
				canDeserialize = false;
				this.ShouldCreateWrapper = true;
			}

			this.CanDeserialize = canDeserialize;

			if (this.CollectionItemType != null)
				this._isCollectionItemTypeNullableType = ReflectionUtils.IsNullableType(this.CollectionItemType);

#if (NET20 || NET35)
	// bug in .NET 2.0 & 3.5 that List<Nullable<T>> throws an error when adding null via IList.Add(object)
	// wrapper will handle calling Add(T) instead
      if (_isCollectionItemTypeNullableType
        && (ReflectionUtils.InheritsGenericDefinition(CreatedType, typeof(List<>), out tempCollectionType)
        || (CreatedType.IsArray && !IsMultidimensionalArray)))
      {
        ShouldCreateWrapper = true;
      }
#endif
		}

		#endregion

		#region Fields/Constants

		private readonly Type _genericCollectionDefinitionType;
		private readonly bool _isCollectionItemTypeNullableType;

		private Func<object> _genericTemporaryCollectionCreator;
		private MethodCall<object, object> _genericWrapperCreator;
		private Type _genericWrapperType;

		#endregion

		#region Properties/Indexers/Events

		internal bool CanDeserialize
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the <see cref="Type" /> of the collection items.
		/// </summary>
		/// <value> The <see cref="Type" /> of the collection items. </value>
		public Type CollectionItemType
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a value indicating whether the collection type is a multidimensional array.
		/// </summary>
		/// <value> <c> true </c> if the collection type is a multidimensional array; otherwise, <c> false </c>. </value>
		public bool IsMultidimensionalArray
		{
			get;
			private set;
		}

		internal ConstructorInfo ParametrizedConstructor
		{
			get;
			private set;
		}

		internal bool ShouldCreateWrapper
		{
			get;
			private set;
		}

		#endregion

		#region Methods/Operators

		internal IList CreateTemporaryCollection()
		{
			if (this._genericTemporaryCollectionCreator == null)
			{
				// multidimensional array will also have array instances in it
				Type collectionItemType = (this.IsMultidimensionalArray) ? typeof(object) : this.CollectionItemType;
				Type temporaryListType = typeof(List<>).MakeGenericType(collectionItemType);
				this._genericTemporaryCollectionCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(temporaryListType);
			}

			return (IList)this._genericTemporaryCollectionCreator();
		}

		internal IWrappedCollection CreateWrapper(object list)
		{
			if (this._genericWrapperCreator == null)
			{
				this._genericWrapperType = typeof(CollectionWrapper<>).MakeGenericType(this.CollectionItemType);

				Type constructorArgument;

				if (ReflectionUtils.InheritsGenericDefinition(this._genericCollectionDefinitionType, typeof(List<>))
					|| this._genericCollectionDefinitionType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
					constructorArgument = typeof(ICollection<>).MakeGenericType(this.CollectionItemType);
				else
					constructorArgument = this._genericCollectionDefinitionType;

				ConstructorInfo genericWrapperConstructor = this._genericWrapperType.GetConstructor(new[] { constructorArgument });
				this._genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(genericWrapperConstructor);
			}

			return (IWrappedCollection)this._genericWrapperCreator(null, list);
		}

		#endregion
	}
}