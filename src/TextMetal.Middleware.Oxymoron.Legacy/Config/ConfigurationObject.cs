/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

using Newtonsoft.Json;

using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Config
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
		private IConfigurationObjectCollection surround;

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

		[JsonIgnore]
		public IConfigurationObjectCollection Surround
		{
			get
			{
				return this.surround;
			}
			set
			{
				this.surround = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected static Message NewError(string description)
		{
			return new Message(string.Empty, description, Severity.Error);
		}

		protected void EnsureParentOnPropertySet(IConfigurationObject oldValueObj, IConfigurationObject newValueObj)
		{
			if ((object)oldValueObj != null)
			{
				oldValueObj.Surround = null;
				oldValueObj.Parent = null;
			}

			if ((object)newValueObj != null)
			{
				newValueObj.Surround = null;
				newValueObj.Parent = this;
			}
		}

		public abstract IEnumerable<Message> Validate();

		#endregion
	}
}