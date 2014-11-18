/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data.Framework
{
	public abstract class ModelObject : BasicObject, IModelObject
	{
		#region Constructors/Destructors

		protected ModelObject()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		public abstract bool IsNew
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		public virtual void Mark()
		{
			// do nothing
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