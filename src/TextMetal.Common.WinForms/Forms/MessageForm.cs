/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using TextMetal.Common.WinForms.Controls;

using Message = TextMetal.Common.Core.Message;

namespace TextMetal.Common.WinForms.Forms
{
	public partial class MessageForm : TmForm
	{
		#region Constructors/Destructors

		public MessageForm()
		{
			this.InitializeComponent();
		}

		#endregion

		#region Fields/Constants

		private IEnumerable<Message> messages;

		#endregion

		#region Properties/Indexers/Events

		public string Message
		{
			get
			{
				return this.lblMessage.CoreGetValue();
			}
			set
			{
				this.lblMessage.CoreSetValue(value);
			}
		}

		public IEnumerable<Message> Messages
		{
			get
			{
				return this.messages;
			}
			set
			{
				this.messages = value;
			}
		}

		#endregion

		#region Methods/Operators

		private void Cancel()
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close(); // direct
		}

		protected override void CoreSetup()
		{
			base.CoreSetup();

			this.RefreshMessages();
		}

		private void Okay()
		{
			this.DialogResult = DialogResult.OK;
			this.Close(); // direct
		}

		private void RefreshMessages()
		{
			TreeNode tnCategory;
			TreeNode tnMessage;

			if ((object)this.messages != null)
			{
				var categories = this.messages.Select(m => (m.Category ?? string.Empty).Trim()).Distinct();

				foreach (string category in categories)
				{
					string _category = category; // prevent closure issue

					if ((category ?? string.Empty).Trim() != string.Empty)
					{
						tnCategory = new TreeNode(category, 0, 0);
						this.tvMessages.Nodes.Add(tnCategory);
					}
					else
						tnCategory = null;

					foreach (Message message in this.messages.Where(m => (m.Category ?? string.Empty).Trim() == _category))
					{
						tnMessage = new TreeNode(message.Description, (int)message.Severity, (int)message.Severity);

						if ((object)tnCategory != null)
							tnCategory.Nodes.Add(tnMessage);
						else
							this.tvMessages.Nodes.Add(tnMessage);
					}
				}
			}

			this.tvMessages.ExpandAll();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Cancel();
		}

		private void btnOkay_Click(object sender, EventArgs e)
		{
			this.Okay();
		}

		#endregion
	}
}