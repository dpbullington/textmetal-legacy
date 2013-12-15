// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
using System.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified
#if !SL3

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class CompiledXPathNode
	{
		#region Constructors/Destructors

		internal CompiledXPathNode()
		{
		}

		#endregion

		#region Fields/Constants

		private static readonly IList<CompiledXPathNode>
			NoDependencies = Array.AsReadOnly(new CompiledXPathNode[0]);

		private IList<CompiledXPathNode> dependencies;

		private bool isAttribute;
		private string localName;
		private CompiledXPathNode next;
		private string prefix;
		private CompiledXPathNode previous;
		private XPathExpression value;

		#endregion

		#region Properties/Indexers/Events

		public IList<CompiledXPathNode> Dependencies
		{
			get
			{
				return this.dependencies ?? (this.dependencies = new List<CompiledXPathNode>());
			}
		}

		public bool IsAttribute
		{
			get
			{
				return this.isAttribute;
			}
			internal set
			{
				this.isAttribute = value;
			}
		}

		public bool IsSelfReference
		{
			get
			{
				return this.localName == null;
			}
		}

		public bool IsSimple
		{
			get
			{
				return this.next == null && this.HasNoRealDependencies();
			}
		}

		public string LocalName
		{
			get
			{
				return this.localName;
			}
			internal set
			{
				this.localName = value;
			}
		}

		public CompiledXPathNode NextNode
		{
			get
			{
				return this.next;
			}
			internal set
			{
				this.next = value;
			}
		}

		public string Prefix
		{
			get
			{
				return this.prefix;
			}
			internal set
			{
				this.prefix = value;
			}
		}

		public CompiledXPathNode PreviousNode
		{
			get
			{
				return this.previous;
			}
			internal set
			{
				this.previous = value;
			}
		}

		public XPathExpression Value
		{
			get
			{
				return this.value ?? this.GetSelfReferenceValue();
			}
			internal set
			{
				this.value = value;
			}
		}

		#endregion

		#region Methods/Operators

		private XPathExpression GetSelfReferenceValue()
		{
			return this.dependencies != null
					&& this.dependencies.Count == 1
					&& this.dependencies[0].IsSelfReference
				? this.dependencies[0].value
				: null;
		}

		private bool HasNoRealDependencies()
		{
			return
				(
					this.dependencies == null ||
					this.dependencies.Count == 0 ||
					(
						this.dependencies.Count == 1 &&
						this.dependencies[0].IsSelfReference
						)
					);
		}

		internal virtual void Prepare()
		{
			this.dependencies = (this.dependencies != null)
				? Array.AsReadOnly(this.dependencies.ToArray())
				: NoDependencies;

			foreach (var child in this.dependencies)
				child.Prepare();

			if (this.next != null)
				this.next.Prepare();
		}

		internal virtual void SetContext(XsltContext context)
		{
			if (this.value != null)
				this.value.SetContext(context);

			foreach (var child in this.dependencies)
				child.SetContext(context);

			if (this.next != null)
				this.next.SetContext(context);
		}

		#endregion
	}
}

#endif
#endif