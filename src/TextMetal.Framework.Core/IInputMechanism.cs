/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace TextMetal.Framework.Core
{
	/// <summary>
	/// Provides for input mechanics.
	/// </summary>
	public interface IInputMechanism : IDisposable
	{
		#region Methods/Operators

		/// <summary>
		/// Loads an assembly by name. Assembly name semantics are implementation specific.
		/// </summary>
		/// <param name="assemblyName"> The assembly name to load. </param>
		/// <returns> An assembly object or null. </returns>
		Assembly LoadAssembly(string assemblyName);

		/// <summary>
		/// Loads content by content name. Content name semantics are implementation specific.
		/// </summary>
		/// <param name="contentName"> The content name to load. </param>
		/// <returns> The text content or null. </returns>
		string LoadContent(string contentName);

		/// <summary>
		/// Loads a source object by source name. Source name semantics are implementation specific.
		/// </summary>
		/// <param name="sourceName"> The source name to load. </param>
		/// <param name="properties"> A list of arbitrary properties (key/value pairs). </param>
		/// <returns> The source object or null. </returns>
		object LoadSource(string sourceName, IDictionary<string, IList<string>> properties);

		/// <summary>
		/// Loads an template by template name. Template name semantics are implementation specific.
		/// </summary>
		/// <param name="templateName"> The template name to load. </param>
		/// <returns> The template root object or null. </returns>
		ITemplateXmlObject LoadTemplate(string templateName);

		#endregion
	}
}