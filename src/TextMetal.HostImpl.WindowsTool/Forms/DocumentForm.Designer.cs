using TextMetal.Common.WinForms.Controls;

namespace TextMetal.HostImpl.WindowsTool.Forms
{
	partial class DocumentForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentForm));
			this.msMain = new TmMenuStrip();
			this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiSaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiClose = new System.Windows.Forms.ToolStripMenuItem();
			this.ssMain = new TmStatusStrip();
			this.tsslMain = new System.Windows.Forms.ToolStripStatusLabel();
			this.pnlMain = new TextMetal.Common.WinForms.Controls.TmPanel();
			this.tmSplitContainer1 = new TextMetal.Common.WinForms.Controls.TmSplitContainer();
			this.tvMain = new TextMetal.Common.WinForms.Controls.TmTreeView();
			this.tmSplitContainer2 = new TextMetal.Common.WinForms.Controls.TmSplitContainer();
			this.txtBxSourceView = new TextMetal.Common.WinForms.Controls.TmTextBox();
			this.pgMain = new TextMetal.Common.WinForms.Controls.TmPropertyGrid();
			this.sfdMain = new System.Windows.Forms.SaveFileDialog();
			this.tabMain = new TextMetal.Common.WinForms.Controls.TmTabControl();
			this.tpgSourceView = new TmTabPage();
			this.tpgDesignView = new TmTabPage();
			this.sketchDesigner = new TextMetal.Common.WinForms.DesignTime.SketchDesigner();
			this.msMain.SuspendLayout();
			this.ssMain.SuspendLayout();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tmSplitContainer1)).BeginInit();
			this.tmSplitContainer1.Panel1.SuspendLayout();
			this.tmSplitContainer1.Panel2.SuspendLayout();
			this.tmSplitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tmSplitContainer2)).BeginInit();
			this.tmSplitContainer2.Panel1.SuspendLayout();
			this.tmSplitContainer2.Panel2.SuspendLayout();
			this.tmSplitContainer2.SuspendLayout();
			this.tabMain.SuspendLayout();
			this.tpgSourceView.SuspendLayout();
			this.tpgDesignView.SuspendLayout();
			this.SuspendLayout();
			// 
			// msMain
			// 
			this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile});
			this.msMain.Location = new System.Drawing.Point(0, 0);
			this.msMain.Name = "msMain";
			this.msMain.Size = new System.Drawing.Size(883, 24);
			this.msMain.TabIndex = 0;
			// 
			// tsmiFile
			// 
			this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSave,
            this.tsmiSaveAs,
            this.toolStripSeparator3,
            this.tsmiClose});
			this.tsmiFile.Name = "tsmiFile";
			this.tsmiFile.Size = new System.Drawing.Size(37, 20);
			this.tsmiFile.Text = "&File";
			// 
			// tsmiSave
			// 
			this.tsmiSave.Name = "tsmiSave";
			this.tsmiSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.tsmiSave.Size = new System.Drawing.Size(195, 22);
			this.tsmiSave.Text = "&Save";
			this.tsmiSave.Click += new System.EventHandler(this.tsmiSave_Click);
			// 
			// tsmiSaveAs
			// 
			this.tsmiSaveAs.Name = "tsmiSaveAs";
			this.tsmiSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
			this.tsmiSaveAs.Size = new System.Drawing.Size(195, 22);
			this.tsmiSaveAs.Text = "Save &As...";
			this.tsmiSaveAs.Click += new System.EventHandler(this.tsmiSaveAs_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(192, 6);
			// 
			// tsmiClose
			// 
			this.tsmiClose.Name = "tsmiClose";
			this.tsmiClose.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
			this.tsmiClose.Size = new System.Drawing.Size(195, 22);
			this.tsmiClose.Text = "&Close";
			this.tsmiClose.Click += new System.EventHandler(this.tsmiClose_Click);
			// 
			// ssMain
			// 
			this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslMain});
			this.ssMain.Location = new System.Drawing.Point(0, 411);
			this.ssMain.Name = "ssMain";
			this.ssMain.Size = new System.Drawing.Size(883, 22);
			this.ssMain.TabIndex = 2;
			this.ssMain.Text = "statusStrip1";
			// 
			// tsslMain
			// 
			this.tsslMain.Name = "tsslMain";
			this.tsslMain.Size = new System.Drawing.Size(54, 17);
			this.tsslMain.Text = "%TEXT%";
			// 
			// pnlMain
			// 
			this.pnlMain.AutoScroll = true;
			this.pnlMain.Controls.Add(this.tmSplitContainer1);
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMain.Location = new System.Drawing.Point(0, 24);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Size = new System.Drawing.Size(883, 387);
			this.pnlMain.TabIndex = 1;
			// 
			// tmSplitContainer1
			// 
			this.tmSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tmSplitContainer1.Location = new System.Drawing.Point(0, 0);
			this.tmSplitContainer1.Name = "tmSplitContainer1";
			// 
			// tmSplitContainer1.Panel1
			// 
			this.tmSplitContainer1.Panel1.Controls.Add(this.tvMain);
			// 
			// tmSplitContainer1.Panel2
			// 
			this.tmSplitContainer1.Panel2.Controls.Add(this.tmSplitContainer2);
			this.tmSplitContainer1.Size = new System.Drawing.Size(883, 387);
			this.tmSplitContainer1.SplitterDistance = 170;
			this.tmSplitContainer1.TabIndex = 0;
			// 
			// tvMain
			// 
			this.tvMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvMain.Location = new System.Drawing.Point(0, 0);
			this.tvMain.Name = "tvMain";
			this.tvMain.Size = new System.Drawing.Size(170, 387);
			this.tvMain.TabIndex = 0;
			this.tvMain.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvMain_AfterSelect);
			// 
			// tmSplitContainer2
			// 
			this.tmSplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tmSplitContainer2.Location = new System.Drawing.Point(0, 0);
			this.tmSplitContainer2.Name = "tmSplitContainer2";
			// 
			// tmSplitContainer2.Panel1
			// 
			this.tmSplitContainer2.Panel1.Controls.Add(this.tabMain);
			// 
			// tmSplitContainer2.Panel2
			// 
			this.tmSplitContainer2.Panel2.Controls.Add(this.pgMain);
			this.tmSplitContainer2.Size = new System.Drawing.Size(709, 387);
			this.tmSplitContainer2.SplitterDistance = 520;
			this.tmSplitContainer2.TabIndex = 0;
			// 
			// txtBxSourceView
			// 
			this.txtBxSourceView.AcceptsReturn = true;
			this.txtBxSourceView.AcceptsTab = true;
			this.txtBxSourceView.BackColor = System.Drawing.SystemColors.Control;
			this.txtBxSourceView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtBxSourceView.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtBxSourceView.HideSelection = false;
			this.txtBxSourceView.Location = new System.Drawing.Point(3, 3);
			this.txtBxSourceView.Multiline = true;
			this.txtBxSourceView.Name = "txtBxSourceView";
			this.txtBxSourceView.ReadOnly = true;
			this.txtBxSourceView.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtBxSourceView.Size = new System.Drawing.Size(506, 355);
			this.txtBxSourceView.TabIndex = 0;
			this.txtBxSourceView.ValueType = null;
			this.txtBxSourceView.WordWrap = false;
			// 
			// pgMain
			// 
			this.pgMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgMain.Location = new System.Drawing.Point(0, 0);
			this.pgMain.Name = "pgMain";
			this.pgMain.Size = new System.Drawing.Size(185, 387);
			this.pgMain.TabIndex = 0;
			this.pgMain.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgMain_PropertyValueChanged);
			// 
			// sfdMain
			// 
			this.sfdMain.Filter = "All files|*.*";
			this.sfdMain.RestoreDirectory = true;
			// 
			// tabMain
			// 
			this.tabMain.Alignment = System.Windows.Forms.TabAlignment.Bottom;
			this.tabMain.Controls.Add(this.tpgDesignView);
			this.tabMain.Controls.Add(this.tpgSourceView);
			this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabMain.Location = new System.Drawing.Point(0, 0);
			this.tabMain.Name = "tabMain";
			this.tabMain.SelectedIndex = 0;
			this.tabMain.Size = new System.Drawing.Size(520, 387);
			this.tabMain.TabIndex = 1;
			// 
			// tpgSourceView
			// 
			this.tpgSourceView.Controls.Add(this.txtBxSourceView);
			this.tpgSourceView.Location = new System.Drawing.Point(4, 4);
			this.tpgSourceView.Name = "tpgSourceView";
			this.tpgSourceView.Padding = new System.Windows.Forms.Padding(3);
			this.tpgSourceView.Size = new System.Drawing.Size(512, 361);
			this.tpgSourceView.TabIndex = 0;
			this.tpgSourceView.Text = "Source";
			this.tpgSourceView.UseVisualStyleBackColor = true;
			// 
			// tpgDesignView
			// 
			this.tpgDesignView.Controls.Add(this.sketchDesigner);
			this.tpgDesignView.Location = new System.Drawing.Point(4, 4);
			this.tpgDesignView.Name = "tpgDesignView";
			this.tpgDesignView.Padding = new System.Windows.Forms.Padding(3);
			this.tpgDesignView.Size = new System.Drawing.Size(512, 361);
			this.tpgDesignView.TabIndex = 1;
			this.tpgDesignView.Text = "Design";
			this.tpgDesignView.UseVisualStyleBackColor = true;
			// 
			// sketchDesigner
			// 
			this.sketchDesigner.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sketchDesigner.Location = new System.Drawing.Point(3, 3);
			this.sketchDesigner.Name = "sketchDesigner";
			this.sketchDesigner.Size = new System.Drawing.Size(506, 355);
			this.sketchDesigner.TabIndex = 3;
			// 
			// DocumentForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(883, 433);
			this.Controls.Add(this.pnlMain);
			this.Controls.Add(this.msMain);
			this.Controls.Add(this.ssMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.msMain;
			this.Name = "DocumentForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.msMain.ResumeLayout(false);
			this.msMain.PerformLayout();
			this.ssMain.ResumeLayout(false);
			this.ssMain.PerformLayout();
			this.pnlMain.ResumeLayout(false);
			this.tmSplitContainer1.Panel1.ResumeLayout(false);
			this.tmSplitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.tmSplitContainer1)).EndInit();
			this.tmSplitContainer1.ResumeLayout(false);
			this.tmSplitContainer2.Panel1.ResumeLayout(false);
			this.tmSplitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.tmSplitContainer2)).EndInit();
			this.tmSplitContainer2.ResumeLayout(false);
			this.tabMain.ResumeLayout(false);
			this.tpgSourceView.ResumeLayout(false);
			this.tpgSourceView.PerformLayout();
			this.tpgDesignView.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private TmMenuStrip msMain;
		private TmStatusStrip ssMain;
		private TmPanel pnlMain;
		private System.Windows.Forms.ToolStripMenuItem tsmiFile;
		private System.Windows.Forms.ToolStripMenuItem tsmiSave;
		private System.Windows.Forms.ToolStripMenuItem tsmiSaveAs;
		private System.Windows.Forms.ToolStripMenuItem tsmiClose;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.SaveFileDialog sfdMain;
		private System.Windows.Forms.ToolStripStatusLabel tsslMain;
		private TmSplitContainer tmSplitContainer1;
		private TmTreeView tvMain;
		private TmSplitContainer tmSplitContainer2;
		private TmPropertyGrid pgMain;
		private TmTextBox txtBxSourceView;
		private TmTabControl tabMain;
		private TmTabPage tpgSourceView;
		private TmTabPage tpgDesignView;
		private Common.WinForms.DesignTime.SketchDesigner sketchDesigner;

	}
}