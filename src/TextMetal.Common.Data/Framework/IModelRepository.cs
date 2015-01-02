/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.Framework
{
	public interface IModelRepository : IUnitOfWorkFactory
	{
		#region Methods/Operators

		bool TrySendEmailTemplate(string templateResourceName, object modelObject);

		bool TryWriteEventLogEntry(string eventText);

		#endregion
	}
}