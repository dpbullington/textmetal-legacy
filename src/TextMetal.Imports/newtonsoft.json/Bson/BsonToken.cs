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

using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Bson
{
	internal abstract class BsonToken
	{
		#region Properties/Indexers/Events

		public int CalculatedSize
		{
			get;
			set;
		}

		public BsonToken Parent
		{
			get;
			set;
		}

		public abstract BsonType Type
		{
			get;
		}

		#endregion
	}

	internal class BsonObject : BsonToken, IEnumerable<BsonProperty>
	{
		#region Fields/Constants

		private readonly List<BsonProperty> _children = new List<BsonProperty>();

		#endregion

		#region Properties/Indexers/Events

		public override BsonType Type
		{
			get
			{
				return BsonType.Object;
			}
		}

		#endregion

		#region Methods/Operators

		public void Add(string name, BsonToken token)
		{
			this._children.Add(new BsonProperty { Name = new BsonString(name, false), Value = token });
			token.Parent = this;
		}

		public IEnumerator<BsonProperty> GetEnumerator()
		{
			return this._children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}

	internal class BsonArray : BsonToken, IEnumerable<BsonToken>
	{
		#region Fields/Constants

		private readonly List<BsonToken> _children = new List<BsonToken>();

		#endregion

		#region Properties/Indexers/Events

		public override BsonType Type
		{
			get
			{
				return BsonType.Array;
			}
		}

		#endregion

		#region Methods/Operators

		public void Add(BsonToken token)
		{
			this._children.Add(token);
			token.Parent = this;
		}

		public IEnumerator<BsonToken> GetEnumerator()
		{
			return this._children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}

	internal class BsonValue : BsonToken
	{
		#region Constructors/Destructors

		public BsonValue(object value, BsonType type)
		{
			this._value = value;
			this._type = type;
		}

		#endregion

		#region Fields/Constants

		private readonly BsonType _type;
		private readonly object _value;

		#endregion

		#region Properties/Indexers/Events

		public override BsonType Type
		{
			get
			{
				return this._type;
			}
		}

		public object Value
		{
			get
			{
				return this._value;
			}
		}

		#endregion
	}

	internal class BsonString : BsonValue
	{
		#region Constructors/Destructors

		public BsonString(object value, bool includeLength)
			: base(value, BsonType.String)
		{
			this.IncludeLength = includeLength;
		}

		#endregion

		#region Properties/Indexers/Events

		public int ByteCount
		{
			get;
			set;
		}

		public bool IncludeLength
		{
			get;
			set;
		}

		#endregion
	}

	internal class BsonBinary : BsonValue
	{
		#region Constructors/Destructors

		public BsonBinary(byte[] value, BsonBinaryType binaryType)
			: base(value, BsonType.Binary)
		{
			this.BinaryType = binaryType;
		}

		#endregion

		#region Properties/Indexers/Events

		public BsonBinaryType BinaryType
		{
			get;
			set;
		}

		#endregion
	}

	internal class BsonRegex : BsonToken
	{
		#region Constructors/Destructors

		public BsonRegex(string pattern, string options)
		{
			this.Pattern = new BsonString(pattern, false);
			this.Options = new BsonString(options, false);
		}

		#endregion

		#region Properties/Indexers/Events

		public BsonString Options
		{
			get;
			set;
		}

		public BsonString Pattern
		{
			get;
			set;
		}

		public override BsonType Type
		{
			get
			{
				return BsonType.Regex;
			}
		}

		#endregion
	}

	internal class BsonProperty
	{
		#region Properties/Indexers/Events

		public BsonString Name
		{
			get;
			set;
		}

		public BsonToken Value
		{
			get;
			set;
		}

		#endregion
	}
}