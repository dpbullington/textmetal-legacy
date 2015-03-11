/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Core
{
	public interface IContentContainerXmlObject<TContentRestriction> : IContainerXmlObject
		where TContentRestriction : IXmlObject
	{
		#region Properties/Indexers/Events

		TContentRestriction Content
		{
			get;
			set;
		}

		#endregion
	}
}