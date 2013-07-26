// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Drawing;
using System.Windows.Forms;

using NUnit.UiException.CodeFormatters;
using NUnit.UiException.Properties;

namespace NUnit.UiException.Controls
{
	/// <summary>
	/// 	Implements IErrorDisplay and displays data about failures and error
	/// 	after a test suite run. SourceCodeDisplay is a control composed of two
	/// 	views. 
	/// 
	/// 	The first view displays the stack trace in an ordered list of items
	/// 	where each item contains the context about a specific failure (file, class
	/// 	method, line number).
	/// 
	/// 	The second view displays a CodeBox control and shows the source code
	/// 	of one element in this list when the localization is available.
	/// </summary>
	public class SourceCodeDisplay :
		IErrorDisplay
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Builds a new instance of SourceCodeDisplay.
		/// </summary>
		public SourceCodeDisplay()
		{
			ErrorList errorList = new ErrorList();
			this._codeBox = new CodeBox();

			this._stacktraceView = errorList;
			this._stacktraceView.AutoSelectFirstItem = true;
			this._stacktraceView.SelectedItemChanged += new EventHandler(this.SelectedItemChanged);
			this._codeView = this._codeBox;

			this._btnPlugin = ErrorToolbar.NewStripButton(true, "Display source code context", Resources.ImageSourceCodeDisplay, null);
			this._btnSwap = ErrorToolbar.NewStripButton(false, "ReverseOrder item order", Resources.ImageReverseItemOrder, this.OnClick);

			SplitterBox splitter = new SplitterBox();
			this._splitter = splitter;
			this._splitter.SplitterDistanceChanged += new EventHandler(this._splitter_DistanceChanged);
			this._splitter.OrientationChanged += new EventHandler(this._splitter_OrientationChanged);

			splitter.Control1 = errorList;
			splitter.Control2 = this._codeBox;

			this._codeBox.ShowCurrentLine = true;

			return;
		}

		#endregion

		#region Fields/Constants

		private ToolStripButton _btnPlugin;
		private ToolStripButton _btnSwap;
		private CodeBox _codeBox;
		protected ICodeView _codeView;
		protected SplitterBox _splitter;
		protected IStackTraceView _stacktraceView;

		#endregion

		#region Properties/Indexers/Events

		public event EventHandler SplitOrientationChanged;
		public event EventHandler SplitterDistanceChanged;

		/// <summary>
		/// 	Gets or sets a value telling whether or not to select automatically
		/// 	the first localizable item each time the stack trace changes.
		/// 	When set to true, the first localizable item will be selected 
		/// 	and the source code context for this item displayed automatically.
		/// 	Default is True.
		/// </summary>
		public bool AutoSelectFirstItem
		{
			get
			{
				return (this._stacktraceView.AutoSelectFirstItem);
			}
			set
			{
				this._stacktraceView.AutoSelectFirstItem = value;
			}
		}

		public Font CodeDisplayFont
		{
			get
			{
				return this._codeBox.Font;
			}
			set
			{
				this._codeBox.Font = value;
			}
		}

		public Control Content
		{
			get
			{
				return (this._splitter);
			}
		}

		/// <summary>
		/// 	Gets or sets a value defining the order of the item in the error list.
		/// </summary>
		public ErrorListOrderPolicy ListOrderPolicy
		{
			get
			{
				return (this._stacktraceView.ListOrderPolicy);
			}
			set
			{
				this._stacktraceView.ListOrderPolicy = value;
			}
		}

		public ToolStripItem[] OptionItems
		{
			get
			{
				return (new ToolStripItem[] { this._btnSwap });
			}
		}

		public ToolStripButton PluginItem
		{
			get
			{
				return (this._btnPlugin);
			}
		}

		/// <summary>
		/// 	Gets or sets the splitter orientation in the SourceCodeDisplay.
		/// </summary>
		public Orientation SplitOrientation
		{
			get
			{
				return (this._splitter.Orientation);
			}
			set
			{
				this._splitter.Orientation = value;
			}
		}

		/// <summary>
		/// 	Gets or sets the splitter distance in the SourceCodeDisplay.
		/// </summary>
		public float SplitterDistance
		{
			get
			{
				return (this._splitter.SplitterDistance);
			}
			set
			{
				this._splitter.SplitterDistance = value;
			}
		}

		#endregion

		#region Methods/Operators

		private void OnClick(object sender, EventArgs e)
		{
			this.ListOrderPolicy = this.ListOrderPolicy == ErrorListOrderPolicy.InitialOrder ?
				                                                                                 ErrorListOrderPolicy.ReverseOrder :
					                                                                                                                   ErrorListOrderPolicy.InitialOrder;

			return;
		}

		public void OnStackTraceChanged(string stackTrace)
		{
			this._stacktraceView.StackTrace = stackTrace;
		}

		protected void SelectedItemChanged(object sender, EventArgs e)
		{
			ErrorItem item;
			IFormatterCatalog formatter;

			item = this._stacktraceView.SelectedItem;

			if (item == null)
			{
				this._codeView.Text = null;
				return;
			}

			formatter = this._codeView.Formatter;
			this._codeView.Language = formatter.LanguageFromExtension(item.FileExtension);

			try
			{
				this._codeView.Text = item.ReadFile();
			}
			catch (Exception ex)
			{
				this._codeView.Text = String.Format(
					"Cannot open file: '{0}'\r\nError: '{1}'\r\n",
					item.Path, ex.Message);
			}

			this._codeView.CurrentLine = item.LineNumber - 1;

			return;
		}

		private void _splitter_DistanceChanged(object sender, EventArgs e)
		{
			if (this.SplitterDistanceChanged != null)
				this.SplitterDistanceChanged(sender, e);
		}

		private void _splitter_OrientationChanged(object sender, EventArgs e)
		{
			if (this.SplitOrientationChanged != null)
				this.SplitOrientationChanged(sender, e);
		}

		#endregion
	}
}