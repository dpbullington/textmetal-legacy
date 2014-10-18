/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Runtime.Remoting.Messaging;

namespace TextMetal.Common.Solder.AmbientExecutionContext
{
	public sealed class LogicalCallContextExecutionPathStorage : IExecutionPathStorage
	{
		#region Constructors/Destructors

		public LogicalCallContextExecutionPathStorage()
		{
		}

		#endregion

		#region Methods/Operators

		public object GetValue(string key)
		{
			return CallContext.LogicalGetData(key);
		}

		public void RemoveValue(string key)
		{
			CallContext.FreeNamedDataSlot(key);
		}

		public void SetValue(string key, object value)
		{
			CallContext.LogicalSetData(key, value);
		}

		#endregion
	}
}