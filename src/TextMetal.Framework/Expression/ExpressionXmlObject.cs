/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Framework.Core;
using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Expression
{
	public abstract class ExpressionXmlObject : XmlObject, IExpressionXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ExpressionXmlObject class.
		/// </summary>
		protected ExpressionXmlObject()
		{
		}

		#endregion

		#region Methods/Operators

		protected abstract object CoreEvaluateExpression(ITemplatingContext templatingContext);

		public object EvaluateExpression(ITemplatingContext templatingContext)
		{
			return this.CoreEvaluateExpression(templatingContext);
		}

		#endregion
	}
}