/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using TextMetal.Common.Core;
using TextMetal.Common.WinForms.Forms;
using TextMetal.Common.Xml;
using TextMetal.Framework.Core;
using TextMetal.Framework.HostingModel;
using TextMetal.Framework.InputOutputModel;
using TextMetal.Framework.SourceModel.Primative;
using TextMetal.Framework.TemplateModel;

using Message = TextMetal.Common.Core.Message;

namespace TextMetal.HostImpl.WindowsTool.Forms
{
	public partial class DocumentForm : TmForm
	{
		#region Constructors/Destructors

		public DocumentForm()
		{
			this.InitializeComponent();
			this.CoreIsDirtyIndicator = '*';
		}

		#endregion

		#region Fields/Constants

		private TemplateConstruct document;
		private string documentFilePath;

		#endregion

		#region Properties/Indexers/Events

		private TemplateConstruct Document
		{
			get
			{
				return this.document;
			}
			set
			{
				this.document = value;
			}
		}

		public string DocumentFilePath
		{
			get
			{
				return this.documentFilePath;
			}
			set
			{
				this.documentFilePath = value;
			}
		}

		public string StatusText
		{
			get
			{
				return this.tsslMain.Text;
			}
			set
			{
				this.tsslMain.Text = value;
			}
		}

		#endregion

		#region Methods/Operators

		private void ApplyModelToView()
		{
		}

		private void ApplyViewToModel()
		{
		}

		protected override void CoreSetup()
		{
			base.CoreSetup();

			this.ApplyModelToView();
		}

		protected override void CoreShown()
		{
			DialogResult dialogResult;
			object asyncResult;
			bool asyncWasCanceled;
			Exception asyncExceptionOrNull;

			base.CoreShown();

			dialogResult = BackgroundTaskForm.Show(this, "Loading template...", o =>
																				{
																					Thread.Sleep(1000);
																					return this._LoadDocument();
																				}, null, out asyncWasCanceled, out asyncExceptionOrNull, out asyncResult);

			if (asyncWasCanceled || dialogResult == DialogResult.Cancel)
				this.Close(); // direct

			if ((object)asyncExceptionOrNull != null)
			{
				if (Program.Instance.HookUnhandledExceptionEvents)
					Program.Instance.ShowNestedExceptionsAndThrowBrickAtProcess(asyncExceptionOrNull);
				// should never reach this point
			}

			this.Document = (TemplateConstruct)asyncResult;

			if ((object)this.Document == null)
				throw new InvalidOperationException("TODO: add meaningful message");

			this.CoreText = string.Format("{0}", this.DocumentFilePath.SafeToString(null, "<new>"));
			this.StatusText = "Template load completed successfully.";

			this.ApplyModelToView();
		}

		public bool SaveDocument(bool asCopy)
		{
			Message[] messages;
			string filePath;

			if ((object)this.Document == null)
				throw new InvalidOperationException("TODO: add meaningful message");

			this.ApplyViewToModel();

			if (asCopy && !DataType.IsNullOrWhiteSpace(this.DocumentFilePath))
			{
				if (MessageBox.Show(this, "Do you want to save a copy of the current document?", Program.Instance.AssemblyInformation.Product, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
					return false;
			}

			messages = this._ValidateDocument();

			if ((object)messages == null)
				throw new InvalidOperationException("TODO: add meaningful message");

			if (messages.Length != 0)
			{
				using (MessageForm messageForm = new MessageForm()
												{
													Message = "",
													Messages = messages
												})
					messageForm.ShowDialog(this);

				return false;
			}

			if (asCopy)
			{
				// get new file path
				if (!this.TryGetFilePath(out filePath))
					return false;

				this.DocumentFilePath = filePath;
			}
			else
			{
				if (DataType.IsNullOrWhiteSpace(this.DocumentFilePath))
				{
					if (!this.TryGetFilePath(out filePath))
						return false;

					this.DocumentFilePath = filePath;
				}
			}

			// save document
			this._SaveDocument();

			this.CoreText = string.Format("{0}", this.DocumentFilePath.SafeToString(null, "<new>"));
			this.StatusText = "Template save completed successfully.";
			this.ApplyModelToView();

			return true;
		}

		private bool TryGetFilePath(out string filePath)
		{
			DialogResult dialogResult;

			this.sfdMain.FileName = filePath = null;
			dialogResult = this.sfdMain.ShowDialog(this);

			if (dialogResult != DialogResult.OK ||
				DataType.IsNullOrWhiteSpace(this.sfdMain.FileName))
				return false;

			filePath = Path.GetFullPath(this.sfdMain.FileName);
			return true;
		}

		public TemplateConstruct _LoadDocument()
		{
			if (DataType.IsNullOrWhiteSpace(this.DocumentFilePath))
				return new TemplateConstruct();
			else
			{
				IXmlPersistEngine xpe;
				ISourceStrategy sourceStrategy;
				IDictionary<string, IList<string>> properties;

				xpe = new XmlPersistEngine();
				xpe.RegisterWellKnownConstructs();

				sourceStrategy = new WellKnownXmlPersistEngineSourceStrategy();
				properties = new Dictionary<string, IList<string>>();

				using (IInputMechanism inputMechanism = new FileInputMechanism(Environment.CurrentDirectory, xpe, sourceStrategy))
					return (TemplateConstruct)inputMechanism.LoadSource(this.DocumentFilePath, properties);
			}
		}

		public void _SaveDocument()
		{
			if (DataType.IsNullOrWhiteSpace(this.DocumentFilePath))
				return;
			else
			{
				IXmlPersistEngine xpe;

				xpe = new XmlPersistEngine();
				xpe.RegisterWellKnownConstructs();

				using (IOutputMechanism outputMechanism = new FileOutputMechanism(Environment.CurrentDirectory, "", xpe))
					outputMechanism.WriteObject(this.Document, this.DocumentFilePath);
			}
		}

		public object _UpdateDocumentProps()
		{
			throw new NotImplementedException();
		}

		public void _UpdateDocumentTree(TreeView tvDocument)
		{
			throw new NotImplementedException();
		}

		public Message[] _ValidateDocument()
		{
			return new Message[] { };
		}

		private void tsmiClose_Click(object sender, EventArgs e)
		{
			this.Close(); // direct
		}

		private void tsmiSaveAs_Click(object sender, EventArgs e)
		{
			this.SaveDocument(true);
		}

		private void tsmiSave_Click(object sender, EventArgs e)
		{
			this.SaveDocument(false);
		}

		#endregion
	}
}