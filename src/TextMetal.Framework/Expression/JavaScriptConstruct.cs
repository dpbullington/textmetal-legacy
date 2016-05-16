/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Framework.Core;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Framework.Expression
{
	[XmlElementMapping(LocalName = "JavaScript", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Sterile)]
	public sealed class JavaScriptConstruct : ExpressionXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the JavaScriptConstruct class.
		/// </summary>
		public JavaScriptConstruct()
		{
		}

		#endregion

		#region Fields/Constants

		private string expr;
		private string file;
		private string script;
		private JavaScriptSource src;

		#endregion

		#region Properties/Indexers/Events

		[XmlAttributeMapping(LocalName = "src", NamespaceUri = "")]
		public string _Src
		{
			get
			{
				return this.Src.SafeToString();
			}
			set
			{
				JavaScriptSource src;

				if (!LegacyInstanceAccessor.DataTypeFascadeLegacyInstance.TryParse<JavaScriptSource>(value, out src))
					this.Src = JavaScriptSource.Unknown;
				else
					this.Src = src;
			}
		}

		[XmlAttributeMapping(LocalName = "expr", NamespaceUri = "")]
		public string Expr
		{
			get
			{
				return this.expr;
			}
			set
			{
				this.expr = value;
			}
		}

		[XmlAttributeMapping(LocalName = "file", NamespaceUri = "")]
		public string File
		{
			get
			{
				return this.file;
			}
			set
			{
				this.file = value;
			}
		}

		[XmlChildElementMapping(LocalName = "Script", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementType = ChildElementType.TextValue)]
		public string Script
		{
			get
			{
				return this.script;
			}
			set
			{
				this.script = value;
			}
		}

		public JavaScriptSource Src
		{
			get
			{
				return this.src;
			}
			set
			{
				this.src = value;
			}
		}

		#endregion

		#region Methods/Operators

		public static object JavaScriptExpressionResolver(ITemplatingContext context, string[] parameters)
		{
			const int CNT_P = 1; // expr

			if ((object)context == null)
				throw new ArgumentNullException(nameof(context));

			if ((object)parameters == null)
				throw new ArgumentNullException(nameof(parameters));

			if (parameters.Length != CNT_P)
				throw new InvalidOperationException(string.Format("JavaScriptExpressionResolver expects '{1}' parameter(s) but received '{0}' parameter(s).", parameters.Length, CNT_P));

			return new JavaScriptConstruct()
					{
						Src = JavaScriptSource.Expr,
						Expr = parameters[0]
					}.CoreEvaluateExpression(context);
		}

		protected override object CoreEvaluateExpression(ITemplatingContext templatingContext)
		{
			JavaScriptHost javaScriptHost;
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;
			string scriptContent;
			IDictionary<string, object> scriptVariables;

			object result;
			Func<string, object> func;
			Action action;
			IDictionary<string, object> textMetal;

			if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));

			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy();

			switch (this.Src)
			{
				case JavaScriptSource.Script:
					scriptContent = this.Script;
					break;
				case JavaScriptSource.Expr:
					scriptContent = this.Expr;
					break;
				case JavaScriptSource.File:
					string file;
					file = templatingContext.Tokenizer.ExpandTokens(this.File, dynamicWildcardTokenReplacementStrategy);
					scriptContent = templatingContext.Input.LoadContent(file);
					break;
				default:
					scriptContent = "";
					break;
			}

			javaScriptHost = JavaScriptHost.Instance;

			if (!javaScriptHost.Compile(scriptContent.GetHashCode(), scriptContent))
				new object(); // in cache already

			textMetal = new Dictionary<string, object>();

			func = (token) =>
					{
						object value;
						value = dynamicWildcardTokenReplacementStrategy.Evaluate(token, null);
						templatingContext.Output.LogTextWriter.WriteLine("[{0}]={1}", token, value);
						return value;
					};

			action = () => templatingContext.LaunchDebugger();

			textMetal.Add("EvaluateToken", func);
			textMetal.Add("DebuggerBreakpoint", action);

			scriptVariables = new Dictionary<string, object>();
			scriptVariables.Add("textMetal", textMetal);

			// natives
			scriptVariables.Add("print", new Action<object>(Console.WriteLine));
			scriptVariables.Add("prompt", new Func<object>(Console.ReadLine));

			foreach (KeyValuePair<string, object> variableEntry in templatingContext.CurrentVariableTable)
			{
				if (scriptVariables.ContainsKey(variableEntry.Key))
					throw new InvalidOperationException(string.Format("Cannot set variable '{0}' in JavaScript script scope; the specified variable name already exists.", variableEntry.Key));

				scriptVariables.Add(variableEntry.Key, variableEntry.Value);
			}

			result = javaScriptHost.Execute(scriptContent.GetHashCode(), scriptVariables);
			return result;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		public enum JavaScriptSource
		{
			Unknown = 0,
			Script,
			Expr,
			File
		}

		#endregion
	}
}