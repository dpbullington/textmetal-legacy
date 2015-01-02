/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.Framework
{
	/// <summary>
	/// Provides a contract for response model objects (procedure, function, etc.).
	/// </summary>
	public interface IResponseModelObject : IBasicObject
	{
		#region Properties/Indexers/Events

		bool EnumerationComplete
		{
			get;
		}

		#endregion

		#region Methods/Operators

		void SignalEnumerationComplete();

		#endregion
	}
}