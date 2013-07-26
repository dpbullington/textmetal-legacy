// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace NUnit.Util
{
	/// <summary>
	/// 	Summary description for ServerBase.
	/// </summary>
	public abstract class ServerBase : MarshalByRefObject, IDisposable
	{
		#region Constructors/Destructors

		protected ServerBase()
		{
		}

		/// <summary>
		/// 	Constructor used to provide
		/// </summary>
		/// <param name="uri"> </param>
		/// <param name="port"> </param>
		protected ServerBase(string uri, int port)
		{
			this.uri = uri;
			this.port = port;
		}

		#endregion

		#region Fields/Constants

		private TcpChannel channel;
		private bool isMarshalled;
		protected int port;

		private object theLock = new object();
		protected string uri;

		#endregion

		#region Properties/Indexers/Events

		public string ServerUrl
		{
			get
			{
				return string.Format("tcp://127.0.0.1:{0}/{1}", this.port, this.uri);
			}
		}

		#endregion

		#region Methods/Operators

		public void Dispose()
		{
			this.Stop();
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public virtual void Start()
		{
			if (this.uri != null && this.uri != string.Empty)
			{
				lock (this.theLock)
				{
					this.channel = ServerUtilities.GetTcpChannel(this.uri + "Channel", this.port, 100);

					RemotingServices.Marshal(this, this.uri);
					this.isMarshalled = true;
				}

				if (this.port == 0)
				{
					ChannelDataStore store = this.channel.ChannelData as ChannelDataStore;
					if (store != null)
					{
						string channelUri = store.ChannelUris[0];
						this.port = int.Parse(channelUri.Substring(channelUri.LastIndexOf(':') + 1));
					}
				}
			}
		}

		[OneWay]
		public virtual void Stop()
		{
			lock (this.theLock)
			{
				if (this.isMarshalled)
				{
					RemotingServices.Disconnect(this);
					this.isMarshalled = false;
				}

				if (this.channel != null)
				{
					ChannelServices.UnregisterChannel(this.channel);
					this.channel = null;
				}

				Monitor.PulseAll(this.theLock);
			}
		}

		public void WaitForStop()
		{
			lock (this.theLock)
			{
				Monitor.Wait(this.theLock);
			}
		}

		#endregion
	}
}