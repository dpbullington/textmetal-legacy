/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;

using TextMetal.Middleware.Solder;

namespace TextMetal.Middleware.Data.Models.Functional
{
	public abstract class CallProcedureModelObject : ModelObject, ICallProcedureModelObject, INotifyPropertyChanged
	{
		#region Constructors/Destructors

		protected CallProcedureModelObject()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		private event PropertyChangedEventHandler PropertyChanged;

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add
			{
				this.PropertyChanged += value;
			}
			remove
			{
				this.PropertyChanged -= value;
			}
		}

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

			return messages.ToArray();
		}

		#endregion
	}
}