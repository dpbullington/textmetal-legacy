/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Reflection;

using TextMetal.Framework.Core;
using TextMetal.Framework.Expression;
using TextMetal.Framework.Source;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;
using TextMetal.Middleware.Solder.Executive;
using TextMetal.Middleware.Solder.Extensions;

namespace TextMetal.Framework.Template
{
	[XmlElementMapping(LocalName = "InvokeSourceStrategy", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Items)]
	public sealed class InvokeSourceStrategyConstruct : TemplateXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the InvokeSourceStrategyConstruct class.
		/// </summary>
		public InvokeSourceStrategyConstruct()
		{
		}

		#endregion

		#region Fields/Constants

		private string assemblyQualifiedTypeName;
		private string sourceFilePath;
		private string args;
		private bool useAlloc;
		private string var;

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

		/// <summary>
		/// NEW dpb 2021-11-25
		/// </summary>
		[XmlAttributeMapping(LocalName = "args", NamespaceUri = "")]
		public string Args
		{
			get
			{
				return this.args;
			}
			set
			{
				this.args = value;
			}
		}
		
		[XmlAttributeMapping(LocalName = "src", NamespaceUri = "")]
		public string SourceFilePath
		{
			get
			{
				return this.sourceFilePath;
			}
			set
			{
				this.sourceFilePath = value;
			}
		}

		[XmlAttributeMapping(LocalName = "alloc", NamespaceUri = "")]
		public bool UseAlloc
		{
			get
			{
				return this.useAlloc;
			}
			set
			{
				this.useAlloc = value;
			}
		}

		[XmlAttributeMapping(LocalName = "var", NamespaceUri = "")]
		public string Var
		{
			get
			{
				return this.var;
			}
			set
			{
				this.var = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreExpandTemplate(ITemplatingContext templatingContext)
		{
			string aqtn;
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;
			ISourceStrategy sourceStrategy;
			Type sourceStrategyType;
			string sourceFilePath, _args, var;
			object source;

			if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));

			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy();

			aqtn = templatingContext.Tokenizer.ExpandTokens(this.AssemblyQualifiedTypeName, dynamicWildcardTokenReplacementStrategy);
			sourceFilePath = templatingContext.Tokenizer.ExpandTokens(this.SourceFilePath, dynamicWildcardTokenReplacementStrategy);
			var = templatingContext.Tokenizer.ExpandTokens(this.Var, dynamicWildcardTokenReplacementStrategy);
			_args = templatingContext.Tokenizer.ExpandTokens(this.Args, dynamicWildcardTokenReplacementStrategy);
			
			sourceStrategyType = Type.GetType(aqtn, false);

			if ((object)sourceStrategyType == null)
				throw new InvalidOperationException(string.Format("Failed to load the source strategy type '{0}' via Type.GetType(..).", aqtn));

			if (!typeof(ISourceStrategy).IsAssignableFrom(sourceStrategyType))
				throw new InvalidOperationException(string.Format("The source strategy type is not assignable to type '{0}'.", typeof(ISourceStrategy).FullName));

			sourceStrategy = (ISourceStrategy)Activator.CreateInstance(sourceStrategyType);
			
			const string CMDLN_TOKEN_PROPERTY = "property";
			IDictionary<string, IList<string>> properties;
			IList<string> propertyValues;
			
			properties = new Dictionary<string, IList<string>>();
			
			var __args = (_args ?? "").Split("|");
			
			var arguments = ExecutableApplicationFascade.ParseCommandLineArguments(__args);
			
			bool hasProperties = arguments.TryGetValue(CMDLN_TOKEN_PROPERTY, out var argumentValues);
			
			if (hasProperties)
			{
				if ((object)argumentValues != null)
				{
					foreach (string argumentValue in argumentValues)
					{
						string key, value;

						if (!ExecutableApplicationFascade.TryParseCommandLineArgumentProperty(argumentValue, out key, out value))
							continue;

						if (!properties.ContainsKey(key))
							properties.Add(key, propertyValues = new List<string>());
						else
							propertyValues = properties[key];

						// duplicate values are ignored
						if (propertyValues.Contains(value))
							continue;

						propertyValues.Add(value);
					}
				}
			}

			source = sourceStrategy.GetSourceObject(/*templatingContext, */sourceFilePath, properties/*templatingContext.Properties*/);

			if (!this.UseAlloc)
			{
				templatingContext.IteratorModels.Push(source);

				if ((object)this.Items != null)
				{
					foreach (ITemplateMechanism templateMechanism in this.Items)
						templateMechanism.ExpandTemplate(templatingContext);
				}

				templatingContext.IteratorModels.Pop();
			}
			else
			{
				if (!SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(var))
				{
					IExpressionContainerConstruct expressionContainerConstruct;
					ValueConstruct valueConstruct;

					new AllocConstruct()
					{
						Token = var
					}.ExpandTemplate(templatingContext);

					expressionContainerConstruct = new ExpressionContainerConstruct();

					valueConstruct = new ValueConstruct()
									{
										__ = source
									};

					((IContentContainerXmlObject<IExpressionXmlObject>)expressionContainerConstruct).Content = valueConstruct;

					new AssignConstruct()
					{
						Token = var,
						Expression = expressionContainerConstruct
					}.ExpandTemplate(templatingContext);
					
					// dpb 2021-11-25
					if ((object)this.Items != null)
					{
						foreach (ITemplateMechanism templateMechanism in this.Items)
							templateMechanism.ExpandTemplate(templatingContext);
					}
				}
			}
		}

		#endregion
	}
}