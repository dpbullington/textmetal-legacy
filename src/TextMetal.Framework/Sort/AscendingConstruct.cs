/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Sort
{
	[XmlElementMapping(LocalName = "Ascending", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Sterile)]
	public sealed class AscendingConstruct : OrderConstruct
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the AscendingConstruct class.
		/// </summary>
		public AscendingConstruct()
			: base(true)
		{
		}

		#endregion
	}
}