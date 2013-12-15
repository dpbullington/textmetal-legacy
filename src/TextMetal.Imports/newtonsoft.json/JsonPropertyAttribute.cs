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

namespace Newtonsoft.Json
{
	/// <summary>
	/// Instructs the <see cref="JsonSerializer" /> to always serialize the member with the specified name.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class JsonPropertyAttribute : Attribute
	{
		// yuck. can't set nullable properties on an attribute in C#
		// have to use this approach to get an unset default state

		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonPropertyAttribute" /> class.
		/// </summary>
		public JsonPropertyAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonPropertyAttribute" /> class with the specified name.
		/// </summary>
		/// <param name="propertyName"> Name of the property. </param>
		public JsonPropertyAttribute(string propertyName)
		{
			this.PropertyName = propertyName;
		}

		#endregion

		#region Fields/Constants

		internal DefaultValueHandling? _defaultValueHandling;
		internal bool? _isReference;
		internal bool? _itemIsReference;
		internal ReferenceLoopHandling? _itemReferenceLoopHandling;
		internal TypeNameHandling? _itemTypeNameHandling;
		internal NullValueHandling? _nullValueHandling;
		internal ObjectCreationHandling? _objectCreationHandling;
		internal int? _order;
		internal ReferenceLoopHandling? _referenceLoopHandling;
		internal Required? _required;
		internal TypeNameHandling? _typeNameHandling;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets the default value handling used when serializing this property.
		/// </summary>
		/// <value> The default value handling. </value>
		public DefaultValueHandling DefaultValueHandling
		{
			get
			{
				return this._defaultValueHandling ?? default(DefaultValueHandling);
			}
			set
			{
				this._defaultValueHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets whether this property's value is serialized as a reference.
		/// </summary>
		/// <value> Whether this property's value is serialized as a reference. </value>
		public bool IsReference
		{
			get
			{
				return this._isReference ?? default(bool);
			}
			set
			{
				this._isReference = value;
			}
		}

		/// <summary>
		/// Gets or sets the converter used when serializing the property's collection items.
		/// </summary>
		/// <value> The collection's items converter. </value>
		public Type ItemConverterType
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets whether this property's collection items are serialized as a reference.
		/// </summary>
		/// <value> Whether this property's collection items are serialized as a reference. </value>
		public bool ItemIsReference
		{
			get
			{
				return this._itemIsReference ?? default(bool);
			}
			set
			{
				this._itemIsReference = value;
			}
		}

		/// <summary>
		/// Gets or sets the the reference loop handling used when serializing the property's collection items.
		/// </summary>
		/// <value> The collection's items reference loop handling. </value>
		public ReferenceLoopHandling ItemReferenceLoopHandling
		{
			get
			{
				return this._itemReferenceLoopHandling ?? default(ReferenceLoopHandling);
			}
			set
			{
				this._itemReferenceLoopHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets the the type name handling used when serializing the property's collection items.
		/// </summary>
		/// <value> The collection's items type name handling. </value>
		public TypeNameHandling ItemTypeNameHandling
		{
			get
			{
				return this._itemTypeNameHandling ?? default(TypeNameHandling);
			}
			set
			{
				this._itemTypeNameHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets the null value handling used when serializing this property.
		/// </summary>
		/// <value> The null value handling. </value>
		public NullValueHandling NullValueHandling
		{
			get
			{
				return this._nullValueHandling ?? default(NullValueHandling);
			}
			set
			{
				this._nullValueHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets the object creation handling used when deserializing this property.
		/// </summary>
		/// <value> The object creation handling. </value>
		public ObjectCreationHandling ObjectCreationHandling
		{
			get
			{
				return this._objectCreationHandling ?? default(ObjectCreationHandling);
			}
			set
			{
				this._objectCreationHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets the order of serialization and deserialization of a member.
		/// </summary>
		/// <value> The numeric order of serialization or deserialization. </value>
		public int Order
		{
			get
			{
				return this._order ?? default(int);
			}
			set
			{
				this._order = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the property.
		/// </summary>
		/// <value> The name of the property. </value>
		public string PropertyName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the reference loop handling used when serializing this property.
		/// </summary>
		/// <value> The reference loop handling. </value>
		public ReferenceLoopHandling ReferenceLoopHandling
		{
			get
			{
				return this._referenceLoopHandling ?? default(ReferenceLoopHandling);
			}
			set
			{
				this._referenceLoopHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this property is required.
		/// </summary>
		/// <value>
		/// A value indicating whether this property is required.
		/// </value>
		public Required Required
		{
			get
			{
				return this._required ?? Required.Default;
			}
			set
			{
				this._required = value;
			}
		}

		/// <summary>
		/// Gets or sets the type name handling used when serializing this property.
		/// </summary>
		/// <value> The type name handling. </value>
		public TypeNameHandling TypeNameHandling
		{
			get
			{
				return this._typeNameHandling ?? default(TypeNameHandling);
			}
			set
			{
				this._typeNameHandling = value;
			}
		}

		#endregion
	}
}