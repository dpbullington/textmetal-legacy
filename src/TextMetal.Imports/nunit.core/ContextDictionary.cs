// ****************************************************************
// Copyright 2011, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;

namespace NUnit.Core
{
	[Serializable]
	public class ContextDictionary : MarshalByRefObject, IDictionary, ILogicalThreadAffinative
	{
		#region Constructors/Destructors

		public ContextDictionary()
		{
		}

		public ContextDictionary(TestExecutionContext context)
		{
			this._context = context;
		}

		#endregion

		#region Fields/Constants

		private readonly Hashtable _storage = new Hashtable();
		internal TestExecutionContext _context;

		#endregion

		#region Properties/Indexers/Events

		public object this[object key]
		{
			get
			{
				// Get Result values dynamically, since
				// they may change as execution proceeds
				switch (key as string)
				{
					case "Test.Name":
						return this._context.CurrentTest.TestName.Name;
					case "Test.FullName":
						return this._context.CurrentTest.TestName.FullName;
					case "Test.Properties":
						return this._context.CurrentTest.Properties;
					case "Result.State":
						return (int)this._context.CurrentResult.ResultState;
					case "TestDirectory":
						return AssemblyHelper.GetDirectoryName(this._context.CurrentTest.FixtureType.Assembly);
					case "WorkDirectory":
						return this._context.TestPackage.Settings.Contains("WorkDirectory")
							? this._context.TestPackage.Settings["WorkDirectory"]
							: Environment.CurrentDirectory;
					default:
						return this._storage[key];
				}
			}
			set
			{
				this._storage[key] = value;
			}
		}

		int ICollection.Count
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region Methods/Operators

		void IDictionary.Add(object key, object value)
		{
			throw new NotImplementedException();
		}

		void IDictionary.Clear()
		{
			throw new NotImplementedException();
		}

		bool IDictionary.Contains(object key)
		{
			throw new NotImplementedException();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		void IDictionary.Remove(object key)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}