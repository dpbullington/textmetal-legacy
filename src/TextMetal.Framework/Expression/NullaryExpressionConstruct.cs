/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Framework.Core;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Expression
{
	[XmlElementMapping(LocalName = "NullaryExpression", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Sterile)]
	public sealed class NullaryExpressionConstruct : ExpressionXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the NullaryExpressionConstruct class.
		/// </summary>
		public NullaryExpressionConstruct()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly NullaryOperator nullaryOperator = NullaryOperator.Nop;

		#endregion

		#region Properties/Indexers/Events

		public NullaryOperator NullaryOperator
		{
			get
			{
				return this.nullaryOperator;
			}
		}

		#endregion

		#region Methods/Operators

		protected override object CoreEvaluateExpression(ITemplatingContext templatingContext)
		{
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;

			if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));

			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy();

			return null;
		}

		#endregion
	}
}