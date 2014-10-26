/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Dynamic;

using TextMetal.Common.Core;
using TextMetal.Common.Core.Tokenization;
using TextMetal.Common.Xml;
using TextMetal.Framework.Core;

namespace TextMetal.Framework.ExpressionModel
{
	[XmlElementMapping(LocalName = "Ruby", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Sterile)]
	public sealed class RubyConstruct : ExpressionXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the RubyConstruct class.
		/// </summary>
		public RubyConstruct()
		{
		}

		#endregion

		#region Fields/Constants

		private static readonly RubyHost instance = new RubyHost();
		private string expr;
		private string file;
		private string script;
		private RubySource src;

		#endregion

		#region Properties/Indexers/Events

		private static RubyHost SingletonRubyHost
		{
			get
			{
				return instance;
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

		public RubySource Src
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

		[XmlAttributeMapping(LocalName = "src", NamespaceUri = "")]
		public string _Src
		{
			get
			{
				return this.Src.SafeToString();
			}
			set
			{
				RubySource src;

				if (!DataType.Instance.TryParse<RubySource>(value, out src))
					this.Src = RubySource.Unknown;
				else
					this.Src = src;
			}
		}

		#endregion

		#region Methods/Operators

		public static object RubyExpressionResolver(object[] context, string[] parameters)
		{
			ITemplatingContext templatingContext;

			if ((object)context == null)
				throw new ArgumentNullException("context");

			if ((object)parameters == null)
				throw new ArgumentNullException("parameters");

			if (context.Length != 1)
				throw new InvalidOperationException(string.Format("RubyExpressionResolver requires a contextual TemplatingContext."));

			if (parameters.Length > 1)
				throw new InvalidOperationException(string.Format("RubyExpressionResolver parameter count '{0}' exceeds limit of '{1}'.", parameters.Length, 1));

			templatingContext = (ITemplatingContext)context[0];

			return new RubyConstruct()
					{
						Src = RubySource.Expr,
						Expr = parameters[0]
					}.CoreEvaluateExpression(templatingContext);
		}

		protected override object CoreEvaluateExpression(ITemplatingContext templatingContext)
		{
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;
			string scriptContent;
			IDictionary<string, object> scriptVariables;

			dynamic result;
			dynamic textMetal;
			Func<string, object> func;
			Action action;
			IDictionary<string, object> scriptFoo;

			if ((object)templatingContext == null)
				throw new ArgumentNullException("templatingContext");

			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy();

			switch (this.Src)
			{
				case RubySource.Script:
					scriptContent = this.Script;
					break;
				case RubySource.Expr:
					scriptContent = this.Expr;
					break;
				case RubySource.File:
					string file;
					file = templatingContext.Tokenizer.ExpandTokens(this.File, dynamicWildcardTokenReplacementStrategy);
					scriptContent = templatingContext.Input.LoadContent(file);
					break;
				default:
					scriptContent = "nil";
					break;
			}

			if (!SingletonRubyHost.Compile(scriptContent.GetHashCode(), scriptContent))
				new object(); // in cache already

			scriptFoo = new Dictionary<string, object>();

			func = (token) =>
					{
						object value;
						value = dynamicWildcardTokenReplacementStrategy.Evaluate(token, null);
						//Console.WriteLine("[{0}]={1}", token, value);
						return value;
					};

			action = () => templatingContext.LaunchDebugger();

			scriptFoo.Add("EvaluateToken", func);
			scriptFoo.Add("DebuggerBreakpoint", action);

			scriptFoo.Add("__EvaluateToken", func);
			scriptFoo.Add("__DebuggerBreakpoint", action);

			textMetal = new DynamicDictionary(scriptFoo);

			scriptVariables = new Dictionary<string, object>();
			scriptVariables.Add("textMetal", textMetal);

			foreach (KeyValuePair<string, object> variableEntry in templatingContext.CurrentVariableTable)
			{
				if (scriptVariables.ContainsKey(variableEntry.Key))
					throw new InvalidOperationException(string.Format("Cannot set variable '{0}' in Ruby script scope; the specified variable name already exists.", variableEntry.Key));

				scriptVariables.Add(variableEntry.Key, variableEntry.Value);
			}

			result = SingletonRubyHost.Execute(scriptContent.GetHashCode(), scriptVariables);
			return result;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		[Serializable]
		public class DynamicDictionary : DynamicObject
		{
			#region Constructors/Destructors

			public DynamicDictionary(IDictionary<string, object> dictionary)
			{
				this.dictionary = dictionary;
			}

			#endregion

			#region Fields/Constants

			private readonly IDictionary<string, object> dictionary;

			#endregion

			#region Properties/Indexers/Events

			private IDictionary<string, object> Dictionary
			{
				get
				{
					return this.dictionary;
				}
			}

			#endregion

			#region Methods/Operators

			public override IEnumerable<string> GetDynamicMemberNames()
			{
				foreach (string key in this.Dictionary.Keys)
					yield return key;
			}

			public override bool TryGetMember(GetMemberBinder binder, out object result)
			{
				if (this.Dictionary.TryGetValue(binder.Name, out result))
					return true;

				return base.TryGetMember(binder, out result);
			}

			public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
			{
				object value;
				Delegate method;

				// LEGACY COMPATABILITY HACK
				if (!binder.Name.SafeToString().Trim().ToLower().StartsWith("__"))
					return base.TryInvokeMember(binder, args, out result);

				if (!this.Dictionary.TryGetValue(binder.Name, out value))
					return base.TryInvokeMember(binder, args, out result);

				method = value as Delegate;

				if ((object)method == null)
					return base.TryInvokeMember(binder, args, out result);

				result = method.DynamicInvoke(args);
				return true;
			}

			public override bool TrySetMember(SetMemberBinder binder, object value)
			{
				if (this.Dictionary.ContainsKey(binder.Name))
					this.Dictionary.Remove(binder.Name);

				if ((object)value != null)
					this.Dictionary.Add(binder.Name, value);

				return true;
			}

			#endregion
		}

		public enum RubySource
		{
			Unknown = 0,
			Script,
			Expr,
			File
		}

		#endregion
	}
}