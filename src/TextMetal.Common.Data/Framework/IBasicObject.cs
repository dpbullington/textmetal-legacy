/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;
using System.ComponentModel;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data.Framework
{
	public interface IBasicObject : INotifyPropertyChanged
	{
		/// <summary>
		/// Called when all properties values change.
		/// </summary>
		void OnAllPropertiesChanged();

		/// <summary>
		/// Called when a property value changes.
		/// </summary>
		/// <param name="propertyName"> The CLR name of the property thaat has changed values. </param>
		void OnPropertyChanged(string propertyName);

		/// <summary>
		/// Validates this model instance.
		/// </summary>
		/// <returns> A enumerable of zero or more messages. </returns>
		IEnumerable<Message> Validate();
	}
}