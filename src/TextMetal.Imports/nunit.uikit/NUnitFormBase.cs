// ****************************************************************
// Copyright 2011, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Windows.Forms;

namespace NUnit.UiKit
{
	public class NUnitFormBase : Form
	{
		#region Constructors/Destructors

		public NUnitFormBase()
		{
		}

		public NUnitFormBase(string caption)
		{
			this.caption = caption;
		}

		#endregion

		#region Fields/Constants

		private string caption;
		private IMessageDisplay messageDisplay;

		#endregion

		#region Properties/Indexers/Events

		public IMessageDisplay MessageDisplay
		{
			get
			{
				if (this.messageDisplay == null)
					this.messageDisplay = new MessageDisplay(this.caption == null ? this.Text : this.caption);

				return this.messageDisplay;
			}
		}

		#endregion
	}
}