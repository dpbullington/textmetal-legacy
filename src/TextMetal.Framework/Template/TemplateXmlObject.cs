﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Framework.Core;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Template
{
	public abstract class TemplateXmlObject : XmlObject, ITemplateXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the TemplateXmlObject class.
		/// </summary>
		protected TemplateXmlObject()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		protected virtual bool IsScopeBlock
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region Methods/Operators

		protected abstract void CoreExpandTemplate(ITemplatingContext templatingContext);

		public void ExpandTemplate(ITemplatingContext templatingContext)
		{
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;

			if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));

			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy();

			if (this.IsScopeBlock)
				templatingContext.VariableTables.Push(new Dictionary<string, object>());

			this.CoreExpandTemplate(templatingContext);

			if (this.IsScopeBlock)
				templatingContext.VariableTables.Pop();
		}

		#endregion
	}
}