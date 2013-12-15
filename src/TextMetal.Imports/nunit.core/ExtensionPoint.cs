// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;

using NUnit.Core.Extensibility;

namespace NUnit.Core
{
	public class ExtensionsCollection : IEnumerable
	{
		#region Constructors/Destructors

		public ExtensionsCollection()
			: this(1)
		{
		}

		public ExtensionsCollection(int levels)
		{
			if (levels < 1)
				levels = 1;
			else if (levels > MAX_LEVELS)
				levels = MAX_LEVELS;

			this.lists = new ArrayList[levels];
		}

		#endregion

		#region Fields/Constants

		private static readonly int DEFAULT_LEVEL = 0;
		private static readonly int MAX_LEVELS = 10;

		private ArrayList[] lists;

		#endregion

		#region Properties/Indexers/Events

		public int Levels
		{
			get
			{
				return this.lists.Length;
			}
		}

		#endregion

		#region Methods/Operators

		public void Add(object extension)
		{
			this.Add(extension, DEFAULT_LEVEL);
		}

		public void Add(object extension, int level)
		{
			if (level < 0 || level >= this.lists.Length)
			{
				throw new ArgumentOutOfRangeException("level", level,
					"Value must be between 0 and " + this.lists.Length);
			}

			if (this.lists[level] == null)
				this.lists[level] = new ArrayList();

			this.lists[level].Insert(0, extension);
		}

		public IEnumerator GetEnumerator()
		{
			return new ExtensionsEnumerator(this.lists);
		}

		public void Remove(object extension)
		{
			foreach (IList list in this.lists)
				list.Remove(extension);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		public class ExtensionsEnumerator : IEnumerator
		{
			#region Constructors/Destructors

			public ExtensionsEnumerator(ArrayList[] lists)
			{
				this.lists = lists;
				this.Reset();
			}

			#endregion

			#region Fields/Constants

			private int currentLevel;
			private IEnumerator listEnum;
			private ArrayList[] lists;

			#endregion

			#region Properties/Indexers/Events

			public object Current
			{
				get
				{
					return this.listEnum.Current;
				}
			}

			#endregion

			#region Methods/Operators

			public bool MoveNext()
			{
				if (this.listEnum != null && this.listEnum.MoveNext())
					return true;

				while (++this.currentLevel < this.lists.Length)
				{
					IList list = this.lists[this.currentLevel];
					if (list != null)
					{
						this.listEnum = list.GetEnumerator();
						if (this.listEnum.MoveNext())
							return true;
					}
				}

				return false;
			}

			public void Reset()
			{
				this.listEnum = null;
				this.currentLevel = -1;
			}

			#endregion
		}

		#endregion
	}

	/// <summary>
	/// ExtensionPoint is used as a base class for all
	/// extension points.
	/// </summary>
	public abstract class ExtensionPoint : IExtensionPoint
	{
		#region Constructors/Destructors

		public ExtensionPoint(string name, IExtensionHost host)
			: this(name, host, 0)
		{
		}

		public ExtensionPoint(string name, IExtensionHost host, int priorityLevels)
		{
			this.name = name;
			this.host = host;
			this.extensions = new ExtensionsCollection(priorityLevels);
		}

		#endregion

		#region Fields/Constants

		private readonly ExtensionsCollection extensions;
		private readonly IExtensionHost host;
		private readonly string name;

		#endregion

		#region Properties/Indexers/Events

		protected IEnumerable Extensions
		{
			get
			{
				return this.extensions;
			}
		}

		/// <summary>
		/// Get the host that provides this extension point
		/// </summary>
		public IExtensionHost Host
		{
			get
			{
				return this.host;
			}
		}

		/// <summary>
		/// Get the name of this extension point
		/// </summary>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Install an extension at this extension point. If the
		/// extension object does not meet the requirements for
		/// this extension point, an exception is thrown.
		/// </summary>
		/// <param name="extension"> The extension to install </param>
		public void Install(object extension)
		{
			if (!this.IsValidExtension(extension))
			{
				throw new ArgumentException(
					extension.GetType().FullName + " is not {0} extension point", "extension");
			}

			this.extensions.Add(extension);
		}

		/// <summary>
		/// Install an extension at this extension point specifying
		/// an integer priority value for the extension.If the
		/// extension object does not meet the requirements for
		/// this extension point, or if the extension point does
		/// not support the requested priority level, an exception
		/// is thrown.
		/// </summary>
		/// <param name="extension"> The extension to install </param>
		/// <param name="priority"> The priority level for this extension </param>
		public void Install(object extension, int priority)
		{
			if (!this.IsValidExtension(extension))
			{
				throw new ArgumentException(
					extension.GetType().FullName + " is not {0} extension point", "extension");
			}

			if (priority < 0 || priority >= this.extensions.Levels)
				throw new ArgumentException("Priority value not supported", "priority");

			this.extensions.Add(extension, priority);
		}

		protected abstract bool IsValidExtension(object extension);

		/// <summary>
		/// Removes an extension from this extension point. If the
		/// extension object is not present, the method returns
		/// without error.
		/// </summary>
		/// <param name="extension"> </param>
		public void Remove(object extension)
		{
			this.extensions.Remove(extension);
		}

		#endregion
	}
}