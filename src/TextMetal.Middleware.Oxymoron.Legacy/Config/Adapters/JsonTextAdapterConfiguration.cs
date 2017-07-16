/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Config.Adapters
{
	public class JsonTextAdapterConfiguration : AdapterSpecificConfiguration
	{
		#region Constructors/Destructors

		public JsonTextAdapterConfiguration()
		{
		}

		#endregion

		#region Fields/Constants

		private string jsonTextFilePath;

		#endregion

		#region Properties/Indexers/Events

		public string JsonTextFilePath
		{
			get
			{
				return this.jsonTextFilePath;
			}
			set
			{
				this.jsonTextFilePath = value;
			}
		}

		#endregion

		#region Methods/Operators

		public override IEnumerable<Message> Validate(string adapterContext)
		{
			List<Message> messages;

			messages = new List<Message>();

			if (SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(this.JsonTextFilePath))
				messages.Add(NewError(string.Format("{0} adapter JSON text file path is required.", adapterContext)));

			return messages;
		}

		#endregion
	}
}