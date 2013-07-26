// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Windows.Forms;

namespace NUnit.UiException.Controls
{
	/// <summary>
	/// 	Provides the panels and layout of ErrorBrowser as
	/// 	shown below:
	/// 
	/// 	+--------------------------------------------+
	/// 	|                  Toolbar                   |
	/// 	+--------------------------------------------+
	/// 	|                                            |
	/// 	|                                            |
	/// 	|                  Content                   |
	/// 	|                                            |
	/// 	|                                            |
	/// 	+--------------------------------------------+
	/// 
	/// 	Toolbar: the control which shows buttons
	/// 	to alternate between the StackTraceDisplay
	/// 	and BrowserDisplay back and forth.
	/// 	The control collection of this control
	/// 	never changes.
	///               
	/// 	Option:  a free place holder to show subfeature
	/// 	for a specific display (e.g: StackTraceDisplay
	/// 	or BrowserDisplay). This control's
	/// 	collection changes in relation with the
	/// 	selected display.
	///               
	/// 	Content: the place where to put the main content
	/// 	for the current display. This control's 
	/// 	collection changes in regard of the
	/// 	selected display.
	/// </summary>
	public class ErrorPanelLayout :
		UserControl
	{
		#region Constructors/Destructors

		public ErrorPanelLayout()
		{
			this._header = new InternalSplitter();
			this._contentDefault = new Panel();
			this._contentCurrent = this._contentDefault;

			this.Controls.Add(this._header[PANEL_LEFT]);
			//Controls.Add(_header[PANEL_RIGHT]);
			this.Controls.Add(this._contentDefault);

			//_header[PANEL_LEFT].BackColor = Color.Yellow;
			//_header[PANEL_RIGHT].BackColor = Color.Violet;
			//_contentDefault.BackColor = Color.Green;

			this.SizeChanged += new EventHandler(this.ErrorPanelLayout_SizeChanged);

			this._header[PANEL_RIGHT].ControlAdded += new ControlEventHandler(this.ErrorPanelLayout_ControlAddedOrRemoved);

			this._header[PANEL_RIGHT].ControlRemoved += new ControlEventHandler(this.ErrorPanelLayout_ControlAddedOrRemoved);

			this.Width = 200;
			this.Height = 200;

			return;
		}

		#endregion

		#region Fields/Constants

		private static readonly int PANEL_LEFT = 0;
		private static readonly int PANEL_RIGHT = 1;
		public static readonly int TOOLBAR_HEIGHT = 26;

		private Control _contentCurrent;
		private Panel _contentDefault;
		private InternalSplitter _header;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Gets or sets the control to be placed in Content location.
		/// 	Pass null to reset content to its default state.
		/// 
		/// 	When setting a control, the control's hierarchy of
		/// 	ErrorPanelLayout is automatically updated with the
		/// 	passed component. Besides, the passed component is
		/// 	automatically positionned to the right coordinates.
		/// </summary>
		public Control Content
		{
			get
			{
				return (this._contentCurrent);
			}
			set
			{
				if (value == null)
					value = this._contentDefault;
				this.Controls.Remove(this._contentCurrent);
				this._contentCurrent = value;
				this.Controls.Add(this._contentCurrent);
				this.doLayout();
			}
		}

		/// <summary>
		/// 	Gets or sets the control to be placed in Option location.
		/// 	Pass null to reset Option to its default state.
		/// 
		/// 	When setting a control, the control's hierarchy of
		/// 	ErrorPanelLayout is automatically updated with the
		/// 	passed component. Besides, the passed component is
		/// 	automatically positionned to the right coordinates.
		/// </summary>
		public Control Option
		{
			get
			{
				return (this._header[PANEL_RIGHT]);
			}
			set
			{
				this.Controls.Remove(this._header[PANEL_RIGHT]);
				this._header[PANEL_RIGHT] = value;
				this.Controls.Add(this._header[PANEL_RIGHT]);
				this.doLayout();
			}
		}

		/// <summary>
		/// 	Gets or sets the control to be placed in Toolbar location.
		/// 	Pass null to reset Toolbar to its default state.
		/// 
		/// 	When setting a control, the control's hierarchy of
		/// 	ErrorPanelLayout is automatically updated with the
		/// 	passed component. Besides, the passed component is
		/// 	automatically positionned to the right coordinates.
		/// </summary>
		public Control Toolbar
		{
			get
			{
				return (this._header[PANEL_LEFT]);
			}
			set
			{
				this.Controls.Remove(this._header[PANEL_LEFT]);
				this._header[PANEL_LEFT] = value;
				this.Controls.Add(this._header[PANEL_LEFT]);
				this.doLayout();
			}
		}

		#endregion

		#region Methods/Operators

		private void ErrorPanelLayout_ControlAddedOrRemoved(object sender, ControlEventArgs e)
		{
			this.doLayout();
		}

		private void ErrorPanelLayout_SizeChanged(object sender, EventArgs e)
		{
			this.doLayout();
		}

		//protected override void OnPaint(PaintEventArgs e)
		//{
		//    e.Graphics.DrawImage(Resources.ErrorBrowserHeader,
		//        new Rectangle(0, 0, Width, TOOLBAR_HEIGHT),
		//        new Rectangle(0, 0, Resources.ErrorBrowserHeader.Width - 1,
		//            Resources.ErrorBrowserHeader.Height),
		//        GraphicsUnit.Pixel);

		//    return;
		//}

		private void doLayout()
		{
//            int widthLeft;
			int widthRight;

			widthRight = this._header.WidthAt(PANEL_RIGHT);
//            widthLeft = _header.WidthAt(PANEL_LEFT);

			this._header[PANEL_LEFT].Width = Math.Max(0, this.Width - widthRight);
			this._contentCurrent.Width = this.Width;

			this._header[PANEL_LEFT].Height = TOOLBAR_HEIGHT;
			this._header[PANEL_RIGHT].Height = Math.Min(TOOLBAR_HEIGHT, this._header[PANEL_RIGHT].Height);
			this._header[PANEL_RIGHT].Width = widthRight;
			this._header[PANEL_RIGHT].Left = this._header[PANEL_LEFT].Width;

			this._contentCurrent.Height = this.Height - TOOLBAR_HEIGHT;
			this._contentCurrent.Top = TOOLBAR_HEIGHT;

			return;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class InternalSplitter : UserControl
		{
			#region Constructors/Destructors

			public InternalSplitter()
			{
				this._panels = new Panel[] { new Panel(), new Panel() };
				this._currents = new Control[] { this._panels[0], this._panels[1] };

				this._panels[0].Width = 0;
				this._panels[1].Width = 0;

				return;
			}

			#endregion

			#region Fields/Constants

			private Control[] _currents;
			private Panel[] _panels;

			#endregion

			#region Properties/Indexers/Events

			public Control this[int index]
			{
				get
				{
					return (this._currents[index]);
				}
				set
				{
					if (value == null)
						value = this._panels[index];
					this._currents[index] = value;
				}
			}

			#endregion

			#region Methods/Operators

			public int WidthAt(int index)
			{
				if (this._currents[index] == this._panels[index])
					return (0);
				return (this._currents[index].Width);
			}

			#endregion
		}

		#endregion
	}
}