/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using CompiledCode = System.String;
using ScriptEngine = Jint.Engine;

namespace TextMetal.Framework.Expression
{
	internal sealed class JavaScriptHost
	{
		#region Constructors/Destructors

		private JavaScriptHost()
		{
		}

		#endregion

		#region Fields/Constants

		private static readonly JavaScriptHost instance = new JavaScriptHost();

		private readonly IDictionary<object, CompiledCode> scriptCompilations = new Dictionary<object, CompiledCode>();
		private readonly ScriptEngine scriptEngine = new ScriptEngine();

		#endregion

		#region Properties/Indexers/Events

		public static JavaScriptHost Instance
		{
			get
			{
				return instance;
			}
		}

		private IDictionary<object, CompiledCode> ScriptCompilations
		{
			get
			{
				return this.scriptCompilations;
			}
		}

		private ScriptEngine ScriptEngine
		{
			get
			{
				return this.scriptEngine;
			}
		}

		#endregion

		#region Methods/Operators

		public bool Compile(object scriptHandle, string scriptContent)
		{
			CompiledCode compiledCode;

			if ((object)scriptHandle == null)
				throw new ArgumentNullException(nameof(scriptHandle));

			if ((object)scriptContent == null)
				throw new ArgumentNullException(nameof(scriptContent));

			compiledCode = scriptContent;

			if (this.ScriptCompilations.ContainsKey(scriptHandle))
				return false;

			this.ScriptCompilations.Add(scriptHandle, compiledCode);

			return true;
		}

		public object Execute(object scriptHandle, IDictionary<string, object> scriptVariables)
		{
			CompiledCode compiledCode;
			object returnValue;

			if ((object)scriptHandle == null)
				throw new ArgumentNullException(nameof(scriptHandle));

			if ((object)scriptVariables == null)
				throw new ArgumentNullException(nameof(scriptVariables));

			if (!this.ScriptCompilations.TryGetValue(scriptHandle, out compiledCode))
				throw new InvalidOperationException(string.Format("'{0}'", scriptHandle));

			foreach (KeyValuePair<string, object> scriptVariable in scriptVariables)
				this.ScriptEngine.SetValue(scriptVariable.Key, scriptVariable.Value);

			returnValue = this.ScriptEngine.Execute(compiledCode).GetCompletionValue().ToObject();

			/*
				BACKLOG(dpbullington@gmail.com / 2015 - 12 - 18):
				Get variables OUT and UP.
			*/

			return returnValue;
		}

		#endregion
	}
}
 