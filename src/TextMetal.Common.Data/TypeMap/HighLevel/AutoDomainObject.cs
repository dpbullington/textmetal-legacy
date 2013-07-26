/*
	Copyright ©2002-2010 Daniel Bullington
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.ComponentModel;
using System.Reflection;

using TextMetal.Common.Data.TypeMap.Objects;

namespace TextMetal.Common.Data.TypeMap.HighLevel
{
	public abstract class AutoDomainObject : PlainObject, INotifyPropertyChanged
	{
		#region Constructors/Destructors

		protected AutoDomainObject()
		{
			this.CreationTimestamp = DateTime.Now;
		}

		#endregion

		#region Fields/Constants

		private DateTime? creationTimestamp;
		private DateTime? modificationTimestamp;
		private DateTime? previousModificationTimestamp;

		#endregion

		#region Properties/Indexers/Events

		public event PropertyChangedEventHandler PropertyChanged;

		public DateTime? CreationTimestamp
		{
			get
			{
				return this.creationTimestamp;
			}
			set
			{
				this.creationTimestamp = value;
			}
		}

		public string IsMarkedForDeletion
		{
			get
			{
				return this.ObjectState == ObjectState.Removed ? "Yes" : "";
			}
		}

		[SuppressMapping]
		public override sealed bool IsNew
		{
			get
			{
				return (object)this.ObjectIdValue == null;
			}
			set
			{
				if (value)
					this.ObjectIdValue = null;
			}
		}

		public DateTime? ModificationTimestamp
		{
			get
			{
				return this.modificationTimestamp;
			}
			set
			{
				this.modificationTimestamp = value;
			}
		}

		public PropertyInfo ObjectIdProperty
		{
			get
			{
				Type thisType;
				PropertyInfo propertyInfo;

				thisType = this.GetType();

				if ((object)thisType == null)
					throw new InvalidOperationException();

				propertyInfo = thisType.GetProperty(string.Format("{0}Id", thisType.Name), BindingFlags.Public | BindingFlags.Instance);

				if ((object)propertyInfo == null)
					throw new InvalidOperationException();

				return propertyInfo;
			}
		}

		[SuppressMapping]
		public object ObjectIdValue
		{
			get
			{
				object value;

				value = this.ObjectIdProperty.GetValue(this, null);

				return value;
			}
			set
			{
				this.ObjectIdProperty.SetValue(this, value, null);
			}
		}

		[SuppressMapping]
		public override sealed ObjectState ObjectState
		{
			get
			{
				return base.ObjectState;
			}
			set
			{
				if (this.ObjectState == ObjectState.Consistent &&
				    (value == ObjectState.Modified || value == ObjectState.Removed))
				{
					this.PreviousModificationTimestamp = this.ModificationTimestamp;
					this.ModificationTimestamp = DateTime.Now;
				}
				else if (this.ObjectState == ObjectState.Modified &&
				         value == ObjectState.Consistent)
					this.PreviousModificationTimestamp = this.ModificationTimestamp;

				base.ObjectState = value;

				this.FirePropertyChange("ObjectState");
			}
		}

		[SuppressMapping]
		public DateTime? PreviousModificationTimestamp
		{
			get
			{
				return this.previousModificationTimestamp;
			}
			set
			{
				this.previousModificationTimestamp = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected void FirePropertyChange(string propertyName)
		{
			if ((object)this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}