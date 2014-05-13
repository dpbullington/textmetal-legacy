/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using WebListItem = System.Web.UI.WebControls.ListItem;

using TextMetal.HostImpl.AspNetSample.Common;

using TextMetal.Common.Core;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Shared
{
	public static class ControlExtensionMethods
	{
		#region Methods/Operators

		public static void CoreBindSelectionItems<TValue>(this DropDownList dropDownList, IListItem<TValue>[] listItems, bool useZerothItemIndex)
		{
			List<WebListItem> webListItems;

			if ((object)dropDownList == null)
				throw new ArgumentNullException("dropDownList");

			webListItems = new List<WebListItem>();

			if ((object)listItems != null)
			{
				foreach (IListItem<TValue> listItem in listItems)
					webListItems.Add(new WebListItem(listItem.Text, listItem.Value.SafeToString()));
			}

			if (useZerothItemIndex)
				webListItems.Insert(0, new WebListItem("-- SELECT --", ""));

			dropDownList.Items.Clear();
			dropDownList.Items.AddRange(webListItems.ToArray());
			dropDownList.SelectedIndex = useZerothItemIndex ? 0 : -1;
		}

		public static bool? CoreGetCheckedValue(this CheckBox checkBox)
		{
			if ((object)checkBox == null)
				throw new ArgumentNullException("checkBox");

			return checkBox.Checked;
		}

		public static string CoreGetSelectedText(this DropDownList dropDownList, bool useZerothItemIndex)
		{
			if ((object)dropDownList == null)
				throw new ArgumentNullException("dropDownList");

			return dropDownList.CoreGetValue();
		}

		public static TValue CoreGetSelectedValue<TValue>(this DropDownList dropDownList)
		{
			WebListItem webListItem;
			TValue value;

			if ((object)dropDownList == null)
				throw new ArgumentNullException("dropDownList");

			if (dropDownList.SelectedIndex == -1)
				return default(TValue);

			webListItem = dropDownList.SelectedItem;

			if ((object)webListItem == null)
				return default(TValue);

			if (!DataType.TryParse<TValue>(webListItem.Value, out value))
				return default(TValue);

			return value;
		}

		public static string CoreGetValue(this ITextControl textControl)
		{
			if ((object)textControl == null)
				throw new ArgumentNullException("textControl");

			return textControl.Text;
		}

		public static TValue CoreGetValue<TValue>(this ITextControl textControl)
		{
			TValue value;

			if ((object)textControl == null)
				throw new ArgumentNullException("textControl");

			if (DataType.TryParse<TValue>(textControl.Text, out value))
				return value;
			else
				return default(TValue);
		}

		public static object CoreGetValue(this ITextControl textControl, Type targetType)
		{
			object value;

			if ((object)textControl == null)
				throw new ArgumentNullException("textControl");

			if ((object)targetType == null)
				throw new ArgumentNullException("targetType");

			if (DataType.TryParse(targetType, textControl.Text, out value))
				return value;
			else
				return DataType.DefaultValue(targetType);
		}

		public static bool CoreIsEmpty(this ITextControl textControl)
		{
			string value;

			if ((object)textControl == null)
				throw new ArgumentNullException("textControl");

			value = CoreGetValue(textControl);

			return DataType.IsNullOrWhiteSpace(value);
		}

		public static bool CoreIsValid<TValue>(this ITextControl textControl)
		{
			TValue value;

			if ((object)textControl == null)
				throw new ArgumentNullException("textControl");

			return DataType.TryParse<TValue>(textControl.Text, out value);
		}

		public static bool CoreIsValid(this ITextControl textControl, Type targetType)
		{
			object value;

			if ((object)textControl == null)
				throw new ArgumentNullException("textControl");

			if ((object)targetType == null)
				throw new ArgumentNullException("targetType");

			return DataType.TryParse(targetType, textControl.Text, out value);
		}

		public static void CoreSetCheckedValue(this CheckBox checkBox, bool? value)
		{
			if ((object)checkBox == null)
				throw new ArgumentNullException("checkBox");

			checkBox.Checked = (value ?? false);
		}

		public static void CoreSetSelectedText(this DropDownList dropDownList, string value, bool useZerothItemIndex)
		{
			WebListItem webListItem;

			if ((object)dropDownList == null)
				throw new ArgumentNullException("dropDownList");

			webListItem = dropDownList.Items.FindByText(value);

			if ((object)webListItem != null)
				webListItem.Selected = true;
			else
				dropDownList.CoreSetValue(value); // ???
		}

		public static void CoreSetSelectedValue<TValue>(this DropDownList dropDownList, TValue value, bool useZerothItemIndex)
		{
			int index;

			if ((object)dropDownList == null)
				throw new ArgumentNullException("dropDownList");

			if ((object)value == (object)default(TValue))
			{
				dropDownList.SelectedIndex = useZerothItemIndex ? 0 : -1;
				return;
			}

			index = 0;
			foreach (IListItem<TValue> item in dropDownList.Items)
			{
				if (value.Equals(item.Value))
				{
					dropDownList.SelectedIndex = index;
					break;
				}

				index++;
			}
		}

		public static void CoreSetValue<TValue>(this ITextControl textControl, TValue value)
		{
			CoreSetValue<TValue>(textControl, value, null, null);
		}

		public static void CoreSetValue<TValue>(this ITextControl textControl, TValue value, string format, string @default)
		{
			if ((object)textControl == null)
				throw new ArgumentNullException("textControl");

			textControl.Text = value.SafeToString(format, @default ?? "");
		}

		#endregion
	}
}