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

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Source
{
	public class DelimitedTextSourceAdapter : SourceAdapter<DelimitedTextAdapterConfiguration>, IDelimitedTextAdapter
	{
		#region Constructors/Destructors

		public DelimitedTextSourceAdapter()
		{
		}

		#endregion

		#region Fields/Constants

		private RecordTextReader delimitedTextReader;

		#endregion

		#region Properties/Indexers/Events

		private RecordTextReader DelimitedTextReader
		{
			get
			{
				return this.delimitedTextReader;
			}
			set
			{
				this.delimitedTextReader = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreInitialize()
		{
			IEnumerable<ITextHeaderSpec> headerSpecs;

			if ((object)this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec == null)
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "DelimitedTextSpec"));

			if (SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextFilePath))
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "DelimitedTextFilePath"));

			this.DelimitedTextReader = new DelimitedTextReader(new StreamReader(File.Open(this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextFilePath, FileMode.Open, FileAccess.Read, FileShare.None)), this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec);
			headerSpecs = this.DelimitedTextReader.ReadHeaderSpecs();

			this.UpstreamMetadata = headerSpecs.Select((hs, i) => new Column()
																{
																	TableIndex = 0,
																	ColumnIndex = i,
																	ColumnName = hs.HeaderName,
																	ColumnType = hs.GetClrTypeFromFieldType(),
																	ColumnIsNullable = true,
																	Context = hs
																});
		}

		protected override IEnumerable<IRecord> CorePullData(TableConfiguration tableConfiguration)
		{
			IEnumerable<IRecord> sourceDataEnumerable;

			if ((object)tableConfiguration == null)
				throw new ArgumentNullException(nameof(tableConfiguration));

			sourceDataEnumerable = this.DelimitedTextReader.ReadRecords();

			return sourceDataEnumerable;
		}

		protected override void CoreTerminate()
		{
			if ((object)this.DelimitedTextReader != null)
				this.DelimitedTextReader.Dispose();

			this.DelimitedTextReader = null;
		}

		#endregion
	}
}