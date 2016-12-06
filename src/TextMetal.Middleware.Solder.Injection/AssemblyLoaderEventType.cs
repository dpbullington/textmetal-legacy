/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Middleware.Solder.Injection
{
	public enum AssemblyLoaderEventType : byte
	{
		Unknown = 0,
		Startup = 1,
		Shutdown = 2,
		Brick = 0xFF
	}
}