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
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json
{
	/// <summary>
	/// Specifies the settings on a <see cref="JsonSerializer" /> object.
	/// </summary>
	public class JsonSerializerSettings
	{
		#region Constructors/Destructors

		static JsonSerializerSettings()
		{
			DefaultContext = new StreamingContext();
			DefaultCulture = CultureInfo.InvariantCulture;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonSerializerSettings" /> class.
		/// </summary>
		public JsonSerializerSettings()
		{
			this.Converters = new List<JsonConverter>();
		}

		#endregion

		#region Fields/Constants

		internal const bool DefaultCheckAdditionalContent = false;
		internal const ConstructorHandling DefaultConstructorHandling = ConstructorHandling.Default;
		internal const DateFormatHandling DefaultDateFormatHandling = DateFormatHandling.IsoDateFormat;
		internal const string DefaultDateFormatString = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
		internal const DateParseHandling DefaultDateParseHandling = DateParseHandling.DateTime;
		internal const DateTimeZoneHandling DefaultDateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
		internal const DefaultValueHandling DefaultDefaultValueHandling = DefaultValueHandling.Include;
		internal const FloatFormatHandling DefaultFloatFormatHandling = FloatFormatHandling.String;
		internal const FloatParseHandling DefaultFloatParseHandling = FloatParseHandling.Double;
		internal const FormatterAssemblyStyle DefaultFormatterAssemblyStyle = FormatterAssemblyStyle.Simple;
		internal const Formatting DefaultFormatting = Formatting.None;

		internal const MissingMemberHandling DefaultMissingMemberHandling = MissingMemberHandling.Ignore;
		internal const NullValueHandling DefaultNullValueHandling = NullValueHandling.Include;
		internal const ObjectCreationHandling DefaultObjectCreationHandling = ObjectCreationHandling.Auto;
		internal const PreserveReferencesHandling DefaultPreserveReferencesHandling = PreserveReferencesHandling.None;
		internal const ReferenceLoopHandling DefaultReferenceLoopHandling = ReferenceLoopHandling.Error;
		internal const StringEscapeHandling DefaultStringEscapeHandling = StringEscapeHandling.Default;
		internal const FormatterAssemblyStyle DefaultTypeNameAssemblyFormat = FormatterAssemblyStyle.Simple;
		internal const TypeNameHandling DefaultTypeNameHandling = TypeNameHandling.None;
		internal static readonly StreamingContext DefaultContext;

		internal static readonly CultureInfo DefaultCulture;
		internal bool? _checkAdditionalContent;
		internal ConstructorHandling? _constructorHandling;
		internal StreamingContext? _context;
		internal CultureInfo _culture;
		internal DateFormatHandling? _dateFormatHandling;
		internal string _dateFormatString;
		internal bool _dateFormatStringSet;
		internal DateParseHandling? _dateParseHandling;
		internal DateTimeZoneHandling? _dateTimeZoneHandling;
		internal DefaultValueHandling? _defaultValueHandling;
		internal FloatFormatHandling? _floatFormatHandling;
		internal FloatParseHandling? _floatParseHandling;
		internal Formatting? _formatting;
		internal int? _maxDepth;
		internal bool _maxDepthSet;
		internal MissingMemberHandling? _missingMemberHandling;
		internal NullValueHandling? _nullValueHandling;
		internal ObjectCreationHandling? _objectCreationHandling;
		internal PreserveReferencesHandling? _preserveReferencesHandling;
		internal ReferenceLoopHandling? _referenceLoopHandling;
		internal StringEscapeHandling? _stringEscapeHandling;
		internal FormatterAssemblyStyle? _typeNameAssemblyFormat;
		internal TypeNameHandling? _typeNameHandling;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets the <see cref="SerializationBinder" /> used by the serializer when resolving type names.
		/// </summary>
		/// <value> The binder. </value>
		public SerializationBinder Binder
		{
			get;
			set;
		}

		/// <summary>
		/// Gets a value indicating whether there will be a check for additional content after deserializing an object.
		/// </summary>
		/// <value>
		/// <c> true </c> if there will be a check for additional content after deserializing an object; otherwise, <c> false </c>.
		/// </value>
		public bool CheckAdditionalContent
		{
			get
			{
				return this._checkAdditionalContent ?? DefaultCheckAdditionalContent;
			}
			set
			{
				this._checkAdditionalContent = value;
			}
		}

		/// <summary>
		/// Gets or sets how constructors are used during deserialization.
		/// </summary>
		/// <value> The constructor handling. </value>
		public ConstructorHandling ConstructorHandling
		{
			get
			{
				return this._constructorHandling ?? DefaultConstructorHandling;
			}
			set
			{
				this._constructorHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="StreamingContext" /> used by the serializer when invoking serialization callback methods.
		/// </summary>
		/// <value> The context. </value>
		public StreamingContext Context
		{
			get
			{
				return this._context ?? DefaultContext;
			}
			set
			{
				this._context = value;
			}
		}

		/// <summary>
		/// Gets or sets the contract resolver used by the serializer when
		/// serializing .NET objects to JSON and vice versa.
		/// </summary>
		/// <value> The contract resolver. </value>
		public IContractResolver ContractResolver
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a collection <see cref="JsonConverter" /> that will be used during serialization.
		/// </summary>
		/// <value> The converters. </value>
		public IList<JsonConverter> Converters
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the culture used when reading JSON. Defaults to <see cref="CultureInfo.InvariantCulture" />.
		/// </summary>
		public CultureInfo Culture
		{
			get
			{
				return this._culture ?? DefaultCulture;
			}
			set
			{
				this._culture = value;
			}
		}

		/// <summary>
		/// Get or set how dates are written to JSON text.
		/// </summary>
		public DateFormatHandling DateFormatHandling
		{
			get
			{
				return this._dateFormatHandling ?? DefaultDateFormatHandling;
			}
			set
			{
				this._dateFormatHandling = value;
			}
		}

		/// <summary>
		/// Get or set how <see cref="DateTime" /> and <see cref="DateTimeOffset" /> values are formatting when writing JSON text.
		/// </summary>
		public string DateFormatString
		{
			get
			{
				return this._dateFormatString ?? DefaultDateFormatString;
			}
			set
			{
				this._dateFormatString = value;
				this._dateFormatStringSet = true;
			}
		}

		/// <summary>
		/// Get or set how date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z", are parsed when reading JSON.
		/// </summary>
		public DateParseHandling DateParseHandling
		{
			get
			{
				return this._dateParseHandling ?? DefaultDateParseHandling;
			}
			set
			{
				this._dateParseHandling = value;
			}
		}

		/// <summary>
		/// Get or set how <see cref="DateTime" /> time zones are handling during serialization and deserialization.
		/// </summary>
		public DateTimeZoneHandling DateTimeZoneHandling
		{
			get
			{
				return this._dateTimeZoneHandling ?? DefaultDateTimeZoneHandling;
			}
			set
			{
				this._dateTimeZoneHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets how null default are handled during serialization and deserialization.
		/// </summary>
		/// <value> The default value handling. </value>
		public DefaultValueHandling DefaultValueHandling
		{
			get
			{
				return this._defaultValueHandling ?? DefaultDefaultValueHandling;
			}
			set
			{
				this._defaultValueHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets the error handler called during serialization and deserialization.
		/// </summary>
		/// <value> The error handler called during serialization and deserialization. </value>
		public EventHandler<ErrorEventArgs> Error
		{
			get;
			set;
		}

		/// <summary>
		/// Get or set how special floating point numbers, e.g. <see cref="F:System.Double.NaN" />,
		/// <see cref="F:System.Double.PositiveInfinity" /> and <see cref="F:System.Double.NegativeInfinity" />,
		/// are written as JSON.
		/// </summary>
		public FloatFormatHandling FloatFormatHandling
		{
			get
			{
				return this._floatFormatHandling ?? DefaultFloatFormatHandling;
			}
			set
			{
				this._floatFormatHandling = value;
			}
		}

		/// <summary>
		/// Get or set how floating point numbers, e.g. 1.0 and 9.9, are parsed when reading JSON text.
		/// </summary>
		public FloatParseHandling FloatParseHandling
		{
			get
			{
				return this._floatParseHandling ?? DefaultFloatParseHandling;
			}
			set
			{
				this._floatParseHandling = value;
			}
		}

		/// <summary>
		/// Indicates how JSON text output is formatted.
		/// </summary>
		public Formatting Formatting
		{
			get
			{
				return this._formatting ?? DefaultFormatting;
			}
			set
			{
				this._formatting = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum depth allowed when reading JSON. Reading past this depth will throw a <see cref="JsonReaderException" />.
		/// </summary>
		public int? MaxDepth
		{
			get
			{
				return this._maxDepth;
			}
			set
			{
				if (value <= 0)
					throw new ArgumentException("Value must be positive.", "value");

				this._maxDepth = value;
				this._maxDepthSet = true;
			}
		}

		/// <summary>
		/// Gets or sets how missing members (e.g. JSON contains a property that isn't a member on the object) are handled during deserialization.
		/// </summary>
		/// <value> Missing member handling. </value>
		public MissingMemberHandling MissingMemberHandling
		{
			get
			{
				return this._missingMemberHandling ?? DefaultMissingMemberHandling;
			}
			set
			{
				this._missingMemberHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets how null values are handled during serialization and deserialization.
		/// </summary>
		/// <value> Null value handling. </value>
		public NullValueHandling NullValueHandling
		{
			get
			{
				return this._nullValueHandling ?? DefaultNullValueHandling;
			}
			set
			{
				this._nullValueHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets how objects are created during deserialization.
		/// </summary>
		/// <value> The object creation handling. </value>
		public ObjectCreationHandling ObjectCreationHandling
		{
			get
			{
				return this._objectCreationHandling ?? DefaultObjectCreationHandling;
			}
			set
			{
				this._objectCreationHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets how object references are preserved by the serializer.
		/// </summary>
		/// <value> The preserve references handling. </value>
		public PreserveReferencesHandling PreserveReferencesHandling
		{
			get
			{
				return this._preserveReferencesHandling ?? DefaultPreserveReferencesHandling;
			}
			set
			{
				this._preserveReferencesHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets how reference loops (e.g. a class referencing itself) is handled.
		/// </summary>
		/// <value> Reference loop handling. </value>
		public ReferenceLoopHandling ReferenceLoopHandling
		{
			get
			{
				return this._referenceLoopHandling ?? DefaultReferenceLoopHandling;
			}
			set
			{
				this._referenceLoopHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="IReferenceResolver" /> used by the serializer when resolving references.
		/// </summary>
		/// <value> The reference resolver. </value>
		public IReferenceResolver ReferenceResolver
		{
			get;
			set;
		}

		/// <summary>
		/// Get or set how strings are escaped when writing JSON text.
		/// </summary>
		public StringEscapeHandling StringEscapeHandling
		{
			get
			{
				return this._stringEscapeHandling ?? DefaultStringEscapeHandling;
			}
			set
			{
				this._stringEscapeHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="ITraceWriter" /> used by the serializer when writing trace messages.
		/// </summary>
		/// <value> The trace writer. </value>
		public ITraceWriter TraceWriter
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets how a type name assembly is written and resolved by the serializer.
		/// </summary>
		/// <value> The type name assembly format. </value>
		public FormatterAssemblyStyle TypeNameAssemblyFormat
		{
			get
			{
				return this._typeNameAssemblyFormat ?? DefaultFormatterAssemblyStyle;
			}
			set
			{
				this._typeNameAssemblyFormat = value;
			}
		}

		/// <summary>
		/// Gets or sets how type name writing and reading is handled by the serializer.
		/// </summary>
		/// <value> The type name handling. </value>
		public TypeNameHandling TypeNameHandling
		{
			get
			{
				return this._typeNameHandling ?? DefaultTypeNameHandling;
			}
			set
			{
				this._typeNameHandling = value;
			}
		}

		#endregion
	}
}