/*
	Copyright ©2002-2010 D. P. Bullington
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

namespace TextMetal.Common.Data.TypeMap.Contracts
{
	/// <summary>
	/// Indicates that a method defines a command that is part of a database contract.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class CommandContractAttribute : Attribute
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the CommandContractAttribute class.
		/// </summary>
		public CommandContractAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private CommandBehavior behavior = CommandBehavior.Default;
		private string commandText;
		private bool prepare;
		private int timeout;
		private CommandType type;
		private bool useDefaultTimeout = true;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets the database command behavior.
		/// </summary>
		public CommandBehavior Behavior
		{
			get
			{
				return this.behavior;
			}
			set
			{
				this.behavior = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to prepare the database command.
		/// </summary>
		public bool Prepare
		{
			get
			{
				return this.prepare;
			}
			set
			{
				this.prepare = value;
			}
		}

		/// <summary>
		/// Gets or sets the database command text.
		/// </summary>
		public string Text
		{
			get
			{
				return this.commandText;
			}
			set
			{
				this.commandText = value;
			}
		}

		/// <summary>
		/// Gets or sets the database command timeout.
		/// </summary>
		public int Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		/// <summary>
		/// Gets or sets the database command type.
		/// </summary>
		public CommandType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to use the database command timeout default.
		/// </summary>
		public bool UseDefaultTimeout
		{
			get
			{
				return this.useDefaultTimeout;
			}
			set
			{
				this.useDefaultTimeout = value;
			}
		}

		#endregion
	}
}