/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using TextMetal.Common.Core;

namespace TextMetal.Common.WinForms.Forms
{
	public partial class TmForm : Form
	{
		#region Constructors/Destructors

		public TmForm()
		{
			this.InitializeComponent();

			this.AssertExecutionContext();
		}

		#endregion

		#region Fields/Constants

		private bool coreHasShown;
		private bool coreIsDirty;
		private char? coreIsDirtyIndicator;
		private string coreText;

		#endregion

		#region Properties/Indexers/Events

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected bool CoreHasShow
		{
			get
			{
				return this.coreHasShown;
			}
			private set
			{
				if (this.coreHasShown || !value)
					throw new InvalidOperationException(string.Format("CoreHasShow property cannot be set more than once and cannot be set to false at anytime."));

				this.coreHasShown = true;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CoreIsDirty
		{
			get
			{
				return this.coreIsDirty;
			}
			set
			{
				this.coreIsDirty = value;

				this.CoreUpdateFormText();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public char? CoreIsDirtyIndicator
		{
			get
			{
				return this.coreIsDirtyIndicator;
			}
			set
			{
				this.coreIsDirtyIndicator = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string CoreText
		{
			get
			{
				return this.coreText;
			}
			set
			{
				this.coreText = value;

				this.CoreUpdateFormText();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected TmForm CoreOwnerForm
		{
			get
			{
				return (TmForm)this.Owner;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected TmForm CoreParentForm
		{
			get
			{
				return (TmForm)this.ParentForm;
			}
		}

		#endregion

		#region Methods/Operators

		protected void AssertExecutionContext()
		{
			//if ((object)ExecutableApplication.Current == null)
			//throw new InvalidOperationException(string.Format("No executable application context exists on the current thread and application domain."));
		}

		protected virtual void CoreQuit(out bool cancel)
		{
			cancel = false;
			// do nothing
		}

		public void CoreSetToolTipText(Control control, string caption)
		{
			this.ttMain.SetToolTip(control, caption);
		}

		protected virtual void CoreSetup()
		{
			Stream stream;
			Icon icon;

			if ((object)this.CoreOwnerForm != null)
				this.CoreText = string.Format("{0}", this.CoreOwnerForm.CoreText.SafeToString());

			stream = this.GetType().Assembly.GetManifestResourceStream("TextMetal.HostImpl.WindowsTool.Icons.TextMetal.ico");

			if ((object)stream == null)
				return;

			icon = new Icon(stream);
			this.Icon = icon;
			// DO NOT DISPOSE (owner cleans up)
		}

		protected virtual void CoreShown()
		{
			this.AssertExecutionContext();

			this.CoreHasShow = true;
			this.CoreIsDirty = false;
		}

		protected virtual void CoreTeardown()
		{
			// do nothing
		}

		private void CoreUpdateFormText()
		{
			this.Text = string.Format("{0}{1}", this.CoreText.SafeToString(), this.CoreIsDirty ? this.CoreIsDirtyIndicator.SafeToString() : string.Empty);
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			this.CoreTeardown();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			bool cancel;

			base.OnClosing(e);

			if (e.Cancel)
				return;

			this.CoreQuit(out cancel);

			e.Cancel = cancel;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.CoreSetup();
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			this.CoreShown();
		}

		#endregion
	}
}