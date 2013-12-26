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
	public class JsonContainerContract : JsonContract
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonContainerContract" /> class.
		/// </summary>
		/// <param name="underlyingType"> The underlying type for the contract. </param>
		internal JsonContainerContract(Type underlyingType)
			: base(underlyingType)
		{
			JsonContainerAttribute jsonContainerAttribute = JsonTypeReflector.GetJsonContainerAttribute(underlyingType);

			if (jsonContainerAttribute != null)
			{
				if (jsonContainerAttribute.ItemConverterType != null)
					this.ItemConverter = JsonConverterAttribute.CreateJsonConverterInstance(jsonContainerAttribute.ItemConverterType);

				this.ItemIsReference = jsonContainerAttribute._itemIsReference;
				this.ItemReferenceLoopHandling = jsonContainerAttribute._itemReferenceLoopHandling;
				this.ItemTypeNameHandling = jsonContainerAttribute._itemTypeNameHandling;
			}
		}

		#endregion

		#region Fields/Constants

		private JsonContract _finalItemContract;
		private JsonContract _itemContract;

		#endregion

		// will be null for containers that don't have an item type (e.g. IList) or for complex objects

		// the final (i.e. can't be inherited from like a sealed class or valuetype) item contract

		#region Properties/Indexers/Events

		internal JsonContract FinalItemContract
		{
			get
			{
				return this._finalItemContract;
			}
		}

		internal JsonContract ItemContract
		{
			get
			{
				return this._itemContract;
			}
			set
			{
				this._itemContract = value;
				if (this._itemContract != null)
					this._finalItemContract = (this._itemContract.UnderlyingType.IsSealed()) ? this._itemContract : null;
				else
					this._finalItemContract = null;
			}
		}

		/// <summary>
		/// Gets or sets the default collection items <see cref="JsonConverter" />.
		/// </summary>
		/// <value> The converter. </value>
		public JsonConverter ItemConverter
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the collection items preserve object references.
		/// </summary>
		/// <value> <c> true </c> if collection items preserve object references; otherwise, <c> false </c>. </value>
		public bool? ItemIsReference
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the collection item reference loop handling.
		/// </summary>
		/// <value> The reference loop handling. </value>
		public ReferenceLoopHandling? ItemReferenceLoopHandling
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the collection item type name handling.
		/// </summary>
		/// <value> The type name handling. </value>
		public TypeNameHandling? ItemTypeNameHandling
		{
			get;
			set;
		}

		#endregion
	}
}