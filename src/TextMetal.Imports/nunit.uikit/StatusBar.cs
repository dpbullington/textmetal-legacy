// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NUnit.Core;
using NUnit.Util;

namespace NUnit.UiKit
{
	public class StatusBar : System.Windows.Forms.StatusBar, TestObserver
	{
		#region Constructors/Destructors

		public StatusBar()
		{
			this.Panels.Add(this.statusPanel);
			this.Panels.Add(this.testCountPanel);
			this.Panels.Add(this.testsRunPanel);
			this.Panels.Add(this.errorsPanel);
			this.Panels.Add(this.failuresPanel);
			this.Panels.Add(this.timePanel);

			this.statusPanel.AutoSize = StatusBarPanelAutoSize.Spring;
			this.statusPanel.BorderStyle = StatusBarPanelBorderStyle.None;
			this.statusPanel.Text = "Status";

			this.testCountPanel.AutoSize = StatusBarPanelAutoSize.Contents;
			this.testsRunPanel.AutoSize = StatusBarPanelAutoSize.Contents;
			this.errorsPanel.AutoSize = StatusBarPanelAutoSize.Contents;
			this.failuresPanel.AutoSize = StatusBarPanelAutoSize.Contents;
			this.timePanel.AutoSize = StatusBarPanelAutoSize.Contents;

			this.ShowPanels = true;
			this.InitPanels();
		}

		#endregion

		#region Fields/Constants

		private bool displayProgress = false;
		private int errors = 0;

		private StatusBarPanel errorsPanel = new StatusBarPanel();
		private int failures = 0;
		private StatusBarPanel failuresPanel = new StatusBarPanel();
		private StatusBarPanel statusPanel = new StatusBarPanel();

		private int testCount = 0;
		private StatusBarPanel testCountPanel = new StatusBarPanel();
		private int testsRun = 0;
		private StatusBarPanel testsRunPanel = new StatusBarPanel();
		private double time = 0.0;
		private StatusBarPanel timePanel = new StatusBarPanel();

		#endregion

		#region Properties/Indexers/Events

		public bool DisplayTestProgress
		{
			get
			{
				return this.displayProgress;
			}
			set
			{
				this.displayProgress = value;
			}
		}

		// Kluge to keep VS from generating code that sets the Panels for
		// the statusbar. Really, our status bar should be a user control
		// to avoid this and shouldn't allow the panels to be set except
		// according to specific protocols.
		[DesignerSerializationVisibility(
			DesignerSerializationVisibility.Hidden)]
		public new StatusBarPanelCollection Panels
		{
			get
			{
				return base.Panels;
			}
		}

		public override string Text
		{
			get
			{
				return this.statusPanel.Text;
			}
			set
			{
				this.statusPanel.Text = value;
			}
		}

		#endregion

		#region Methods/Operators

		private void DisplayErrors()
		{
			this.errorsPanel.Text = "Errors : " + this.errors.ToString();
		}

		private void DisplayFailures()
		{
			this.failuresPanel.Text = "Failures : " + this.failures.ToString();
		}

		private void DisplayResult(TestResult result)
		{
			ResultSummarizer summarizer = new ResultSummarizer(result);

			//this.testCount = summarizer.ResultCount;
			this.testsRun = summarizer.TestsRun;
			this.errors = summarizer.Errors;
			this.failures = summarizer.Failures;
			this.time = summarizer.Time;

			this.DisplayTestCount();
			this.DisplayTestsRun();
			this.DisplayErrors();
			this.DisplayFailures();
			this.DisplayTime();
		}

		private void DisplayTestCount()
		{
			this.testCountPanel.Text = "Test Cases : " + this.testCount.ToString();
		}

		private void DisplayTestsRun()
		{
			this.testsRunPanel.Text = "Tests Run : " + this.testsRun.ToString();
		}

		private void DisplayTime()
		{
			this.timePanel.Text = "Time : " + this.time.ToString();
		}

		private void InitPanels()
		{
			this.testCountPanel.MinWidth = 50;
			this.DisplayTestCount();

			this.testsRunPanel.MinWidth = 50;
			this.testsRunPanel.Text = "";

			this.errorsPanel.MinWidth = 50;
			this.errorsPanel.Text = "";

			this.failuresPanel.MinWidth = 50;
			this.failuresPanel.Text = "";

			this.timePanel.MinWidth = 50;
			this.timePanel.Text = "";
		}

		public void Initialize(int testCount)
		{
			this.Initialize(testCount, testCount > 0 ? "Ready" : "");
		}

		public void Initialize(int testCount, string text)
		{
			this.statusPanel.Text = text;

			this.testCount = testCount;
			this.testsRun = 0;
			this.errors = 0;
			this.failures = 0;
			this.time = 0.0;

			this.InitPanels();
		}

		private void OnRunFinished(object sender, TestEventArgs e)
		{
			if (e.Exception != null)
				this.statusPanel.Text = "Failed";
			else
			{
				this.statusPanel.Text = "Completed";
				this.DisplayResult(e.Result);
			}
		}

		private void OnRunStarting(object sender, TestEventArgs e)
		{
			this.Initialize(e.TestCount, "Running :" + e.Name);
			this.DisplayTestCount();
			this.DisplayTestsRun();
			this.DisplayErrors();
			this.DisplayFailures();
			this.DisplayTime();
		}

		private void OnTestFinished(object sender, TestEventArgs e)
		{
			if (this.DisplayTestProgress && e.Result.Executed)
			{
				++this.testsRun;
				this.DisplayTestsRun();
				switch (e.Result.ResultState)
				{
					case ResultState.Error:
					case ResultState.Cancelled:
						++this.errors;
						this.DisplayErrors();
						break;
					case ResultState.Failure:
						++this.failures;
						this.DisplayFailures();
						break;
				}
			}
		}

		public void OnTestLoaded(object sender, TestEventArgs e)
		{
			this.Initialize(e.TestCount);
		}

		public void OnTestReloaded(object sender, TestEventArgs e)
		{
			this.Initialize(e.TestCount, "Reloaded");
		}

		public void OnTestStarting(object sender, TestEventArgs e)
		{
			string fullText = "Running : " + e.TestName.FullName;
			string shortText = "Running : " + e.TestName.Name;

			Graphics g = Graphics.FromHwnd(this.Handle);
			SizeF sizeNeeded = g.MeasureString(fullText, this.Font);
			if (this.statusPanel.Width >= (int)sizeNeeded.Width)
			{
				this.statusPanel.Text = fullText;
				this.statusPanel.ToolTipText = "";
			}
			else
			{
				sizeNeeded = g.MeasureString(shortText, this.Font);
				this.statusPanel.Text = this.statusPanel.Width >= (int)sizeNeeded.Width
					? shortText : e.TestName.Name;
				this.statusPanel.ToolTipText = e.TestName.FullName;
			}
		}

		public void OnTestUnloaded(object sender, TestEventArgs e)
		{
			this.Initialize(0, "Unloaded");
		}

//        protected override void OnFontChanged(EventArgs e)
//        {
//            base.OnFontChanged(e);
//
//            this.Height = (int)(this.Font.Height * 1.6);
//        }

		public void Subscribe(ITestEvents events)
		{
			events.TestLoaded += new TestEventHandler(this.OnTestLoaded);
			events.TestReloaded += new TestEventHandler(this.OnTestReloaded);
			events.TestUnloaded += new TestEventHandler(this.OnTestUnloaded);

			events.TestStarting += new TestEventHandler(this.OnTestStarting);
			events.TestFinished += new TestEventHandler(this.OnTestFinished);
			events.RunStarting += new TestEventHandler(this.OnRunStarting);
			events.RunFinished += new TestEventHandler(this.OnRunFinished);
		}

		#endregion
	}
}