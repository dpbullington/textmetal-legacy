/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.ComponentModel;

namespace TextMetal.Middleware.Data.Models
{
	public abstract class ModelObject : IModelObject, INotifyPropertyChanged
	{
		#region Constructors/Destructors

		protected ModelObject()
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

		protected void OnAllPropertiesChanged()
		{
			this.OnPropertyChanged(null);
		}

		protected void OnPropertyChanged(string propertyName)
		{
			if ((object)this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}