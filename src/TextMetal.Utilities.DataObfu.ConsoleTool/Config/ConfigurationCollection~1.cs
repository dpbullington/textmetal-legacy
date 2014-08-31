/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.ObjectModel;

using Newtonsoft.Json;

namespace TextMetal.Utilities.DataObfu.ConsoleTool.Config
{
	public class ConfigurationCollection<TConfigurationObject> : Collection<TConfigurationObject>
		where TConfigurationObject : IConfigurationObject
	{
		#region Constructors/Destructors

		public ConfigurationCollection(IConfigurationObject site)
		{
			if ((object)site == null)
				throw new ArgumentNullException("site");

			this.site = site;
		}

		#endregion

		#region Fields/Constants

		private readonly IConfigurationObject site;

		#endregion

		#region Properties/Indexers/Events

		[JsonIgnore]
		public IConfigurationObject Site
		{
			get
			{
				return this.site;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void ClearItems()
		{
			foreach (TConfigurationObject item in base.Items)
				item.Parent = null;

			base.ClearItems();
		}

		protected override void InsertItem(int index, TConfigurationObject item)
		{
			if ((object)item == null)
				throw new ArgumentNullException("item");

			item.Parent = this.Site;

			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			TConfigurationObject item;

			item = base[index];

			if ((object)item == null)
				item.Parent = null;

			base.RemoveItem(index);
		}

		protected override void SetItem(int index, TConfigurationObject item)
		{
			if ((object)item == null)
				throw new ArgumentNullException("item");

			item.Parent = this.Site;

			base.SetItem(index, item);
		}

		#endregion
	}
}