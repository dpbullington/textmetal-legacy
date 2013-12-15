// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Xml;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
#if !SL3
	using System.Xml.XPath;
	using System.Xml.Xsl;

#endif

#if !SL3
	public class XmlContextBase : XsltContext, IXmlNamespaceSource
#else
	public class XmlContextBase : XmlNamespaceManager, IXmlNamespaceSource
#endif
	{
		private readonly XmlContextBase parent;
		private Dictionary<string, string> rootNamespaces;
		private bool hasNamespaces;
#if !SL3
		private XPathContext xPathContext;
		private Dictionary<XmlName, IXsltContextVariable> variables;
		private Dictionary<XmlName, IXsltContextFunction> functions;
#endif

		public XmlContextBase()
			: base(new NameTable())
		{
			this.AddNamespace(Xsd.Namespace);
			this.AddNamespace(Xsi.Namespace);
			this.AddNamespace(Wsdl.Namespace);
			this.AddNamespace(XRef.Namespace);
		}

		protected XmlContextBase(XmlContextBase parent)
			: base(GetNameTable(parent))
		{
			this.parent = parent;
		}

#if !SL3
		private static NameTable GetNameTable(XmlContextBase parent)
		{
			return parent.NameTable as NameTable ?? new NameTable();
		}
#else
		private static XmlNameTable GetNameTable(XmlContext parent)
		{
			return parent.NameTable;
		}   
#endif

		public void AddNamespace(XmlNamespaceAttribute attribute)
		{
			var prefix = attribute.Prefix;
			var uri = attribute.NamespaceUri;

			if (string.IsNullOrEmpty(uri))
				throw Error.InvalidNamespaceUri();

			if (attribute.Default)
				this.AddNamespace(string.Empty, uri);

			if (string.IsNullOrEmpty(prefix))
				return;

			this.AddNamespace(prefix, uri);

			if (attribute.Root)
				this.EnsureRootNamespaces().Add(prefix, uri);
		}

		public override void AddNamespace(string prefix, string uri)
		{
			base.AddNamespace(prefix, uri);
			this.hasNamespaces = true;
		}

		private Dictionary<string, string> EnsureRootNamespaces()
		{
			return this.rootNamespaces ??
					(
						this.rootNamespaces = this.parent != null
							? new Dictionary<string, string>(this.parent.EnsureRootNamespaces())
							: new Dictionary<string, string>()
						);
		}

		public override string LookupNamespace(string prefix)
		{
			return this.hasNamespaces
				? base.LookupNamespace(prefix)
				: this.parent.LookupNamespace(prefix);
		}

		public override string LookupPrefix(string uri)
		{
			return this.hasNamespaces
				? base.LookupPrefix(uri)
				: this.parent.LookupPrefix(uri);
		}

		public string GetElementPrefix(IXmlNode node, string namespaceUri)
		{
			string prefix;
			if (namespaceUri == node.LookupNamespaceUri(string.Empty))
				return string.Empty;
			if (TryGetDefinedPrefix(node, namespaceUri, out prefix))
				return prefix;
			if (!this.TryGetPreferredPrefix(node, namespaceUri, out prefix))
				return string.Empty;
			if (!this.ShouldDefineOnRoot(prefix, namespaceUri))
				return string.Empty;

			node.DefineNamespace(prefix, namespaceUri, true);
			return prefix;
		}

		public string GetAttributePrefix(IXmlNode node, string namespaceUri)
		{
			string prefix;
			if (string.IsNullOrEmpty(namespaceUri)) // was: namespaceUri == node.Name.NamespaceUri
				return string.Empty;
			if (TryGetDefinedPrefix(node, namespaceUri, out prefix))
				return prefix;
			if (!this.TryGetPreferredPrefix(node, namespaceUri, out prefix))
				prefix = GeneratePrefix(node);

			var root = this.ShouldDefineOnRoot(prefix, namespaceUri);
			node.DefineNamespace(prefix, namespaceUri, root);
			return prefix;
		}

		private static bool TryGetDefinedPrefix(IXmlNode node, string namespaceUri, out string prefix)
		{
			var definedPrefix = node.LookupPrefix(namespaceUri);
			return string.IsNullOrEmpty(definedPrefix)
				? Try.Failure(out prefix)
				: Try.Success(out prefix, definedPrefix);
		}

		private bool TryGetPreferredPrefix(IXmlNode node, string namespaceUri, out string prefix)
		{
			prefix = this.LookupPrefix(namespaceUri);
			if (string.IsNullOrEmpty(prefix))
				return Try.Failure(out prefix); // No preferred prefix

			namespaceUri = node.LookupNamespaceUri(prefix);
			return string.IsNullOrEmpty(namespaceUri)
				? true // Can use preferred prefix
				: Try.Failure(out prefix); // Preferred prefix already in use
		}

		private static string GeneratePrefix(IXmlNode node)
		{
			for (var i = 0;; i++)
			{
				var prefix = "p" + i;
				var namespaceUri = node.LookupNamespaceUri(prefix);
				if (string.IsNullOrEmpty(namespaceUri))
					return prefix;
			}
		}

		private bool ShouldDefineOnRoot(string prefix, string uri)
		{
			return this.rootNamespaces != null
				? this.ShouldDefineOnRootCore(prefix, uri)
				: this.parent.ShouldDefineOnRoot(prefix, uri);
		}

		private bool ShouldDefineOnRootCore(string prefix, string uri)
		{
			string candidate;
			return this.rootNamespaces.TryGetValue(prefix, out candidate)
					&& candidate == uri;
		}

#if !SL3
		private XPathContext XPathContext
		{
			get
			{
				return this.xPathContext ?? (this.xPathContext = new XPathContext(this));
			}
		}

		public override bool Whitespace
		{
			get
			{
				return true;
			}
		}

		public override bool PreserveWhitespace(XPathNavigator node)
		{
			return true;
		}

		public override int CompareDocument(string baseUriA, string baseUriB)
		{
			return StringComparer.Ordinal.Compare(baseUriA, baseUriB);
		}

		public void AddVariable(string prefix, string name, IXsltContextVariable variable)
		{
			var key = new XmlName(name, prefix ?? string.Empty);
			this.AddVariable(key, variable);
		}

		public void AddFunction(string prefix, string name, IXsltContextFunction function)
		{
			var key = new XmlName(name, prefix ?? string.Empty);
			this.AddFunction(key, function);
		}

		public void AddVariable(XPathVariableAttribute attribute)
		{
			this.AddVariable(attribute.Name, attribute);
		}

		public void AddFunction(XPathFunctionAttribute attribute)
		{
			this.AddFunction(attribute.Name, attribute);
		}

		public void AddVariable(XmlName name, IXsltContextVariable variable)
		{
			this.EnsureVariables()[name] = variable;
		}

		public void AddFunction(XmlName name, IXsltContextFunction function)
		{
			this.EnsureFunctions()[name] = function;
		}

		private Dictionary<XmlName, IXsltContextVariable> EnsureVariables()
		{
			return this.variables ??
					(
						this.variables = (this.parent != null)
							? new Dictionary<XmlName, IXsltContextVariable>(this.parent.EnsureVariables())
							: new Dictionary<XmlName, IXsltContextVariable>()
						);
		}

		private Dictionary<XmlName, IXsltContextFunction> EnsureFunctions()
		{
			return this.functions ??
					(
						this.functions = (this.parent != null)
							? new Dictionary<XmlName, IXsltContextFunction>(this.parent.EnsureFunctions())
							: new Dictionary<XmlName, IXsltContextFunction>()
						);
		}

		public override IXsltContextVariable ResolveVariable(string prefix, string name)
		{
			return
				this.variables != null ? this.ResolveVariableCore(prefix, name) :
					this.parent != null ? this.parent.ResolveVariable(prefix, name) :
						null;
		}

		public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
		{
			return
				this.functions != null ? this.ResolveFunctionCore(prefix, name, argTypes) :
					this.parent != null ? this.parent.ResolveFunction(prefix, name, argTypes) :
						null;
		}

		private IXsltContextVariable ResolveVariableCore(string prefix, string name)
		{
			IXsltContextVariable variable;
			var key = new XmlName(name, prefix ?? string.Empty);
			this.variables.TryGetValue(key, out variable);
			return variable;
		}

		private IXsltContextFunction ResolveFunctionCore(string prefix, string name, XPathResultType[] argTypes)
		{
			IXsltContextFunction function;
			var key = new XmlName(name, prefix ?? string.Empty);
			this.functions.TryGetValue(key, out function);
			return function;
		}

		public void Enlist(CompiledXPath path)
		{
			path.SetContext(this.XPathContext);
		}
#endif
	}
}

#endif