/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Hosting
{
	public abstract class OxymoronHost : IOxymoronHost
	{
		#region Constructors/Destructors

		protected OxymoronHost()
		{
		}

		#endregion

		#region Fields/Constants

		private bool disposed;

		#endregion

		#region Properties/Indexers/Events

		public bool Disposed
		{
			get
			{
				return this.disposed;
			}
			private set
			{
				this.disposed = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected virtual void CoreDispose(bool disposing)
		{
			if (disposing)
			{
				// do nothing
			}
		}

		protected abstract object CoreGetValueForIdViaDictionaryResolution(DictionaryConfiguration dictionaryConfiguration, IColumn column, object surrogateId);

		public void Dispose()
		{
			if (this.Disposed)
				return;

			try
			{
				this.CoreDispose(true);
			}
			finally
			{
				this.Disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		public object GetValueForIdViaDictionaryResolution(DictionaryConfiguration dictionaryConfiguration, IColumn column, object surrogateId)
		{
			return this.CoreGetValueForIdViaDictionaryResolution(dictionaryConfiguration, column, surrogateId);
		}

		#endregion
	}
}