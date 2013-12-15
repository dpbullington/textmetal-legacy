// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;

namespace NUnit.Core.Extensibility
{
	/// <summary>
	/// EventListenerCollection holds multiple event listeners
	/// and relays all event calls to each of them.
	/// </summary>
	public class EventListenerCollection : ExtensionPoint, EventListener
	{
		#region Constructors/Destructors

		public EventListenerCollection(IExtensionHost host)
			: base("EventListeners", host)
		{
		}

		#endregion

		#region Methods/Operators

		protected override bool IsValidExtension(object extension)
		{
			return extension is EventListener;
		}

		public void RunFinished(TestResult result)
		{
			foreach (EventListener listener in this.Extensions)
				listener.RunFinished(result);
		}

		public void RunFinished(Exception exception)
		{
			foreach (EventListener listener in this.Extensions)
				listener.RunFinished(exception);
		}

		public void RunStarted(string name, int testCount)
		{
			foreach (EventListener listener in this.Extensions)
				listener.RunStarted(name, testCount);
		}

		public void SuiteFinished(TestResult result)
		{
			foreach (EventListener listener in this.Extensions)
				listener.SuiteFinished(result);
		}

		public void SuiteStarted(TestName testName)
		{
			foreach (EventListener listener in this.Extensions)
				listener.SuiteStarted(testName);
		}

		public void TestFinished(TestResult result)
		{
			foreach (EventListener listener in this.Extensions)
				listener.TestFinished(result);
		}

		public void TestOutput(TestOutput testOutput)
		{
			foreach (EventListener listener in this.Extensions)
				listener.TestOutput(testOutput);
		}

		public void TestStarted(TestName testName)
		{
			foreach (EventListener listener in this.Extensions)
				listener.TestStarted(testName);
		}

		public void UnhandledException(Exception exception)
		{
			foreach (EventListener listener in this.Extensions)
				listener.UnhandledException(exception);
		}

		#endregion
	}
}