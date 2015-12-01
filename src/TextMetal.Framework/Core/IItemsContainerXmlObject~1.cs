/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Core
{
	public interface IItemsContainerXmlObject<TItemsRestriction> : IContainerXmlObject
		where TItemsRestriction : IXmlObject
	{
		#region Properties/Indexers/Events

		IXmlObjectCollection<TItemsRestriction> Items
		{
			get;
		}

		#endregion
	}
}