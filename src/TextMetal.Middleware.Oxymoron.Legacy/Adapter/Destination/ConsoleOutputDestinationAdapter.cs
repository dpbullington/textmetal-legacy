/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Oxymoron.Legacy.Config.Adapters;
using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Primitives;
using TextMetal.Middleware.Textual.Delimited;
using TextMetal.Middleware.Textual.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Destination
{
	public class ConsoleOutputDestinationAdapter : DestinationAdapter<AdapterSpecificConfiguration>, IConsoleAdapter
	{
		#region Constructors/Destructors

		public ConsoleOutputDestinationAdapter()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly TextWriter textWriter = Console.Out;

		#endregion

		#region Properties/Indexers/Events

		private TextWriter TextWriter
		{
			get
			{
				return this.textWriter;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreInitialize()
		{
			// DO NOTHING
		}

		protected override void CorePushData(TableConfiguration tableConfiguration, IEnumerable<IRecord> sourceDataEnumerable)
		{
			if ((object)tableConfiguration == null)
				throw new ArgumentNullException(nameof(tableConfiguration));

			if ((object)sourceDataEnumerable == null)
				throw new ArgumentNullException(nameof(sourceDataEnumerable));

			foreach (IRecord sourceDataRecord in sourceDataEnumerable)
			{
				string temp;
				temp = string.Join("|", sourceDataRecord.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value.SafeToString("null"))).ToArray());
				this.TextWriter.WriteLine(temp);
			}
		}

		protected override void CoreTerminate()
		{
			// DO NOT DISPOSE
		}

		#endregion
	}
}