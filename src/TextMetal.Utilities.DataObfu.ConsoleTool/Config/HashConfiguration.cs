/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Core;

namespace TextMetal.Utilities.DataObfu.ConsoleTool.Config
{
	public class HashConfiguration : ConfigurationObject
	{
		#region Constructors/Destructors

		public HashConfiguration()
		{
		}

		#endregion

		#region Fields/Constants

		private long? multiplier;
		private long? seed;
		private long? size;

		#endregion

		#region Properties/Indexers/Events

		public long? Multiplier
		{
			get
			{
				return this.multiplier;
			}
			set
			{
				this.multiplier = value;
			}
		}

		public long? Seed
		{
			get
			{
				return this.seed;
			}
			set
			{
				this.seed = value;
			}
		}

		public long? Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		#endregion

		#region Methods/Operators

		public HashConfiguration Clone()
		{
			return new HashConfiguration() { Multiplier = this.Multiplier, Size = this.Size, Seed = this.Seed };
		}

		public override IEnumerable<Message> Validate()
		{
			return this.Validate("<unknown>");
		}

		public IEnumerable<Message> Validate(string prefix)
		{
			List<Message> messages;

			messages = new List<Message>();

			if ((object)this.Multiplier == null)
				messages.Add(NewError(string.Format("{0} multiplier is required.", prefix)));

			if ((object)this.Size == null)
				messages.Add(NewError(string.Format("{0} size is required.", prefix)));

			if ((object)this.Seed == null)
				messages.Add(NewError(string.Format("{0} seed is required.", prefix)));

			return messages;
		}

		#endregion
	}
}