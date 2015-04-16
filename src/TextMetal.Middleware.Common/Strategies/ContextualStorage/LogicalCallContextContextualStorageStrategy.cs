/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Runtime.Remoting.Messaging;

namespace TextMetal.Middleware.Common.Strategies.ContextualStorage
{
	public sealed class LogicalCallContextContextualStorageStrategy : IContextualStorageStrategy
	{
		#region Constructors/Destructors

		public LogicalCallContextContextualStorageStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		public T GetValue<T>(string key)
		{
			return (T)CallContext.LogicalGetData(key);
		}

		public bool HasValue(string key)
		{
			return (object)CallContext.LogicalGetData(key) == null;
		}

		public void RemoveValue(string key)
		{
			CallContext.FreeNamedDataSlot(key);
		}

		public void SetValue<T>(string key, T value)
		{
			CallContext.LogicalSetData(key, value);
		}

		#endregion
	}
}