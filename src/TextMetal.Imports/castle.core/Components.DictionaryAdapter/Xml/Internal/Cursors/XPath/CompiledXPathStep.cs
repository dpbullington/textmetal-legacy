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

using System.Xml.XPath;
using System.Xml.Xsl;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified
#if !SL3

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class CompiledXPathStep : CompiledXPathNode
	{
		#region Constructors/Destructors

		internal CompiledXPathStep()
		{
		}

		#endregion

		#region Fields/Constants

		private XPathExpression path;

		#endregion

		#region Properties/Indexers/Events

		public CompiledXPathStep NextStep
		{
			get
			{
				return (CompiledXPathStep)this.NextNode;
			}
		}

		public XPathExpression Path
		{
			get
			{
				return this.path;
			}
			internal set
			{
				this.path = value;
			}
		}

		#endregion

		#region Methods/Operators

		internal override void SetContext(XsltContext context)
		{
			this.path.SetContext(context);
			base.SetContext(context);
		}

		#endregion
	}
}

#endif
#endif