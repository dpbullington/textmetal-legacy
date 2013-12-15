// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.IO;
using System.Text;

namespace Castle.Core.Resource
{
	public delegate Stream StreamFactory();

	/// <summary>
	/// </summary>
	public abstract class AbstractStreamResource : AbstractResource
	{
		#region Constructors/Destructors

		~AbstractStreamResource()
		{
			this.Dispose(false);
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// This returns a new stream instance each time it is called.
		/// It is the responsibility of the caller to dispose of this stream
		/// </summary>
		private StreamFactory createStream;

		#endregion

		#region Properties/Indexers/Events

		public StreamFactory CreateStream
		{
			get
			{
				return this.createStream;
			}
			set
			{
				this.createStream = value;
			}
		}

		#endregion

		#region Methods/Operators

		public override TextReader GetStreamReader()
		{
			return new StreamReader(this.CreateStream());
		}

		public override TextReader GetStreamReader(Encoding encoding)
		{
			return new StreamReader(this.CreateStream(), encoding);
		}

		#endregion
	}
}