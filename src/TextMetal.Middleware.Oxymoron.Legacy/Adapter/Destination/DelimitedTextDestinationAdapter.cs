/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;

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
			AdapterConfiguration<DelimitedTextAdapterConfiguration> sourceAdapterConfiguration;
			DelimitedTextSpec effectiveDelimitedTextSpec;

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextFilePath))
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "DestinationAdapterConfiguration.DelimitedTextAdapterConfiguration.DelimitedTextFilePath"));

			sourceAdapterConfiguration = new AdapterConfiguration<DelimitedTextAdapterConfiguration>(((ObfuscationConfiguration)this.AdapterConfiguration.Parent).SourceAdapterConfiguration);

			if ((object)this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec == null &&
				((object)sourceAdapterConfiguration == null ||
				(object)sourceAdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec == null))
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "[Source/Destination]...DelimitedTextSpec"));

			if ((object)this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec != null)
			{
				effectiveDelimitedTextSpec = this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec;

				if (effectiveDelimitedTextSpec.HeaderSpecs.Count <= 0 &&
					(object)sourceAdapterConfiguration != null &&
					(object)sourceAdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec.HeaderSpecs != null)
					effectiveDelimitedTextSpec.HeaderSpecs.AddRange(sourceAdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec.HeaderSpecs);
			}
			else
				effectiveDelimitedTextSpec = this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec;

			if ((object)effectiveDelimitedTextSpec == null ||
				effectiveDelimitedTextSpec.HeaderSpecs.Count <= 0)
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "[Source/Destination]...DelimitedTextSpec.HeaderSpecs"));

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