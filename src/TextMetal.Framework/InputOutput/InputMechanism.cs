/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using TextMetal.Framework.Core;
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

		private TextReader currentTextReader;

		private bool disposed;

		#endregion

		#region Properties/Indexers/Events

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
		/// Loads an assembly by name. Assembly name semantics are implementation specific.
		/// </summary>
		/// <param name="assemblyName"> The assembly name to load. </param>
		/// <returns> An assembly object or null. </returns>
		public Assembly LoadAssembly(string assemblyName)
		{
			try
			{
				return this.CoreLoadAssembly(assemblyName);
			}
			catch (Exception ex)
			{
				throw new TextMetalException(string.Format("CoreLoadAssembly failed for assembly name '{0}'; see inner exception for details.", assemblyName), ex);
			}
		}

		/// <summary>
		/// Loads content by content name. Content name semantics are implementation specific.
		/// </summary>
		/// <param name="contentName"> The content name to load. </param>
		/// <returns> The text content or null. </returns>
		public string LoadContent(string contentName)
		{
			try
			{
				return this.CoreLoadContent(contentName);
			}
			catch (Exception ex)
			{
				throw new TextMetalException(string.Format("CoreLoadContent failed for content name '{0}'; see inner exception for details.", contentName), ex);
			}
		}

		/// <summary>
		/// Loads a source object by source name. Source name semantics are implementation specific.
		/// </summary>
		/// <param name="sourceName"> The source name to load. </param>
		/// <param name="properties"> A list of arbitrary properties (key/value pairs). </param>
		/// <returns> The source object or null. </returns>
		public object LoadSource(string sourceName, IDictionary<string, IList<string>> properties)
		{
			try
			{
				return this.CoreLoadSource(sourceName, properties);
			}
			catch (Exception ex)
			{
				throw new TextMetalException(string.Format("CoreLoadSource failed for source name '{0}'; see inner exception for details.", sourceName), ex);
			}
		}

		/// <summary>
		/// Loads an template by template name. Template name semantics are implementation specific.
		/// </summary>
		/// <param name="templateName"> The template name to load. </param>
		/// <returns> The template root object or null. </returns>
		public ITemplateXmlObject LoadTemplate(string templateName)
		{
			try
			{
				return this.CoreLoadTemplate(templateName);
			}
			catch (Exception ex)
			{
				throw new TextMetalException(string.Format("CoreLoadTemplate failed for template name '{0}'; see inner exception for details.", templateName), ex);
			}
		}

		#endregion
	}
}