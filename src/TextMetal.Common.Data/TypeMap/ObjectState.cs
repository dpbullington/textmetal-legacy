/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.TypeMap
{
	/// <summary>
	/// Specifies object states.
	/// </summary>
	[Serializable]
	public enum ObjectState : uint
	{
		/// <summary>
		/// The current object instance has not incurred any changes.
		/// </summary>
		Consistent = 0x00000000,

		/// <summary>
		/// The current object instance has been marked as modified.
		/// This state implies that the object has changed.
		/// </summary>
		Modified = 1,

		/// <summary>
		/// The current object instance has been marked as removed.
		/// This state implies that the object has been marked for removal but
		/// has NOT yet been removed from a persistent medium.
		/// </summary>
		Removed = 2,

		/// <summary>
		/// The current object instance has been marked as obsoleted.
		/// This state implies that the object has:
		/// a) been superceded by a newer version;
		/// b) is no longer required (possibly due to a successful removal);
		/// c) is a clone of another instance and should not be used for operational purposes.
		/// </summary>
		Obsoleted = 3,

		/// <summary>
		/// The current object instance cannot be determined.
		/// </summary>
		Faulty = 0xFFFFFFFF
	}
}