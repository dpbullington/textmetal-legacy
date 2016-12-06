/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Framework.Hosting;
using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Source.Primative
{
	public class WellKnownXmlPersistEngineSourceStrategy : XmlPersistEngineSourceStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the WellKnownXmlPersistEngineSourceStrategy class.
		/// </summary>
		public WellKnownXmlPersistEngineSourceStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		protected override IXmlPersistEngine GetXmlPersistEngine(IDictionary<string, IList<string>> properties)
		{
			IXmlPersistEngine xpe;

			xpe = new XmlPersistEngine();
			xpe.RegisterWellKnownConstructs();

			return xpe;
		}

		#endregion
	}
}