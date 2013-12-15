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
	/// <summary>
	/// ColorProgressBar provides a custom progress bar with the
	/// ability to control the color of the bar and to render itself
	/// in either solid or segmented style. The bar can be updated
	/// on the fly and has code to avoid repainting the entire bar
	/// when that occurs.
	/// </summary>
	public class ColorProgressBar : Control
	{
		#region Constructors/Destructors

		public ColorProgressBar()
		{
			// This call is required by the Windows.Forms Form Designer.
			this.InitializeComponent();

			this.SetStyle(ControlStyles.ResizeRedraw, true);
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// The brush to use in painting the background of the bar
		/// </summary>
		private Brush backBrush = null;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		/// <summary>
		/// The brush to use in painting the progress bar
		/// </summary>
		private Brush foreBrush = null;

		/// <summary>
		/// Last segment displayed when displaying asynchronously rather
		/// than through OnPaint calls.
		/// </summary>
		private int lastSegmentCount = 0;

		/// <summary>
		/// The maximum value allowed
		/// </summary>
		private int max = 100;

		/// <summary>
		/// The minimum value allowed
		/// </summary>
		private int min = 0;

		/// <summary>
		/// Indicates whether to draw the bar in segments or not
		/// </summary>
		private bool segmented = false;

		/// <summary>
		/// Amount to advance for each step
		/// </summary>
		private int step = 1;

		/// <summary>
		/// The current progress value
		/// </summary>
		private int val = 0;

		#endregion

		#region Properties/Indexers/Events

		[Category("Behavior")]
		public int Maximum
		{
			get
			{
				return this.max;
			}
			set
			{
				if (value >= this.Minimum)
				{
					if (this.max != value)
					{
						this.max = value;
						this.Invalidate();
					}
				}
				else
				{
					throw new ArgumentOutOfRangeException("Maximum", value
						, "Maximum must be >= Minimum.");
				}
			}
		}

		[Category("Behavior")]
		public int Minimum
		{
			get
			{
				return this.min;
			}
			set
			{
				if (value <= this.Maximum)
				{
					if (this.min != value)
					{
						this.min = value;
						this.Invalidate();
					}
				}
				else
				{
					throw new ArgumentOutOfRangeException("Minimum", value
						, "Minimum must be <= Maximum.");
				}
			}
		}

		[Browsable(false)]
		private float PercentValue
		{
			get
			{
				if (0 != this.Maximum - this.Minimum) // NRG 05/28/03: Prevent divide by zero
					return ((float)this.val / ((float)this.Maximum - (float)this.Minimum));
				else
					return (0);
			}
		}

		[Category("Appearance")]
		public bool Segmented
		{
			get
			{
				return this.segmented;
			}
			set
			{
				this.segmented = value;
			}
		}

		[Category("Behavior")]
		public int Step
		{
			get
			{
				return this.step;
			}
			set
			{
				if (value <= this.Maximum && value >= this.Minimum)
					this.step = value;
				else
				{
					throw new ArgumentOutOfRangeException("Step", value
						, "Must fall between Minimum and Maximum inclusive.");
				}
			}
		}

		[Category("Behavior")]
		public int Value
		{
			get
			{
				return this.val;
			}
			set
			{
				if (value == this.val)
					return;
				else if (value <= this.Maximum && value >= this.Minimum)
				{
					this.val = value;
					this.Invalidate();
				}
				else
				{
					throw new ArgumentOutOfRangeException("Value", value
						, "Must fall between Minimum and Maximum inclusive.");
				}
			}
		}

		#endregion

		#region Methods/Operators

		private void AcquireBrushes()
		{
			if (this.foreBrush == null)
			{
				this.foreBrush = new SolidBrush(this.ForeColor);
				this.backBrush = new SolidBrush(this.BackColor);
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.components != null)
					this.components.Dispose();
				this.ReleaseBrushes();
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// ProgressBar
			// 
			this.CausesValidation = false;
			this.Enabled = false;
			this.ForeColor = SystemColors.Highlight;
			this.Name = "ProgressBar";
			this.Size = new Size(432, 24);
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			base.OnBackColorChanged(e);
			this.Refresh();
		}

		protected override void OnCreateControl()
		{
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			base.OnForeColorChanged(e);
			this.Refresh();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			this.lastSegmentCount = 0;
			this.ReleaseBrushes();
			this.PaintBar(e.Graphics);
			ControlPaint.DrawBorder3D(
				e.Graphics
				, this.ClientRectangle
				, Border3DStyle.SunkenOuter);
			//e.Graphics.Flush();
		}

		private void PaintBar(Graphics g)
		{
			Rectangle theBar = Rectangle.Inflate(this.ClientRectangle, -2, -2);
			int maxRight = theBar.Right - 1;
			this.AcquireBrushes();

			if (this.segmented)
			{
				int segmentWidth = (int)((float)this.ClientRectangle.Height * 0.66f);
				int maxSegmentCount = (theBar.Width + segmentWidth) / segmentWidth;

				//int maxRight = Bar.Right;
				int newSegmentCount = (int)Math.Ceiling(this.PercentValue * maxSegmentCount);
				if (newSegmentCount > this.lastSegmentCount)
				{
					theBar.X += this.lastSegmentCount * segmentWidth;
					while (this.lastSegmentCount < newSegmentCount)
					{
						theBar.Width = Math.Min(maxRight - theBar.X, segmentWidth - 2);
						g.FillRectangle(this.foreBrush, theBar);
						theBar.X += segmentWidth;
						this.lastSegmentCount++;
					}
				}
				else if (newSegmentCount < this.lastSegmentCount)
				{
					theBar.X += newSegmentCount * segmentWidth;
					theBar.Width = maxRight - theBar.X;
					g.FillRectangle(this.backBrush, theBar);
					this.lastSegmentCount = newSegmentCount;
				}
			}
			else
			{
				//g.FillRectangle( backBrush, theBar );
				theBar.Width = theBar.Width * this.val / this.max;
				g.FillRectangle(this.foreBrush, theBar);
			}

			if (this.Value == this.Minimum || this.Value == this.Maximum)
				this.ReleaseBrushes();
		}

		public void PerformStep()
		{
			int newValue = this.Value + this.Step;

			if (newValue > this.Maximum)
				newValue = this.Maximum;

			this.Value = newValue;
		}

		private void ReleaseBrushes()
		{
			if (this.foreBrush != null)
			{
				this.foreBrush.Dispose();
				this.backBrush.Dispose();
				this.foreBrush = null;
				this.backBrush = null;
			}
		}

		#endregion
	}

	public class TestProgressBar : ColorProgressBar, TestObserver
	{
		#region Constructors/Destructors

		public TestProgressBar()
		{
			this.Initialize(100);
		}

		#endregion

		#region Fields/Constants

		private static readonly Color FailureColor = Color.Red;
		private static readonly Color IgnoredColor = Color.Yellow;
		private static readonly Color SuccessColor = Color.Lime;

		#endregion

		#region Methods/Operators

		private void Initialize(int testCount)
		{
			this.Value = 0;
			this.Maximum = testCount;
			this.ForeColor = SuccessColor;
		}

		private void OnRunStarting(object Sender, TestEventArgs e)
		{
			this.Initialize(e.TestCount);
		}

		private void OnSuiteFinished(object sender, TestEventArgs e)
		{
			TestResult result = e.Result;
			if (result.FailureSite == FailureSite.TearDown)
			{
				switch (result.ResultState)
				{
					case ResultState.Error:
					case ResultState.Failure:
					case ResultState.Cancelled:
						this.ForeColor = FailureColor;
						break;
				}
			}
		}

		private void OnTestException(object sender, TestEventArgs e)
		{
			this.ForeColor = FailureColor;
		}

		private void OnTestFinished(object sender, TestEventArgs e)
		{
			this.PerformStep();

			switch (e.Result.ResultState)
			{
				case ResultState.NotRunnable:
				case ResultState.Failure:
				case ResultState.Error:
				case ResultState.Cancelled:
					this.ForeColor = FailureColor;
					break;
				case ResultState.Ignored:
					if (this.ForeColor == SuccessColor)
						this.ForeColor = IgnoredColor;
					break;
				default:
					break;
			}
		}

		private void OnTestLoaded(object sender, TestEventArgs e)
		{
			this.Initialize(e.TestCount);
		}

		private void OnTestReloaded(object sender, TestEventArgs e)
		{
			if (Services.UserSettings.GetSetting("Options.TestLoader.ClearResultsOnReload", false))
				this.Initialize(e.TestCount);
			else
				this.Value = this.Maximum = e.TestCount;
		}

		private void OnTestUnloaded(object sender, TestEventArgs e)
		{
			this.Initialize(100);
		}

		public void Subscribe(ITestEvents events)
		{
			events.TestLoaded += new TestEventHandler(this.OnTestLoaded);
			events.TestReloaded += new TestEventHandler(this.OnTestReloaded);
			events.TestUnloaded += new TestEventHandler(this.OnTestUnloaded);
			events.RunStarting += new TestEventHandler(this.OnRunStarting);
			events.TestFinished += new TestEventHandler(this.OnTestFinished);
			events.SuiteFinished += new TestEventHandler(this.OnSuiteFinished);
			events.TestException += new TestEventHandler(this.OnTestException);
		}

		#endregion
	}
}