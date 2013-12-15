// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

using System.Threading;
using System.Xml;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	// DEPTH:               ROOT STATES:
	// 0: <?xml>            [Start, Prolog]
	// 1: <Root>            [Element, Attribute]
	// 2:   <Foo>...</Foo>  [Content]

	public class XmlSubtreeWriter : XmlWriter
	{
		#region Constructors/Destructors

		public XmlSubtreeWriter(IXmlNode node)
		{
			if (node == null)
				throw Error.ArgumentNull("node");

			this.node = node;
		}

		#endregion

		#region Fields/Constants

		private readonly IXmlNode node;
		private XmlWriter childWriter;
		private int depth;
		private XmlWriter rootWriter;
		private WriteState state;

		#endregion

		#region Properties/Indexers/Events

		private XmlWriter ChildWriter
		{
			get
			{
				return this.childWriter ?? (this.childWriter = this.node.WriteChildren());
			}
		}

		private bool IsInChild
		{
			get
			{
				return this.depth > 1;
			}
		}

		private bool IsInRoot
		{
			get
			{
				return this.depth > 0;
			}
		}

		private bool IsInRootAttribute
		{
			get
			{
				return this.state == WriteState.Attribute;
			}
		}

		private XmlWriter RootWriter
		{
			get
			{
				return this.rootWriter ?? (this.rootWriter = this.node.WriteAttributes());
			}
		}

		public override WriteState WriteState
		{
			get
			{
				return (this.IsInRoot && this.state == WriteState.Content) ? this.childWriter.WriteState : this.state;
			}
		}

		#endregion

		#region Methods/Operators

		public override void Close()
		{
			this.WithWriters(w => w.Close(), resetTo: WriteState.Closed, worksIfClosed: true);
		}

		protected override void Dispose(bool managed)
		{
			try
			{
				if (managed)
				{
					this.Reset(WriteState.Closed);
					this.DisposeWriter(ref this.rootWriter);
					this.DisposeWriter(ref this.childWriter);
				}
			}
			finally
			{
				base.Dispose(managed);
			}
		}

		private void DisposeWriter(ref XmlWriter writer)
		{
			var value = Interlocked.Exchange(ref writer, null);
			if (null != value)
				value.Close();
		}

		public override void Flush()
		{
			this.WithWriters(w => w.Flush());
		}

		public override string LookupPrefix(string ns)
		{
			// This one is the oddball
			try
			{
				string prefix;
				return
					( // Try child writer first
						null != this.childWriter &&
						null != (prefix = this.childWriter.LookupPrefix(ns))
						) ? prefix :
						( // Try root writer next
							null != this.rootWriter &&
							null != (prefix = this.rootWriter.LookupPrefix(ns))
							) ? prefix :
							null;
			}
			catch
			{
				this.Reset(WriteState.Error);
				throw;
			}
		}

		private void RequireNotClosed()
		{
			if (this.state == WriteState.Closed || this.state == WriteState.Error)
				throw Error.InvalidOperation();
		}

		private void RequireState(WriteState state)
		{
			if (this.state != state)
				throw Error.InvalidOperation();
		}

		private void RequireState(WriteState state1, WriteState state2)
		{
			if (this.state != state1 && this.state != state2)
				throw Error.InvalidOperation();
		}

		private void Reset(WriteState state)
		{
			this.depth = 0;
			this.state = state;
		}

		private void WithWriters(Action<XmlWriter> action, bool worksIfClosed = false, WriteState? resetTo = null)
		{
			try
			{
				if (! worksIfClosed)
					this.RequireNotClosed();
				if (null != this.rootWriter)
					action(this.rootWriter);
				if (null != this.childWriter)
					action(this.childWriter);
				if (null != resetTo)
					this.Reset(resetTo.Value);
			}
			catch
			{
				this.Reset(WriteState.Error);
				throw;
			}
		}

		private void WriteAttribute(Action<XmlWriter> action, WriteState entryState, WriteState exitState)
		{
			try
			{
				if (this.IsInChild)
					action(this.ChildWriter);
				else // is in root (or prolog)
				{
					this.RequireState(entryState);
					action(this.RootWriter);
					this.state = exitState;
				}
			}
			catch
			{
				this.Reset(WriteState.Error);
				throw;
			}
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			this.WriteElementOrAttributeContent(w => w.WriteBase64(buffer, index, count));
		}

		public override void WriteCData(string text)
		{
			this.WriteElementContent(w => w.WriteCData(text));
		}

		public override void WriteCharEntity(char ch)
		{
			this.WriteElementOrAttributeContent(w => w.WriteCharEntity(ch));
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.WriteElementOrAttributeContent(w => w.WriteChars(buffer, index, count));
		}

		public override void WriteComment(string text)
		{
			this.WriteElementContent(w => w.WriteComment(text));
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			this.RequireState(WriteState.Start, WriteState.Prolog);
			this.state = WriteState.Prolog;
			// (do not write anything)
		}

		private void WriteElementContent(Action<XmlWriter> action)
		{
			try
			{
				this.RequireState(WriteState.Element, WriteState.Content);
				action(this.ChildWriter);
				this.state = WriteState.Content;
			}
			catch
			{
				this.Reset(WriteState.Error);
				throw;
			}
		}

		private void WriteElementOrAttributeContent(Action<XmlWriter> action)
		{
			try
			{
				if (this.IsInChild)
					action(this.ChildWriter);
				else if (this.IsInRootAttribute)
					action(this.RootWriter);
				else // is in root (or prolog)
				{
					this.RequireState(WriteState.Element, WriteState.Content);
					action(this.ChildWriter);
					this.state = WriteState.Content;
				}
			}
			catch
			{
				this.Reset(WriteState.Error);
				throw;
			}
		}

		public override void WriteEndAttribute()
		{
			this.WriteAttribute(w => w.WriteEndAttribute(),
				entryState: WriteState.Attribute,
				exitState: WriteState.Element);
		}

		public override void WriteEndDocument()
		{
			this.WithWriters(w => w.WriteEndDocument(), resetTo: WriteState.Start);
		}

		private void WriteEndElement(Action<XmlWriter> action)
		{
			try
			{
				if (this.IsInChild)
				{
					action(this.ChildWriter);
					this.state = WriteState.Content;
				}
				else // is in root (or prolog)
				{
					this.RequireState(WriteState.Element, WriteState.Content);
					this.state = WriteState.Prolog;
				}
				this.depth--;
			}
			catch
			{
				this.Reset(WriteState.Error);
				throw;
			}
		}

		public override void WriteEndElement()
		{
			this.WriteEndElement(w => w.WriteEndElement());
		}

		public override void WriteEntityRef(string name)
		{
			this.WriteElementOrAttributeContent(w => w.WriteEntityRef(name));
		}

		public override void WriteFullEndElement()
		{
			this.WriteEndElement(w => w.WriteFullEndElement());
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			this.WriteElementContent(w => w.WriteProcessingInstruction(name, text));
		}

		public override void WriteRaw(string data)
		{
			this.WriteElementOrAttributeContent(w => w.WriteRaw(data));
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.WriteElementOrAttributeContent(w => w.WriteRaw(buffer, index, count));
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			this.WriteAttribute(w => w.WriteStartAttribute(prefix, localName, ns),
				entryState: WriteState.Element,
				exitState: WriteState.Attribute);
		}

		public override void WriteStartDocument(bool standalone)
		{
			this.WriteStartDocument();
		}

		public override void WriteStartDocument()
		{
			this.RequireState(WriteState.Start);
			this.state = WriteState.Prolog;
			// (do not write anything)
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			try
			{
				if (this.IsInRoot)
				{
					this.ChildWriter.WriteStartElement(prefix, localName, ns);
					this.state = WriteState.Content;
				}
				else // is in prolog
				{
					this.RequireState(WriteState.Start, WriteState.Prolog);
					this.node.Clear();
					this.state = WriteState.Element;
				}
				this.depth++;
			}
			catch
			{
				this.Reset(WriteState.Error);
				throw;
			}
		}

		public override void WriteString(string text)
		{
			this.WriteElementOrAttributeContent(w => w.WriteString(text));
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			this.WriteElementOrAttributeContent(w => w.WriteSurrogateCharEntity(lowChar, highChar));
		}

		public override void WriteWhitespace(string ws)
		{
			this.WriteElementContent(w => w.WriteWhitespace(ws));
		}

		#endregion
	}
}

#endif