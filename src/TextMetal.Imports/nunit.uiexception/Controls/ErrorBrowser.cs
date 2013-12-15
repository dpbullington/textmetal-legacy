// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Windows.Forms;

namespace NUnit.UiException.Controls
{
	/// <summary>
	/// A control that encapsulates a collection of IErrorDisplay instances
	/// and which shows relevant information about failures & errors after
	/// a test suite run.
	/// By default, ErrorBrowser is empty and should be populated with
	/// IErrorDisplay instances at loading time. The example below shows how
	/// to achieve this:
	/// <code>ErrorBrowser errorBrowser = new ErrorBrowser();
	///  
	/// 		// configure and register a SourceCodeDisplay
	/// 		// that will display source code context on failure
	///  
	/// 		SourceCodeDisplay sourceCode = new SourceCodeDisplay();
	/// 		sourceCode.AutoSelectFirstItem = true;
	/// 		sourceCode.ListOrderPolicy = ErrorListOrderPolicy.ReverseOrder;
	/// 		sourceCode.SplitOrientation = Orientation.Vertical;
	/// 		sourceCode.SplitterDistance = 0.3f;
	/// 
	/// 		errorBrowser.RegisterDisplay(sourceCode);
	///  
	/// 		// configure and register a StackTraceDisplay
	/// 		// that will display the stack trace details on failure
	///  
	/// 		errorBrowser.RegisterDisplay(new StackTraceDisplay());
	/// 		[...]
	/// 		// set the stack trace information
	/// 		errorBrowser.StackTraceSource = [a stack trace here]</code>
	/// </summary>
	public class ErrorBrowser : UserControl
	{
		#region Constructors/Destructors

		/// <summary>
		/// Builds a new instance of ErrorBrowser.
		/// </summary>
		public ErrorBrowser()
		{
			this._layout = new ErrorPanelLayout();
			this._layout.Toolbar = new ErrorToolbar();
			this.Toolbar.SelectedRendererChanged += new EventHandler(this.Toolbar_SelectedRendererChanged);

			this.Controls.Add(this._layout);
			this._layout.Left = 0;
			this._layout.Top = 0;
			this._layout.Width = this.Width;
			this._layout.Height = this.Height;

			this._layout.Anchor = AnchorStyles.Top |
								AnchorStyles.Left |
								AnchorStyles.Bottom |
								AnchorStyles.Right;

			return;
		}

		#endregion

		#region Fields/Constants

		private ErrorPanelLayout _layout;

		private string _stackStace;

		#endregion

		#region Properties/Indexers/Events

		public event EventHandler StackTraceDisplayChanged;
		public event EventHandler StackTraceSourceChanged;

		protected ErrorPanelLayout LayoutPanel
		{
			get
			{
				return (this._layout);
			}
		}

		/// <summary>
		/// Gets the selected display.
		/// </summary>
		public IErrorDisplay SelectedDisplay
		{
			get
			{
				return (this.Toolbar.SelectedDisplay);
			}
			set
			{
				this.Toolbar.SelectedDisplay = value;
			}
		}

		/// <summary>
		/// Use this property to get or set the new stack trace details.
		/// The changes are repercuted on the registered displays.
		/// </summary>
		public string StackTraceSource
		{
			get
			{
				return (this._stackStace);
			}
			set
			{
				if (this._stackStace == value)
					return;

				this._stackStace = value;

				foreach (IErrorDisplay item in this.Toolbar)
					item.OnStackTraceChanged(value);

				if (this.StackTraceSourceChanged != null)
					this.StackTraceSourceChanged(this, new EventArgs());

				return;
			}
		}

		protected ErrorToolbar Toolbar
		{
			get
			{
				return ((ErrorToolbar)this._layout.Toolbar);
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Removes all display from ErrorBrowser.
		/// </summary>
		public void ClearAll()
		{
			this.Toolbar.Clear();

			this.LayoutPanel.Option = null;
			this.LayoutPanel.Content = null;

			return;
		}

		/// <summary>
		/// Populates ErrorBrowser with the new display passed in parameter.
		/// If ErrorBrowser is empty, the display becomes automatically the
		/// new selected display.
		/// </summary>
		/// <param name="display"> </param>
		public void RegisterDisplay(IErrorDisplay display)
		{
			UiExceptionHelper.CheckNotNull(display, "display");

			this.Toolbar.Register(display);
			display.OnStackTraceChanged(this._stackStace);

			if (this.Toolbar.SelectedDisplay == null)
				this.Toolbar.SelectedDisplay = display;

			return;
		}

		private void Toolbar_SelectedRendererChanged(object sender, EventArgs e)
		{
			this.LayoutPanel.Content = this.Toolbar.SelectedDisplay.Content;

			if (this.StackTraceDisplayChanged != null)
				this.StackTraceDisplayChanged(this, EventArgs.Empty);

			return;
		}

		#endregion
	}
}