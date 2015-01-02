/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;
using System.ComponentModel;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data.Framework
{
	public abstract class BasicObject : IBasicObject
	{
		#region Constructors/Destructors

		protected BasicObject()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Methods/Operators

		public void OnAllPropertiesChanged()
		{
			this.OnPropertyChanged(null);
		}

		public void OnPropertyChanged(string propertyName)
		{
			if ((object)this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public virtual IEnumerable<Message> Validate()
		{
			List<Message> messages;

			messages = new List<Message>();

			return messages;
		}

		#endregion
	}
}