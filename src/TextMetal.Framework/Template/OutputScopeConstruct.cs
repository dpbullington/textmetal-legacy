/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Text;

using TextMetal.Framework.Core;
using TextMetal.Framework.Expression;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;
using TextMetal.Middleware.Solder.Extensions;

namespace TextMetal.Framework.Template
{
	[XmlElementMapping(LocalName = "OutputScope", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Items)]
	public sealed class OutputScopeConstruct : TemplateXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the OutputScopeConstruct class.
		/// </summary>
		public OutputScopeConstruct()
		{
		}

		#endregion

		#region Fields/Constants

		private bool append;
		private string encodingName;
		private string scopeName;

		#endregion

		#region Properties/Indexers/Events

		protected override bool IsScopeBlock
		{
			get
			{
				return true;
			}
		}

		[XmlAttributeMapping(LocalName = "append", NamespaceUri = "")]
		public bool Append
		{
			get
			{
				return this.append;
			}
			set
			{
				this.append = value;
			}
		}

		[XmlAttributeMapping(LocalName = "encoding", NamespaceUri = "")]
		public string EncodingName
		{
			get
			{
				return this.encodingName;
			}
			set
			{
				this.encodingName = value;
			}
		}

		[XmlAttributeMapping(LocalName = "name", NamespaceUri = "")]
		public string ScopeName
		{
			get
			{
				return this.scopeName;
			}
			set
			{
				this.scopeName = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreExpandTemplate(ITemplatingContext templatingContext)
		{
			string scopeName;
			bool appendMode;
			Encoding encoding;
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;

			if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));

			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy();

			scopeName = templatingContext.Tokenizer.ExpandTokens(this.ScopeName, dynamicWildcardTokenReplacementStrategy);
			appendMode = this.Append;

			if (!SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(this.EncodingName))
				encoding = Encoding.GetEncoding(this.EncodingName);
			else
				encoding = Encoding.UTF8;

			if (!SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(scopeName))
			{
				new AllocConstruct()
				{
					Token = "#OutputScopeName"
				}.ExpandTemplate(templatingContext);
			}

			if (!SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(scopeName))
			{
				IExpressionContainerConstruct expressionContainerConstruct;
				ValueConstruct valueConstruct;

				expressionContainerConstruct = new ExpressionContainerConstruct();

				valueConstruct = new ValueConstruct()
								{
									Type = typeof(string).FullName,
									__ = scopeName
								};

				((IContentContainerXmlObject<IExpressionXmlObject>)expressionContainerConstruct).Content = valueConstruct;

				new AssignConstruct()
				{
					Token = "#OutputScopeName",
					Expression = expressionContainerConstruct
				}.ExpandTemplate(templatingContext);
			}

			templatingContext.Output.LogTextWriter.WriteLine("['{0:O}' (UTC)]\tEntering output scope '{1}'.", DateTime.UtcNow, scopeName);
			templatingContext.Output.EnterScope(scopeName, appendMode, encoding);

			if ((object)this.Items != null)
			{
				foreach (ITemplateMechanism templateMechanism in this.Items)
					templateMechanism.ExpandTemplate(templatingContext);
			}

			templatingContext.Output.LeaveScope(scopeName);
			templatingContext.Output.LogTextWriter.WriteLine("['{0:O}' (UTC)]\tLeaving output scope '{1}'.", DateTime.UtcNow, scopeName);

			if (!SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(scopeName))
			{
				new FreeConstruct()
				{
					Token = "#OutputScopeName"
				}.ExpandTemplate(templatingContext);
			}
		}

		#endregion
	}
}