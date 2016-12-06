/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Datazoid.Models.Tabular
{
	public abstract class DynamicTableModelObject : DynamicModelObject, ITableModelObject
	{
		#region Constructors/Destructors

		protected DynamicTableModelObject()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the ordinal sorted array values identifying the current table model object instance.
		/// </summary>
		[JsonIgnore]
		public abstract object[] IdValues
		{
			get;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the current table model object instance
		/// is new (never been persisted) or old (has been persisted).
		/// </summary>
		[JsonIgnore]
		public abstract bool IsNew
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Called prior to any non-nullipotent (e.g. insert, update, delete) operation.
		/// </summary>
		public virtual void Mark()
		{
			// do nothing
		}

		/// <summary>
		/// Validates this model instance.
		/// </summary>
		/// <returns> A enumerable of zero or more messages. </returns>
		public virtual IEnumerable<Message> Validate()
		{
			// do nothing
			return new Message[] { };
		}

		#endregion
	}
}