/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Scripting.Hosting;

namespace TextMetal.Framework.ExpressionModel
{
	public class RubyHost
	{
		#region Constructors/Destructors

		public RubyHost()
		{
			List<string> paths;

			//this.remoteAppDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString("N"));
			this.scriptRuntimeSetup = new ScriptRuntimeSetup();
			this.ScriptRuntimeSetup.LanguageSetups.Add(
				new LanguageSetup(
					"IronRuby.Runtime.RubyContext, IronRuby",
					"IronRuby",
					new[] { "IronRuby", "Ruby", "rb" },
					new[] { ".rb" }));

			this.scriptRuntime = new ScriptRuntime(this.scriptRuntimeSetup);
			//this.scriptRuntime = ScriptRuntime.CreateRemote(this.RemoteAppDomain, this.ScriptRuntimeSetup);
			this.scriptEngine = this.ScriptRuntime.GetEngine("Ruby");

			paths = this.ScriptEngine.GetSearchPaths().ToList();
			paths.Clear();
			//paths.Add(System.IO.Directory.GetCurrentDirectory());
			this.ScriptEngine.SetSearchPaths(paths);
		}

		#endregion

		#region Fields/Constants

		private readonly IDictionary<object, CompiledCode> scriptCompilations = new Dictionary<object, CompiledCode>();
		private readonly ScriptEngine scriptEngine;
		private readonly ScriptRuntime scriptRuntime;
		private readonly ScriptRuntimeSetup scriptRuntimeSetup;

		#endregion

		#region Properties/Indexers/Events

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

		private ScriptRuntime ScriptRuntime
		{
			get
			{
				return this.scriptRuntime;
			}
		}

		private ScriptRuntimeSetup ScriptRuntimeSetup
		{
			get
			{
				return this.scriptRuntimeSetup;
			}
		}

		#endregion

		#region Methods/Operators

		public bool Compile(object scriptHandle, string scriptContent)
		{
			CompiledCode compiledCode;
			ScriptSource scriptSource;

			if ((object)scriptHandle == null)
				throw new ArgumentNullException("scriptHandle");

			if ((object)scriptContent == null)
				throw new ArgumentNullException("scriptContent");

			scriptSource = this.ScriptEngine.CreateScriptSourceFromString(scriptContent);
			compiledCode = scriptSource.Compile();

			if (this.ScriptCompilations.ContainsKey(scriptHandle))
				return false;

			this.ScriptCompilations.Add(scriptHandle, compiledCode);

			return true;
		}

		public object Execute(object scriptHandle, IDictionary<string, object> scriptVariables)
		{
			CompiledCode compiledCode;
			ScriptScope scriptScope;
			object returnValue;

			if ((object)scriptHandle == null)
				throw new ArgumentNullException("scriptHandle");

			if ((object)scriptVariables == null)
				throw new ArgumentNullException("scriptVariables");

			//scriptScope = this.ScriptEngine.CreateScope();

			if (!this.ScriptCompilations.TryGetValue(scriptHandle, out compiledCode))
				throw new InvalidOperationException(string.Format("'{0}'", scriptHandle));

			scriptScope = compiledCode.Engine.CreateScope();

			foreach (KeyValuePair<string, object> scriptVariable in scriptVariables)
			{
				if (scriptScope.ContainsVariable(scriptVariable.Key))
					throw new InvalidOperationException(string.Format("Cannot set variable '{0}' in Ruby script scope; the specified variable name already exists.", scriptVariable.Key));

				scriptScope.SetVariable(scriptVariable.Key, scriptVariable.Value);
			}

			returnValue = compiledCode.Execute(scriptScope);

			// TODO get variables OUT and UP

			return returnValue;
		}

		#endregion
	}
}