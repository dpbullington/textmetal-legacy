/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

/*
	Import Sources:
	Unified framework
		Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Actual frameworks
		Copyright © others: see license files in source or imports raw source.
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
[assembly: AssemblyCopyright("©2002-2014 Daniel Bullington (dpbullington@gmail.com)")]
[assembly: AssemblyDescription("Distributed under the MIT license:\r\nhttp://www.opensource.org/licenses/mit-license.php")]
[assembly: AssemblyTrademark("π")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("6.0.0.*")]
[assembly: AssemblyFileVersion("6.0.0.0")]
[assembly: AssemblyInformationalVersion("2014.04.23/unstable")]
[assembly: AssemblyDelaySign(false)]
[assembly: ComVisible(false)]

#if DEFINE_CLR_VERSION_20
// this causes NUnit to die in CLR 4.0

[assembly: AllowPartiallyTrustedCallers]
#endif