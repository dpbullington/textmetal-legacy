/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;

namespace TextMetal.Common.Solder.RuntimeInterception
{
	public class LoggingAspectDynamicInvoker : AspectDynamicInvoker
	{
		#region Constructors/Destructors

		public LoggingAspectDynamicInvoker(object logee)
		{
			if ((object)logee == null)
				throw new ArgumentNullException("logee");

			this.logee = logee;
		}

		#endregion

		#region Fields/Constants

		private readonly object logee;

		#endregion

		#region Properties/Indexers/Events

		public override object InterceptedInstance
		{
			get
			{
				return this.Logee;
			}
		}

		private object Logee
		{
			get
			{
				return this.logee;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void OnInterceptAfterInvoke(bool invocationPreceeded, Type proxiedType, MethodInfo invokedMethodInfo, object proxyInstance, object[] invocationParameters, ref object returnValue, ref Exception thrownException)
		{
			var oldConsoleColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("after invoke: {0}::{1}", proxiedType.Name, invokedMethodInfo.Name);
			Console.ForegroundColor = oldConsoleColor;
		}

		protected override void OnInterceptBeforeInvoke(out bool proceedWithInvocation, Type proxiedType, MethodInfo invokedMethodInfo, object proxyInstance, object[] invocationParameters)
		{
			var oldConsoleColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("before invoke: {0}::{1}", proxiedType.Name, invokedMethodInfo.Name);
			Console.ForegroundColor = oldConsoleColor;
			proceedWithInvocation = true;
		}

		protected override object OnInterceptProceedInvoke(Type proxiedType, MethodInfo invokedMethodInfo, object proxyInstance, object[] invocationParameters)
		{
			object returnValue = null;

			if ((object)proxiedType == null)
				throw new ArgumentNullException("proxiedType");

			if ((object)invokedMethodInfo == null)
				throw new ArgumentNullException("invokedMethodInfo");

			if ((object)proxyInstance == null)
				throw new ArgumentNullException("proxyInstance");

			if ((object)invocationParameters == null)
				throw new ArgumentNullException("invocationParameters");

			returnValue = base.OnInterceptProceedInvoke(proxiedType, invokedMethodInfo, proxyInstance, invocationParameters);

			return returnValue;
		}

		#endregion
	}
}