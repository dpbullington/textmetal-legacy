/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Core.StringTokens;
using TextMetal.Common.Xml;
using TextMetal.Framework.Core;
using TextMetal.Framework.ExpressionModel;

namespace TextMetal.Framework.TemplateModel
{
	public abstract class ConditionalBranchConstruct : TemplateXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ConditionalBranchConstruct class.
		/// </summary>
		protected ConditionalBranchConstruct(bool affirmative)
		{
			this.affirmative = affirmative;
		}

		#endregion

		#region Fields/Constants

		private readonly bool affirmative;
		private IExpressionContainerConstruct condition;
		private ITemplateContainerConstruct @else;
		private ITemplateContainerConstruct @then;

		#endregion

		//private ConditionalBranchConstruct vector;

		#region Properties/Indexers/Events

		private bool Affirmative
		{
			get
			{
				return this.affirmative;
			}
		}

		[XmlChildElementMapping(ChildElementType = ChildElementType.ParentQualified, LocalName = "Condition", NamespaceUri = "http://www.textmetal.com/api/v6.0.0")]
		public IExpressionContainerConstruct Condition
		{
			get
			{
				return this.condition;
			}
			set
			{
				this.condition = value;
			}
		}

		[XmlChildElementMapping(ChildElementType = ChildElementType.ParentQualified, LocalName = "Else", NamespaceUri = "http://www.textmetal.com/api/v6.0.0")]
		public ITemplateContainerConstruct Else
		{
			get
			{
				return this.@else;
			}
			set
			{
				this.@else = value;
			}
		}

		protected override bool IsScopeBlock
		{
			get
			{
				return true;
			}
		}

		[XmlChildElementMapping(ChildElementType = ChildElementType.ParentQualified, LocalName = "Then", NamespaceUri = "http://www.textmetal.com/api/v6.0.0")]
		public ITemplateContainerConstruct Then
		{
			get
			{
				return this.@then;
			}
			set
			{
				this.@then = value;
			}
		}

		[XmlChildElementMapping(ChildElementType = ChildElementType.ParentQualified, LocalName = "False", NamespaceUri = "http://www.textmetal.com/api/v6.0.0")]
		public ITemplateContainerConstruct _False
		{
			get
			{
				return this.Else;
			}
			set
			{
				this.Else = value;
			}
		}

		[XmlChildElementMapping(ChildElementType = ChildElementType.ParentQualified, LocalName = "True", NamespaceUri = "http://www.textmetal.com/api/v6.0.0")]
		public ITemplateContainerConstruct _True
		{
			get
			{
				return this.Then;
			}
			set
			{
				this.Then = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreExpandTemplate(ITemplatingContext templatingContext)
		{
			object obj;
			bool conditional;
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;

			if ((object)templatingContext == null)
				throw new ArgumentNullException("templatingContext");

			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy();

			if ((object)this.Condition == null)
				return;

			obj = this.Condition.EvaluateExpression(templatingContext);

			if ((object)obj != null && !(obj is bool) && !(obj is bool?))
				throw new InvalidOperationException(string.Format("The conditional branch expression has evaluated to a non-null value with an unsupported type '{0}'; only '{1}' and '{2}' types are supported.", obj.GetType().FullName, typeof(bool).FullName, typeof(bool?).FullName));

			conditional = ((bool)(obj ?? false));

			if (conditional || !this.Affirmative)
			{
				if ((object)this.Then != null)
					this.Then.ExpandTemplate(templatingContext);
			}
			else
			{
				if ((object)this.Else != null)
					this.Else.ExpandTemplate(templatingContext);
			}
		}

		#endregion
	}
}