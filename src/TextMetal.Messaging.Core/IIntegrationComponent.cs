/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Messaging.Core
{
	public interface IIntegrationComponent : IDisposable
	{
		#region Properties/Indexers/Events

		Guid ComponentClassId
		{
			get;
		}

		Guid DesignTimeId
		{
			get;
		}

		bool IsDisposed
		{
			get;
		}

		bool IsInitialized
		{
			get;
		}

		bool IsMutable
		{
			get;
		}

		bool IsTerminated
		{
			get;
		}

		Guid RunTimeId
		{
			get;
		}

		#endregion

		#region Methods/Operators

		void Freeze();

		void Initialize();

		void Terminate();

		#endregion
	}
}