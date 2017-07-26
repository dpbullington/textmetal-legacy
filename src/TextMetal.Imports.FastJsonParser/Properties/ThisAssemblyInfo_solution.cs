/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Reflection;
using System.Runtime.InteropServices;

#if DEBUG

[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyCompany("textmetal.com")]
[assembly: AssemblyProduct("TextMetal")]
[assembly: AssemblyCopyright("©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)")]
[assembly: AssemblyDescription("Distributed under the MIT license:\r\nhttp://www.opensource.org/licenses/mit-license.php")]
[assembly: AssemblyTrademark("π")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("9.0.0.0")]
[assembly: AssemblyFileVersion("9.0.0.0")]
[assembly: AssemblyInformationalVersion("2016.12.05/preview")]
[assembly: AssemblyDelaySign(false)]
[assembly: ComVisible(false)]