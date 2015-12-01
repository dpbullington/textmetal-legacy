/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Runtime.Remoting.Messaging;

namespace TextMetal.Middleware.Solder.Context
{
	public sealed class CallContextContextualStorageStrategy : IContextualStorageStrategy
	{
		#region Constructors/Destructors

		public CallContextContextualStorageStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		public T GetValue<T>(string key)
		{
			return (T)CallContext.GetData(key);
		}

		public bool HasValue(string key)
		{
			return (object)CallContext.GetData(key) == null;
		}

		public void RemoveValue(string key)
		{
			CallContext.FreeNamedDataSlot(key);
		}

		public void SetValue<T>(string key, T value)
		{
			CallContext.SetData(key, value);
		}

		#endregion
	}
}