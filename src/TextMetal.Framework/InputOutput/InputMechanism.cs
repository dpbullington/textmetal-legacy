/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using TextMetal.Framework.Template;

namespace TextMetal.Framework.InputOutput
{
	public abstract class InputMechanism : IInputMechanism
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the InputMechanism class.
		/// </summary>
		protected InputMechanism()
		{
		}

		#endregion

		#region Fields/Constants

		private bool disposed;
		private TextReader currentTextReader;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets a value indicating whether the current instance has been disposed.
		/// </summary>
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

		protected abstract Assembly CoreLoadAssembly(string assemblyName);

		protected abstract string CoreLoadContent(string contentName);

		protected abstract object CoreLoadSource(string sourceName, IDictionary<string, IList<string>> properties);

		protected abstract ITemplateXmlObject CoreLoadTemplate(string templateName);

		/// <summary>
		/// Dispose of the data source transaction.
		/// </summary>
		public void Dispose()
		{
			if (this.Disposed)
				return;

			try
			{
				if ((object)this.CurrentTextReader != null)
				{
					this.CurrentTextReader.Dispose();
					this.CurrentTextReader = null;
				}
			}
			finally
			{
				this.Disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Gets the current text reader instance.
		/// </summary>
		public TextReader CurrentTextReader
		{
			get
			{
				return this.currentTextReader ?? Console.In;
			}
			private set
			{
				this.currentTextReader = value;
			}
		}

		/// <summary>
		/// Loads an assembly by name. Assembly name semantics are implementation specific.
		/// </summary>
		/// <param name="assemblyName"> The assembly name to load. </param>
		/// <returns> An assembly object or null. </returns>
		public Assembly LoadAssembly(string assemblyName)
		{
			return this.CoreLoadAssembly(assemblyName);
		}

		/// <summary>
		/// Loads content by content name. Content name semantics are implementation specific.
		/// </summary>
		/// <param name="contentName"> The content name to load. </param>
		/// <returns> The text content or null. </returns>
		public string LoadContent(string contentName)
		{
			return this.CoreLoadContent(contentName);
		}

		/// <summary>
		/// Loads a source object by source name. Source name semantics are implementation specific.
		/// </summary>
		/// <param name="sourceName"> The source name to load. </param>
		/// <param name="properties"> A list of arbitrary properties (key/value pairs). </param>
		/// <returns> The source object or null. </returns>
		public object LoadSource(string sourceName, IDictionary<string, IList<string>> properties)
		{
			return this.CoreLoadSource(sourceName, properties);
		}

		/// <summary>
		/// Loads an template by template name. Template name semantics are implementation specific.
		/// </summary>
		/// <param name="templateName"> The template name to load. </param>
		/// <returns> The template root object or null. </returns>
		public ITemplateXmlObject LoadTemplate(string templateName)
		{
			return this.CoreLoadTemplate(templateName);
		}

		#endregion
	}
}