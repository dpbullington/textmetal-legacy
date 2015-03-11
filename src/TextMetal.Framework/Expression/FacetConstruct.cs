/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Framework.Core;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Expression
{
	[XmlElementMapping(LocalName = "Facet", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Sterile)]
	public sealed class FacetConstruct : SurfaceConstruct
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the FacetConstruct class.
		/// </summary>
		public FacetConstruct()
		{
		}

		#endregion

		#region Methods/Operators

		protected override object CoreEvaluateExpression(ITemplatingContext templatingContext)
		{
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;
			object obj;

			if ((object)templatingContext == null)
				throw new ArgumentNullException("templatingContext");

			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy();

			if (!dynamicWildcardTokenReplacementStrategy.GetByToken(this.Name, out obj))
				throw new InvalidOperationException(string.Format("The facet name '{0}' was not found on the target model.", this.Name));

			return obj;
		}

		#endregion
	}
}