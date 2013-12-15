// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.Windows.Forms;

using NUnit.Core;
using NUnit.Util;

namespace NUnit.Gui
{
	using System;

	/// <summary>
	/// Summary description for DetailResults
	/// </summary>
	public class DetailResults
	{
		#region Constructors/Destructors

		public DetailResults(ListBox listBox, TreeView notRun)
		{
			this.testDetails = listBox;
			this.notRunTree = notRun;
		}

		#endregion

		#region Fields/Constants

		private readonly TreeView notRunTree;
		private readonly ListBox testDetails;

		#endregion

		#region Methods/Operators

		private static TreeNode MakeNotRunNode(TestResult result)
		{
			TreeNode node = new TreeNode(result.Name);

			TreeNode reasonNode = new TreeNode("Reason: " + result.Message);

			node.Nodes.Add(reasonNode);

			return node;
		}

		public void DisplayResults(TestResult results)
		{
			this.notRunTree.BeginUpdate();
			this.ProcessResults(results);
			this.notRunTree.EndUpdate();

			if (this.testDetails.Items.Count > 0)
				this.testDetails.SelectedIndex = 0;
		}

		private void ProcessResults(TestResult result)
		{
			switch (result.ResultState)
			{
				case ResultState.Failure:
				case ResultState.Error:
				case ResultState.Cancelled:
					TestResultItem item = new TestResultItem(result);
					//string resultString = String.Format("{0}:{1}", result.Name, result.Message);
					this.testDetails.BeginUpdate();
					this.testDetails.Items.Insert(this.testDetails.Items.Count, item);
					this.testDetails.EndUpdate();
					break;
				case ResultState.Skipped:
				case ResultState.NotRunnable:
				case ResultState.Ignored:
					this.notRunTree.Nodes.Add(MakeNotRunNode(result));
					break;
			}

			if (result.HasResults)
			{
				foreach (TestResult childResult in result.Results)
					this.ProcessResults(childResult);
			}
		}

		#endregion
	}
}