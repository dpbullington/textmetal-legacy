/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

using Newtonsoft.Json;

using TextMetal.Common.Core;

namespace TextMetal.Utilities.DataObfu.ConsoleTool.Config
{
	public abstract class ConfigurationObject : IConfigurationObject
	{
		#region Constructors/Destructors

		protected ConfigurationObject()
		{
		}

		#endregion

		#region Fields/Constants

		private IConfigurationObject parent;

		#endregion

		#region Properties/Indexers/Events

		[JsonIgnore]
		public IConfigurationObject Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected static Message NewError(string description)
		{
			return new Message("", description, Severity.Error);
		}

		protected void EnsureParentOnPropertySet(IConfigurationObject oldValueObj, IConfigurationObject newValueObj)
		{
			if ((object)oldValueObj != null)
				oldValueObj.Parent = null;

			if ((object)newValueObj != null)
				newValueObj.Parent = this;
		}

		public abstract IEnumerable<Message> Validate();

		#endregion
	}
}