/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.TypeMap
{
	/// <summary>
	/// Provides a base for plain objects (domain, transfer, etc.).
	/// </summary>
	[Serializable]
	public abstract class PlainObject : MarshalByRefObject, IPlainObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the PlainObject class.
		/// </summary>
		protected PlainObject()
		{
		}

		#endregion

		#region Fields/Constants

		private ObjectState objectState;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets a value indicating whether the current plain object instance is
		/// new (never been persisted) or old (has been persisted).
		/// </summary>
		public abstract bool IsNew
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the current plain object state.
		/// </summary>
		public virtual ObjectState ObjectState
		{
			get
			{
				return this.objectState;
			}
			set
			{
				this.objectState = value;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Determines the post-perist state for a plain object instance.
		/// </summary>
		/// <param name="obj"> The plain object instance to determine the post-perist state. </param>
		/// <returns> The post-perist state. </returns>
		public static ObjectState DetermineAfter(IPlainObject obj)
		{
			if ((object)obj == null)
				throw new ArgumentNullException("obj");

			if (obj.ObjectState == ObjectState.Modified)
				return ObjectState.Consistent;
			else if (obj.ObjectState == ObjectState.Removed)
				return ObjectState.Obsoleted;
			else if (obj.ObjectState == ObjectState.Consistent)
				return ObjectState.Consistent;
			else if (obj.ObjectState == ObjectState.Obsoleted)
				return ObjectState.Obsoleted;
			else
				return ObjectState.Faulty;
		}

		/// <summary>
		/// Gets or sets the current plain object data operation.
		/// </summary>
		public static DataOperation DetermineBefore(IPlainObject obj)
		{
			DataOperation dataOperation;

			if ((object)obj == null)
				throw new ArgumentNullException("obj");

			if (obj.ObjectState == ObjectState.Consistent ||
				obj.ObjectState == ObjectState.Obsoleted ||
				(obj.ObjectState == ObjectState.Removed && obj.IsNew))
				dataOperation = DataOperation.None;
			else if (obj.ObjectState == ObjectState.Removed && !obj.IsNew)
				dataOperation = DataOperation.Delete;
			else if (obj.ObjectState == ObjectState.Modified && obj.IsNew)
				dataOperation = DataOperation.Insert;
			else if (obj.ObjectState == ObjectState.Modified && !obj.IsNew)
				dataOperation = DataOperation.Update;
			else
				dataOperation = DataOperation.StateError;

			return dataOperation;
		}

		#endregion
	}
}