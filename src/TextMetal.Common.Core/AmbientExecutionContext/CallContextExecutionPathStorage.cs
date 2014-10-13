/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Runtime.Remoting.Messaging;

namespace TextMetal.Common.Core.AmbientExecutionContext
{
	public sealed class CallContextExecutionPathStorage : IExecutionPathStorage
	{
		#region Constructors/Destructors

		public CallContextExecutionPathStorage()
		{
		}

		#endregion

		#region Methods/Operators

		public object GetValue(string key)
		{
			return CallContext.GetData(key);
		}

		public void RemoveValue(string key)
		{
			CallContext.FreeNamedDataSlot(key);
		}

		public void SetValue(string key, object value)
		{
			CallContext.SetData(key, value);
		}

		#endregion
	}
}