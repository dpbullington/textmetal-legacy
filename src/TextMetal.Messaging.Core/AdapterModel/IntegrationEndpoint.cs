/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Messaging.Core.AdapterModel
{
	public sealed class IntegrationEndpoint
	{
		#region Constructors/Destructors

		public IntegrationEndpoint(IntegrationAddress address, IntegrationBinding binding, IntegrationContract contract, Type pipeline)
		{
			if ((object)address == null)
				throw new ArgumentNullException(nameof(address));

			if ((object)binding == null)
				throw new ArgumentNullException(nameof(binding));

			if ((object)contract == null)
				throw new ArgumentNullException(nameof(contract));

			if ((object)pipeline == null)
				throw new ArgumentNullException(nameof(pipeline));

			this.address = address;
			this.binding = binding;
			this.contract = contract;
			this.pipeline = pipeline;
		}

		private IntegrationEndpoint()
		{
		}

		#endregion

		#region Fields/Constants

		private static readonly IntegrationEndpoint empty = new IntegrationEndpoint();
		private readonly IntegrationAddress address;
		private readonly IntegrationBinding binding;
		private readonly IntegrationContract contract;
		private readonly Type pipeline;

		#endregion

		#region Properties/Indexers/Events

		public static IntegrationEndpoint Empty
		{
			get
			{
				return empty;
			}
		}

		public IntegrationAddress Address
		{
			get
			{
				return this.address;
			}
		}

		public IntegrationBinding Binding
		{
			get
			{
				return this.binding;
			}
		}

		public IntegrationContract Contract
		{
			get
			{
				return this.contract;
			}
		}

		public Type Pipeline
		{
			get
			{
				return this.pipeline;
			}
		}

		#endregion
	}
}