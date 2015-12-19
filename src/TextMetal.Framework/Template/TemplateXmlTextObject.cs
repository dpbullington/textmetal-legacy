/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Framework.Core;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Template
{
	[XmlElementMapping(ChildElementModel = ChildElementModel.Sterile)]
	public sealed class TemplateXmlTextObject : TemplateXmlObject, IXmlTextObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the TemplateXmlTextObject class.
		/// </summary>
		public TemplateXmlTextObject()
		{
		}

		#endregion

		#region Fields/Constants

		private XmlName name;
		private string text;

		#endregion

		#region Properties/Indexers/Events

		public XmlName Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string Text
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
			string text;
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;

			if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));

			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy();

			text = templatingContext.Tokenizer.ExpandTokens(this.Text, dynamicWildcardTokenReplacementStrategy);

			templatingContext.Output.CurrentTextWriter.Write(text);
		}

		#endregion
	}
}