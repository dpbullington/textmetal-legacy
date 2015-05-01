/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Framework.Core;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;
using TextMetal.Middleware.Common.Fascades.Utilities;

namespace TextMetal.Framework.Expression
{
	[XmlElementMapping(LocalName = "Value", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Sterile)]
	public sealed class ValueConstruct : ExpressionXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ValueConstruct class.
		/// </summary>
		public ValueConstruct()
		{
		}

		#endregion

		#region Fields/Constants

		private object _;
		private string type;

		#endregion

		#region Properties/Indexers/Events

		public object __
		{
			get
			{
				return this._;
			}
			set
			{
				this._ = value;
			}
		}

		[XmlAttributeMapping(LocalName = "data", NamespaceUri = "", Order = 2)]
		public string Data
		{
			get
			{
				return this.__.SafeToString(null, null, true);
			}
			set
			{
				this.SetObjectValue(value);
			}
		}

		[XmlAttributeMapping(LocalName = "type", NamespaceUri = "", Order = 1)]
		public string Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override object CoreEvaluateExpression(ITemplatingContext templatingContext)
		{
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;

			if ((object)templatingContext == null)
				throw new ArgumentNullException("templatingContext");

			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy();

			return this.__;
		}

		private void SetObjectValue(string data)
		{
			object value;
			Type valueType;

			if (DataTypeFascade.Instance.IsNullOrWhiteSpace(this.Type))
				valueType = typeof(string);
			else
				valueType = System.Type.GetType(this.Type, false);

			if ((object)valueType == null)
				throw new InvalidOperationException(string.Format("The value run-time type failed to load for type '{0}'.", this.Type));

			if (!DataTypeFascade.Instance.TryParse(valueType, data, out value))
				throw new InvalidOperationException(string.Format("Could not parse the value '{0}' as type '{1}'.", data, valueType.FullName));

			this._ = value;
		}

		#endregion
	}
}