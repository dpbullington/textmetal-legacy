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
	public class DelimitedTextDestinationAdapter : DestinationAdapter<DelimitedTextAdapterConfiguration>, IDelimitedTextAdapter
	{
		#region Constructors/Destructors

		public DelimitedTextDestinationAdapter()
		{
		}

		#endregion

		#region Fields/Constants

		private RecordTextWriter delimitedTextWriter;

		#endregion

		#region Properties/Indexers/Events

		private RecordTextWriter DelimitedTextWriter
		{
			get
			{
				return this.delimitedTextWriter;
			}
			set
			{
				this.delimitedTextWriter = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreInitialize()
		{
			TextHeaderSpec[] upstreamTextHeaderSpec;

			AdapterConfiguration<DelimitedTextAdapterConfiguration> destinationAdapterConfiguration;
			AdapterConfiguration<DelimitedTextAdapterConfiguration> sourceAdapterConfiguration;
			DelimitedTextSpec effectiveDelimitedTextSpec;

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextFilePath))
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "DestinationAdapterConfiguration.DelimitedTextAdapterConfiguration.DelimitedTextFilePath"));

			upstreamTextHeaderSpec = this.UpstreamMetadata.Select(um => um.Context).OfType<IRecord>().Select(r => r.Context).OfType<TextHeaderSpec>().ToArray();

			destinationAdapterConfiguration = this.AdapterConfiguration;
			sourceAdapterConfiguration = new AdapterConfiguration<DelimitedTextAdapterConfiguration>(((ObfuscationConfiguration)this.AdapterConfiguration.Parent).SourceAdapterConfiguration);

			if ((object)destinationAdapterConfiguration != null &&
				(object)destinationAdapterConfiguration.AdapterSpecificConfiguration != null &&
				(object)destinationAdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec != null)
				effectiveDelimitedTextSpec = destinationAdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec;
			else
				effectiveDelimitedTextSpec = new DelimitedTextSpec();

			// 2016-07-12 / dpbullington@gmail.com: fix NRE bug in the below check
			// attempt to "flow" the DTM spec from source to destination if not specified on destination
			if ((object)sourceAdapterConfiguration != null &&
				(object)sourceAdapterConfiguration.AdapterSpecificConfiguration != null &&
				(object)sourceAdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec != null)
			{
				if (effectiveDelimitedTextSpec.TextHeaderSpecs.Count <= 0)
				{
					if (upstreamTextHeaderSpec.Length <= 0)
						effectiveDelimitedTextSpec.TextHeaderSpecs.AddRange(sourceAdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec.TextHeaderSpecs);
					else
						effectiveDelimitedTextSpec.TextHeaderSpecs.AddRange(upstreamTextHeaderSpec);
				}

				if (effectiveDelimitedTextSpec.FieldDelimiter == null)
					effectiveDelimitedTextSpec.FieldDelimiter = sourceAdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec.FieldDelimiter;

				if (effectiveDelimitedTextSpec.QuoteValue == null)
					effectiveDelimitedTextSpec.QuoteValue = sourceAdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec.QuoteValue;

				if (effectiveDelimitedTextSpec.RecordDelimiter == null)
					effectiveDelimitedTextSpec.FieldDelimiter = sourceAdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec.RecordDelimiter;

				if (effectiveDelimitedTextSpec.FirstRecordIsHeader == null)
					effectiveDelimitedTextSpec.FirstRecordIsHeader = sourceAdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec.FirstRecordIsHeader;
			}

			if ((object)effectiveDelimitedTextSpec == null ||
				effectiveDelimitedTextSpec.TextHeaderSpecs.Count <= 0)
				throw new InvalidOperationException(string.Format("Configuration missing: [Source and/or Destination]...{0}.{1}", nameof(DelimitedTextSpec), nameof(DelimitedTextSpec.TextHeaderSpecs)));

			this.DelimitedTextWriter = new DelimitedTextWriter(new StreamWriter(File.Open(this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextFilePath, FileMode.Create, FileAccess.Write, FileShare.None)), effectiveDelimitedTextSpec);
		}

		protected override void CorePushData(TableConfiguration tableConfiguration, IEnumerable<IRecord> sourceDataEnumerable)
		{
			if ((object)tableConfiguration == null)
				throw new ArgumentNullException(nameof(tableConfiguration));

			if ((object)sourceDataEnumerable == null)
				throw new ArgumentNullException(nameof(sourceDataEnumerable));

			this.DelimitedTextWriter.WriteRecords(sourceDataEnumerable);
		}

		protected override void CoreTerminate()
		{
			if ((object)this.DelimitedTextWriter != null)
			{
				this.DelimitedTextWriter.Flush();
				this.DelimitedTextWriter.Dispose();
			}

			this.DelimitedTextWriter = null;
		}

		#endregion
	}
}