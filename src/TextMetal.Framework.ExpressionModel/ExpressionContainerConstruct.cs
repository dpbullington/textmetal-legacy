/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Core.Tokenization;
using TextMetal.Common.Syntax.Expressions;
using TextMetal.Common.Xml;
using TextMetal.Framework.Core;

namespace TextMetal.Framework.ExpressionModel
{
	[XmlElementMapping(LocalName = "ExpressionContainer", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Content)]
	public sealed class ExpressionContainerConstruct : ExpressionXmlObject, IContainer, IExpressionContainerConstruct
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ExpressionContainerConstruct class.
		/// </summary>
		public ExpressionContainerConstruct()
		{
		}

		#endregion

		#region Fields/Constants

		private string id;

		#endregion

		#region Properties/Indexers/Events

		public new IExpressionXmlObject Content
		{
			get
			{
				return (IExpressionXmlObject)base.Content;
			}
			set
			{
				base.Content = value;
			}
		}

		[XmlAttributeMapping(LocalName = "id", NamespaceUri = "")]
		public string Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		IExpression IContainer.Content
		{
			get
			{
				return this.Content;
			}
		}

		#endregion

		#region Methods/Operators

		protected override object CoreEvaluateExpression(ITemplatingContext templatingContext)
		{
			object ovalue;
			string svalue;
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;

			if ((object)templatingContext == null)
				throw new ArgumentNullException("templatingContext");

			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy();

			if ((object)this.Content != null)
				ovalue = ((IExpressionXmlObject)this.Content).EvaluateExpression(templatingContext);
			else
				ovalue = null;

			svalue = ovalue as string;

			if ((object)svalue != null)
				ovalue = templatingContext.Tokenizer.ExpandTokens(svalue, dynamicWildcardTokenReplacementStrategy);

			return ovalue;
		}

		#endregion
	}
}