/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;
using System.ServiceModel;
using System.Web;

namespace TextMetal.Common.Solder.AmbientExecutionContext
{
	public sealed class WcfContextExecutionPathStorage : IExecutionPathStorage
	{
		#region Constructors/Destructors

		public WcfContextExecutionPathStorage()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets a value indicating if the current application domain is running under WCF.
		/// </summary>
		public static bool IsInWcfContext
		{
			get
			{
				return (object)OperationContext.Current != null;
			}
		}

		private static WcfContextExtension WcfContext
		{
			get
			{
				WcfContextExtension wcfContextExtension;

				wcfContextExtension = OperationContext.Current.Extensions.Find<WcfContextExtension>();

				if ((object)wcfContextExtension == null)
				{
					wcfContextExtension = new WcfContextExtension();
					OperationContext.Current.Extensions.Add(wcfContextExtension);
				}

				return wcfContextExtension;
			}
		}

		#endregion

		#region Methods/Operators

		public object GetValue(string key)
		{
			return HttpContext.Current.Items[key];
		}

		public void RemoveValue(string key)
		{
			HttpContext.Current.Items.Remove(key);
		}

		public void SetValue(string key, object value)
		{
			HttpContext.Current.Items[key] = value;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		public sealed class WcfContextExtension : IExtension<OperationContext>
		{
			#region Constructors/Destructors

			public WcfContextExtension()
			{
			}

			#endregion

			#region Fields/Constants

			private readonly IDictionary<string, object> context = new Dictionary<string, object>();

			#endregion

			#region Properties/Indexers/Events

			public IDictionary<string, object> Context
			{
				get
				{
					return this.context;
				}
			}

			#endregion

			#region Methods/Operators

			public void Attach(OperationContext owner)
			{
				// do nothing
			}

			public void Detach(OperationContext owner)
			{
				// do nothing
			}

			#endregion
		}

		#endregion
	}
}