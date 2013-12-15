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
using System.Xml.XPath;
using System.Xml.Xsl;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified
#if !SL3

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class CompiledXPath
	{
		#region Constructors/Destructors

		internal CompiledXPath()
		{
		}

		#endregion

		#region Fields/Constants

		private static readonly IList<CompiledXPathStep>
			NoSteps = Array.AsReadOnly(new CompiledXPathStep[0]);

		private int depth;
		private CompiledXPathStep firstStep;
		private XPathExpression path;

		#endregion

		#region Properties/Indexers/Events

		public int Depth
		{
			get
			{
				return this.depth;
			}
			internal set
			{
				this.depth = value;
			}
		}

		public CompiledXPathStep FirstStep
		{
			get
			{
				return this.firstStep;
			}
			internal set
			{
				this.firstStep = value;
			}
		}

		public bool IsCreatable
		{
			get
			{
				return this.firstStep != null;
			}
		}

		public CompiledXPathStep LastStep
		{
			get
			{
				var step = null as CompiledXPathStep;
				var next = this.firstStep;

				while (next != null)
				{
					step = next;
					next = step.NextStep;
				}

				return step;
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

		internal void MakeNotCreatable()
		{
			this.firstStep = null;
			this.depth = 0;
		}

		internal void Prepare()
		{
			if (this.firstStep != null)
				this.firstStep.Prepare();
		}

		public void SetContext(XsltContext context)
		{
			this.path.SetContext(context);

			if (this.firstStep != null)
				this.firstStep.SetContext(context);
		}

		#endregion
	}
}

#endif
#endif