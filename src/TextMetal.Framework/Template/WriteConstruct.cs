﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Framework.Core;
using TextMetal.Framework.Expression;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;
using TextMetal.Middleware.Solder.Extensions;

namespace TextMetal.Framework.Template
{
	[XmlElementMapping(LocalName = "Write", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Sterile)]
	public sealed class WriteConstruct : TemplateXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the WriteConstruct class.
		/// </summary>
		public WriteConstruct()
		{
		}

		#endregion

		#region Fields/Constants

		private IExpressionContainerConstruct @default;
		private bool dofvisnow;
		private string format;
		private bool newline;
		private IExpressionContainerConstruct text;

		#endregion

		#region Properties/Indexers/Events

		[XmlChildElementMapping(ChildElementType = ChildElementType.ParentQualified, LocalName = "default", NamespaceUri = "http://www.textmetal.com/api/v6.0.0")]
		public IExpressionContainerConstruct Default
		{
			get
			{
				return this.@default;
			}
			set
			{
				this.@default = value;
			}
		}

		[XmlAttributeMapping(LocalName = "dofvisnow", NamespaceUri = "")]
		public bool DoFvIsNoW
		{
			get
			{
				return this.dofvisnow;
			}
			set
			{
				this.dofvisnow = value;
			}
		}

		[XmlAttributeMapping(LocalName = "format", NamespaceUri = "")]
		public string Format
		{
			get
			{
				return this.format;
			}
			set
			{
				this.format = value;
			}
		}

		[XmlAttributeMapping(LocalName = "newline", NamespaceUri = "")]
		public bool Newline
		{
			get
			{
				return this.newline;
			}
			set
			{
				this.newline = value;
			}
		}

		[XmlChildElementMapping(ChildElementType = ChildElementType.ParentQualified, LocalName = "Text", NamespaceUri = "http://www.textmetal.com/api/v6.0.0")]
		public IExpressionContainerConstruct Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreExpandTemplate(ITemplatingContext templatingContext)
		{
			object valueObj = null, defaultObj = null;
			string output;
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;

			if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));

			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy();

			if ((object)this.Text != null)
				valueObj = this.Text.EvaluateExpression(templatingContext);

			if ((object)this.Default != null)
				defaultObj = this.Default.EvaluateExpression(templatingContext);

			output = valueObj.SafeToString(this.Format, defaultObj.SafeToString(string.Empty, null, true), this.DoFvIsNoW);

			output = templatingContext.Tokenizer.ExpandTokens(output, dynamicWildcardTokenReplacementStrategy);

			if (this.Newline)
				templatingContext.Output.CurrentTextWriter.WriteLine(output);
			else
				templatingContext.Output.CurrentTextWriter.Write(output);
		}

		#endregion
	}
}