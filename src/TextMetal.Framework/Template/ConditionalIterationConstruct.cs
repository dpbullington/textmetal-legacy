/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
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
	public abstract class ConditionalIterationConstruct : TemplateXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ConditionalIterationConstruct class.
		/// </summary>
		protected ConditionalIterationConstruct(bool precondition, bool affirmative)
		{
			this.precondition = precondition;
			this.affirmative = affirmative;
		}

		#endregion

		#region Fields/Constants

		private readonly bool affirmative;
		private readonly bool precondition;
		private ITemplateContainerConstruct body;
		private IExpressionContainerConstruct condition;
		private string varIx;

		#endregion

		#region Properties/Indexers/Events

		private bool Affirmative
		{
			get
			{
				return this.affirmative;
			}
		}

		protected override bool IsScopeBlock
		{
			get
			{
				return true;
			}
		}

		private bool Precondition
		{
			get
			{
				return this.precondition;
			}
		}

		[XmlChildElementMapping(ChildElementType = ChildElementType.ParentQualified, LocalName = "Body", NamespaceUri = "http://www.textmetal.com/api/v6.0.0")]
		public ITemplateContainerConstruct Body
		{
			get
			{
				return this.body;
			}
			set
			{
				this.body = value;
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

		[XmlAttributeMapping(LocalName = "var-ix", NamespaceUri = "")]
		public string VarIx
		{
			get
			{
				return this.varIx;
			}
			set
			{
				this.varIx = value;
			}
		}

		#endregion

		#region Methods/Operators

		private bool CheckCondition(ITemplatingContext templatingContext)
		{
			object value;
			bool conditional;

			if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));

			if ((object)this.Condition != null)
				value = this.Condition.EvaluateExpression(templatingContext);
			else
				value = false;

			if ((object)value != null && !(value is bool) && !(value is bool?))
				throw new InvalidOperationException(string.Format("The conditional iteration construct condition expression has evaluated to a non-null value with an unsupported type '{0}'; only '{1}' and '{2}' types are supported.", value.GetType().FullName, typeof(bool).FullName, typeof(bool?).FullName));

			conditional = ((bool)(value ?? false));

			return (conditional || !this.Affirmative);
		}

		protected abstract void CoreConditionalIterationInitialize(ITemplatingContext templatingContext);

		protected abstract void CoreConditionalIterationStep(ITemplatingContext templatingContext, uint indexOneBase);

		protected abstract void CoreConditionalIterationTerminate(ITemplatingContext templatingContext);

		protected override void CoreExpandTemplate(ITemplatingContext templatingContext)
		{
			const uint MAX_ITERATIONS_INFINITE_LOOP_CHECK = 999999;
			string varIx;
			uint index = 1; // one-based
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;

			if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));

			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy();

			varIx = templatingContext.Tokenizer.ExpandTokens(this.VarIx, dynamicWildcardTokenReplacementStrategy);

			if (!LegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(varIx))
			{
				new AllocConstruct()
				{
					Token = varIx
				}.ExpandTemplate(templatingContext);
			}

			this.CoreConditionalIterationInitialize(templatingContext);

			// DO NOT USE REAL FOR LOOP HERE
			while (true)
			{
				if (!LegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(varIx))
				{
					IExpressionContainerConstruct expressionContainerConstruct;
					ValueConstruct valueConstruct;

					expressionContainerConstruct = new ExpressionContainerConstruct();

					valueConstruct = new ValueConstruct()
									{
										Type = typeof(int).FullName,
										__ = index
									};

					((IContentContainerXmlObject<IExpressionXmlObject>)expressionContainerConstruct).Content = valueConstruct;

					new AssignConstruct()
					{
						Token = varIx,
						Expression = expressionContainerConstruct
					}.ExpandTemplate(templatingContext);
				}

				if (index > MAX_ITERATIONS_INFINITE_LOOP_CHECK)
					throw new InvalidOperationException(string.Format("The conditional iteration construct has exceeded the maximun number of iterations '{0}'; this is an infinite loop prevention mechansim.", MAX_ITERATIONS_INFINITE_LOOP_CHECK));

				if (this.Precondition)
				{
					if (!this.CheckCondition(templatingContext))
						break;
				}

				if ((object)this.Body != null)
					this.Body.ExpandTemplate(templatingContext);

				this.CoreConditionalIterationStep(templatingContext, index);

				if (!this.Precondition)
				{
					if (!this.CheckCondition(templatingContext))
						break;
				}

				index++;
			}

			this.CoreConditionalIterationTerminate(templatingContext);

			if (!LegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(varIx))
			{
				new FreeConstruct()
				{
					Token = varIx
				}.ExpandTemplate(templatingContext);
			}
		}

		#endregion
	}
}