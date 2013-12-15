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

using System.Xml.XPath;
using System.Xml.Xsl;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified
#if !SL3

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true)]
	public abstract class XPathFunctionAttribute : Attribute, IXsltContextFunction
	{
		#region Constructors/Destructors

		protected XPathFunctionAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		public static readonly XPathResultType[]
			NoArgs = new XPathResultType[0];

		#endregion

		#region Properties/Indexers/Events

		public virtual XPathResultType[] ArgTypes
		{
			get
			{
				return NoArgs;
			}
		}

		public virtual int Maxargs
		{
			get
			{
				return this.ArgTypes.Length;
			}
		}

		public virtual int Minargs
		{
			get
			{
				return this.ArgTypes.Length;
			}
		}

		public abstract XmlName Name
		{
			get;
		}

		public abstract XPathResultType ReturnType
		{
			get;
		}

		#endregion

		#region Methods/Operators

		public abstract object Invoke(XsltContext context, object[] args, XPathNavigator node);

		#endregion
	}
}

#endif
#endif