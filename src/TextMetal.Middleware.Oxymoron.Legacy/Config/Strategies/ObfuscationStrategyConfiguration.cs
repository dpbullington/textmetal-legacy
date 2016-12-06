/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

using Newtonsoft.Json;

using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Config.Strategies
{
	public class ObfuscationStrategyConfiguration : ConfigurationObject
	{
		#region Constructors/Destructors

		public ObfuscationStrategyConfiguration()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		[JsonIgnore]
		public new ColumnConfiguration Parent
		{
			get
			{
				return (ColumnConfiguration)base.Parent;
			}
			set
			{
				base.Parent = value;
			}
		}

		#endregion

		#region Methods/Operators

		public sealed override IEnumerable<Message> Validate()
		{
			return this.Validate(null);
		}

		public virtual IEnumerable<Message> Validate(int? columnIndex)
		{
			return new Message[] { };
		}

		#endregion
	}
}