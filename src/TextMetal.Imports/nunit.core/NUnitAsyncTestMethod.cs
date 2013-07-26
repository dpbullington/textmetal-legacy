using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace NUnit.Core
{
	public class NUnitAsyncTestMethod : NUnitTestMethod
	{
		#region Constructors/Destructors

		public NUnitAsyncTestMethod(MethodInfo method)
			: base(method)
		{
		}

		#endregion

		#region Fields/Constants

		private const string InnerExceptionsProperty = "InnerExceptions";
		private const string SystemAggregateException = "System.AggregateException";
		private const string TaskResultProperty = "Result";
		private const BindingFlags TaskResultPropertyBindingFlags = BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public;
		private const string TaskWaitMethod = "Wait";

		#endregion

		#region Methods/Operators

		private object RunTaskAsyncMethod(TestResult testResult)
		{
			try
			{
				object task = base.RunTestMethod(testResult);

				Reflect.InvokeMethod(this.method.ReturnType.GetMethod(TaskWaitMethod, new Type[0]), task);
				PropertyInfo resultProperty = Reflect.GetNamedProperty(this.method.ReturnType, TaskResultProperty, TaskResultPropertyBindingFlags);

				return resultProperty != null ? resultProperty.GetValue(task, null) : task;
			}
			catch (NUnitException e)
			{
				if (e.InnerException != null &&
				    e.InnerException.GetType().FullName.Equals(SystemAggregateException))
				{
					IList<Exception> inner = (IList<Exception>)e.InnerException.GetType()
						                                           .GetProperty(InnerExceptionsProperty).GetValue(e.InnerException, null);

					throw new NUnitException("Rethrown", inner[0]);
				}

				throw;
			}
		}

		protected override object RunTestMethod(TestResult testResult)
		{
			if (this.method.ReturnType == typeof(void))
				return this.RunVoidAsyncMethod(testResult);

			return this.RunTaskAsyncMethod(testResult);
		}

		private object RunVoidAsyncMethod(TestResult testResult)
		{
			var previousContext = SynchronizationContext.Current;
			var currentContext = new AsyncSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(currentContext);

			try
			{
				object result = base.RunTestMethod(testResult);

				try
				{
					currentContext.WaitForPendingOperationsToComplete();
				}
				catch (Exception e)
				{
					throw new NUnitException("Rethrown", e);
				}

				return result;
			}
			finally
			{
				SynchronizationContext.SetSynchronizationContext(previousContext);
			}
		}

		#endregion
	}
}