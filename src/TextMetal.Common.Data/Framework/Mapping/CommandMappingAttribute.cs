/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

namespace TextMetal.Common.Data.Framework.Mapping
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class CommandMappingAttribute : Attribute
	{
		#region Constructors/Destructors

		public CommandMappingAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private CommandBehavior commandBehavior = CommandBehavior.Default;
		private bool commandPrepare;
		private string commandText;
		private int? commandTimeout;
		private CommandType commandType;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets the command behavior.
		/// </summary>
		public CommandBehavior CommandBehavior
		{
			get
			{
				return this.commandBehavior;
			}
			set
			{
				this.commandBehavior = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to prepare the command.
		/// </summary>
		public bool CommandPrepare
		{
			get
			{
				return this.commandPrepare;
			}
			set
			{
				this.commandPrepare = value;
			}
		}

		/// <summary>
		/// Gets or sets the command timeout.
		/// </summary>
		public int? CommandTimeout
		{
			get
			{
				return this.commandTimeout;
			}
			set
			{
				this.commandTimeout = value;
			}
		}

		/// <summary>
		/// Gets or sets the command type.
		/// </summary>
		public CommandType CommandType
		{
			get
			{
				return this.commandType;
			}
			set
			{
				this.commandType = value;
			}
		}

		/// <summary>
		/// Gets or sets the command text.
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

		#endregion
	}
}