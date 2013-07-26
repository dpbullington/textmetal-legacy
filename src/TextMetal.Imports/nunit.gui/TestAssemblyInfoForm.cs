// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using NUnit.Core;
using NUnit.UiKit;
using NUnit.Util;

namespace NUnit.Gui
{
	public class TestAssemblyInfoForm : ScrollingTextDisplayForm
	{
		#region Methods/Operators

		private void AppendAssemblyInfo(TestAssemblyInfo info)
		{
			this.AppendBoldText(
				string.Format("    {0}\r\n", Path.GetFileNameWithoutExtension(info.Name)));

			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("      Path: {0}\r\n", info.Name);
			sb.AppendFormat("      Image Runtime Version: {0}\r\n", info.ImageRuntimeVersion.ToString());

			if (info.TestFrameworks != null)
			{
				string prefix = "      Uses: ";
				foreach (AssemblyName framework in info.TestFrameworks)
				{
					sb.AppendFormat("{0}{1}\r\n", prefix, framework.FullName);
					prefix = "            ";
				}
			}

			this.TextBox.AppendText(sb.ToString());
		}

		private void AppendBoldText(string text)
		{
			this.TextBox.Select(this.TextBox.Text.Length, 0);
			this.TextBox.SelectionFont = new Font(this.TextBox.Font, FontStyle.Bold);

			this.TextBox.SelectedText += text;
		}

		private void AppendDomainInfo(TestAssemblyInfo info)
		{
			this.AppendBoldText(string.Format("\r\n  {0}\r\n", info.DomainName));

			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("    ApplicationBase: {0}\r\n", info.ApplicationBase);

			if (info.PrivateBinPath != null)
			{
				string prefix = "    PrivateBinPath: ";
				foreach (string s in info.PrivateBinPath.Split(new char[] { ';' }))
				{
					sb.AppendFormat("{0}{1}\r\n", prefix, s);
					prefix = "                    ";
				}
			}

			sb.AppendFormat("    Configuration File: {0}\r\n", info.ConfigurationFile);

			this.TextBox.AppendText(sb.ToString());
		}

		private void AppendProcessInfo(TestAssemblyInfo info)
		{
			this.AppendProcessInfo(info.ProcessId, info.ModuleName, info.RunnerRuntimeFramework);
		}

		private void AppendProcessInfo(int pid, string moduleName, RuntimeFramework framework)
		{
			this.AppendBoldText(string.Format("{0} ( {1} )\r\n", moduleName, pid));

			this.TextBox.AppendText(string.Format(
				"  Framework Version: {0}\r\n",
				framework.DisplayName));

			this.TextBox.AppendText(string.Format(
				"  CLR Version: {0}\r\n",
				framework.ClrVersion.ToString()));
		}

		protected override void OnLoad(EventArgs e)
		{
			this.Text = "Test Assemblies";
			this.TextBox.WordWrap = false;
			//this.TextBox.ContentsResized += new ContentsResizedEventHandler(TextBox_ContentsResized);
			this.TextBox.Font = new Font(FontFamily.GenericMonospace, 8.25F);

			base.OnLoad(e);

			Process p = Process.GetCurrentProcess();
			int currentProcessId = p.Id;
			string currentDomainName = "";

			this.AppendProcessInfo(
				currentProcessId,
				Path.GetFileName(Assembly.GetEntryAssembly().Location),
				RuntimeFramework.CurrentFramework);

			foreach (TestAssemblyInfo info in Services.TestLoader.AssemblyInfo)
			{
				if (info.ProcessId != currentProcessId)
				{
					this.TextBox.AppendText("\r\n");
					this.AppendProcessInfo(info);
					currentProcessId = info.ProcessId;
				}

				if (info.DomainName != currentDomainName)
				{
					this.AppendDomainInfo(info);
					currentDomainName = info.DomainName;
				}

				this.AppendAssemblyInfo(info);
			}

			this.TextBox.Select(0, 0);
			this.TextBox.ScrollToCaret();
		}

		private void TextBox_ContentsResized(object sender, ContentsResizedEventArgs e)
		{
			int increase = e.NewRectangle.Width - this.TextBox.ClientSize.Width;
			if (increase > 0)
			{
				this.TextBox.Width += increase;
				this.Width += increase;
			}
		}

		#endregion
	}
}