// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using NUnit.UiException.Properties;

namespace NUnit.UiException.Controls
{
	/// <summary>
	/// 	A specialization of a ToolStrip to show instances of IErrorDisplay.
	/// </summary>
	public class ErrorToolbar :
		ToolStrip,
		IEnumerable
	{
		#region Constructors/Destructors

		public ErrorToolbar()
		{
			this._displays = new List<IErrorDisplay>();

			this._separator = this.CreateDefaultItem("-", null, null);
			this.Items.Add(this._separator);

			this._selection = -1;

			this.BackgroundImage = Resources.ImageErrorBrowserHeader;
			this.BackgroundImageLayout = ImageLayout.Tile;

			return;
		}

		#endregion

		#region Fields/Constants

		private List<IErrorDisplay> _displays;

		private int _selection;
		private ToolStripItem _separator;

		#endregion

		#region Properties/Indexers/Events

		public event EventHandler SelectedRendererChanged;

		/// <summary>
		/// 	Gets the display at the given index.
		/// </summary>
		public IErrorDisplay this[int index]
		{
			get
			{
				return (this._displays[index]);
			}
		}

		/// <summary>
		/// 	Gets the count of IErrorDisplay instances.
		/// </summary>
		public int Count
		{
			get
			{
				return (this._displays.Count);
			}
		}

		/// <summary>
		/// 	Gets or sets the IErrorDisplay to be selected.
		/// </summary>
		public IErrorDisplay SelectedDisplay
		{
			get
			{
				if (this._selection == -1)
					return (null);
				return ((IErrorDisplay)this.Items[this._selection].Tag);
			}
			set
			{
				int index = this.IndexOf(value);

				UiExceptionHelper.CheckFalse(index == -1 && value != null,
				                             "Cannot select unregistered display.", "SelectedDisplay");

				if (index == this._selection)
					return;

				this._selection = index;
				this.SetOrUnsetCheckedFlag(this._selection);
				this.ShowOrHideOptionItems(this._selection);

				if (this.SelectedRendererChanged != null)
					this.SelectedRendererChanged(this, new EventArgs());

				return;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Create and configure a ToolStripButton.
		/// </summary>
		public static ToolStripButton NewStripButton(
			bool canCheck, string text, Image image, EventHandler onClick)
		{
			ToolStripButton button;

			button = new ToolStripButton(text, image, onClick);
			button.CheckOnClick = canCheck;
			button.Image = image;
			button.ImageScaling = ToolStripItemImageScaling.None;
			button.TextImageRelation = TextImageRelation.ImageBeforeText;
			button.DisplayStyle = ToolStripItemDisplayStyle.Image;

			return (button);
		}

		/// <summary>
		/// 	Clears all IErrorDisplay in the toolbar.
		/// </summary>
		public void Clear()
		{
			this._displays.Clear();
			this.Items.Clear();
			this.Items.Add(this._separator);

			return;
		}

		public IEnumerator GetEnumerator()
		{
			return (this._displays.GetEnumerator());
		}

		private int IndexOf(IErrorDisplay renderer)
		{
			int i;

			if (renderer == null)
				return (-1);

			for (i = 0; i < this.Items.Count; ++i)
			{
				if (ReferenceEquals(this.Items[i].Tag, renderer))
					return (i);
			}

			return (-1);
		}

		/// <summary>
		/// 	Register a new IErrorDisplay in the toolbar.
		/// </summary>
		public void Register(IErrorDisplay display)
		{
			ToolStripItem item;
			int sepIndex;

			UiExceptionHelper.CheckNotNull(display, "display");
			UiExceptionHelper.CheckNotNull(display.PluginItem, "display.PluginItem");

			item = display.PluginItem;
			item.Tag = display;
			item.Click += new EventHandler(this.item_Click);

			this._displays.Add(display);
			sepIndex = this.Items.IndexOf(this._separator);
			this.Items.Insert(sepIndex, item);

			if (display.OptionItems != null)
			{
				ToolStripItem[] array = display.OptionItems;
				foreach (ToolStripItem value in array)
				{
					value.Visible = false;
					this.Items.Add(value);
				}
			}

			if (this._displays.Count == 1)
				this.SelectedDisplay = display;

			return;
		}

		private void SetOrUnsetCheckedFlag(int selectedIndex)
		{
			int index;

			foreach (IErrorDisplay item in this._displays)
			{
				index = this.IndexOf(item);
				if (index == -1)
					continue;
				item.PluginItem.Checked = (index == selectedIndex);
			}

			return;
		}

		private void ShowOrHideOptionItems(int selectedIndex)
		{
			int index;

			foreach (IErrorDisplay item in this._displays)
			{
				if ((index = this.IndexOf(item)) == -1)
					continue;

				if (item.OptionItems == null)
					continue;

				foreach (ToolStripItem stripItem in item.OptionItems)
					stripItem.Visible = (index == selectedIndex);
			}

			return;
		}

		private void item_Click(object sender, EventArgs e)
		{
			ToolStripItem item = sender as ToolStripItem;
			IErrorDisplay renderer;

			if (item == null || item.Tag == null)
				return;

			renderer = item.Tag as IErrorDisplay;
			if (renderer == null)
				return;

			this.SelectedDisplay = renderer;

			return;
		}

		#endregion
	}
}