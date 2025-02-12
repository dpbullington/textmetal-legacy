﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Framework.Core;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Template
{
	[XmlElementMapping(LocalName = "Alias", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Sterile)]
	public sealed class AliasConstruct : TemplateXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the AliasConstruct class.
		/// </summary>
		public AliasConstruct()
		{
		}

		#endregion

		#region Fields/Constants

		private string assemblyQualifiedTypeName;
		private string localName;
		private string namespaceUri;

		#endregion

		#region Properties/Indexers/Events

		[XmlAttributeMapping(LocalName = "aqt-name", NamespaceUri = "")]
		public string AssemblyQualifiedTypeName
		{
			get
			{
				return this.assemblyQualifiedTypeName;
			}
			set
			{
				this.assemblyQualifiedTypeName = value;
			}
		}

		[XmlAttributeMapping(LocalName = "local-name", NamespaceUri = "")]
		public string LocalName
		{
			get
			{
				return this.localName;
			}
			set
			{
				this.localName = value;
			}
		}

		[XmlAttributeMapping(LocalName = "namespace-uri", NamespaceUri = "")]
		public string NamespaceUri
		{
			get
			{
				return this.namespaceUri;
			}
			set
			{
				this.namespaceUri = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreExpandTemplate(ITemplatingContext templatingContext)
		{
			string localName, namespaceUri, aqtn;
			Type aliasedType;
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;

			if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));

			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy();

			localName = templatingContext.Tokenizer.ExpandTokens(this.LocalName, dynamicWildcardTokenReplacementStrategy);
			namespaceUri = templatingContext.Tokenizer.ExpandTokens(this.NamespaceUri, dynamicWildcardTokenReplacementStrategy);
			aqtn = templatingContext.Tokenizer.ExpandTokens(this.AssemblyQualifiedTypeName, dynamicWildcardTokenReplacementStrategy);

			aliasedType = Type.GetType(aqtn, false);

			if ((object)aliasedType == null)
				throw new InvalidOperationException(string.Format("Failed to load the aliased type '{0}' via Type.GetType(..).", aqtn));

			templatingContext.AddReference(new XmlName()
											{
												LocalName = localName,
												NamespaceUri = namespaceUri
											}, aliasedType);
		}

		#endregion
	}
}