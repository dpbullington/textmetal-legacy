/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Framework.Hosting.Tool
{
	public interface IToolHost : ITextMetalHost
	{
		#region Methods/Operators

		/// <summary>
		/// Provides a hosting shim between a 'tool' host and the underlying TextMetal run-time.
		/// </summary>
		/// <param name="argc"> The raw argument count passed into the host. </param>
		/// <param name="argv"> The raw arguments passed into the host. </param>
		/// <param name="args"> The parsed arguments passed into the host. </param>
		/// <param name="templateFilePath"> The file path of the input TextMetal template file to execute. </param>
		/// <param name="sourceFilePath"> The file path (or source specific URI) of the input data source to leverage. </param>
		/// <param name="baseDirectoryPath"> The root output directory path to place output arifacts (since this implementation uses file output mechanics). </param>
		/// <param name="sourceStrategyAqtn"> The assembly qualified type name for the ISourceStrategy to instantiate and execute. </param>
		/// <param name="strictMatching"> A value indicating whether to use strict matching semantics for tokens. </param>
		/// <param name="properties"> Arbitrary dictionary of string lists used to further customize the text templating process. The individual components or template files can use the properties as they see fit. </param>
		void Host(int argc, string[] argv, IDictionary<string, object> args, string templateFilePath, string sourceFilePath, string baseDirectoryPath,
			string sourceStrategyAqtn, bool strictMatching, IDictionary<string, IList<string>> properties);

		#endregion
	}
}