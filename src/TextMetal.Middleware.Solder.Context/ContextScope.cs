/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Middleware.Solder.Context
{
	public enum ContextScope
	{
		/// <summary>
		/// Unknowm, undefined, or invalid scope.
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Globally visble, static semantics, implicitly not thread-safe.
		/// </summary>
		GlobalStaticUnsafe,

		/// <summary>
		/// Globally visble, dispatcher-safe, (thread-safety ??); commonly used with windowing frameworks that prevent cross-thread control access.
		/// </summary>
		GlobalDispatchSafe,

		/// <summary>
		/// Local method call stack frame visible, implciitly thread-safe.
		/// </summary>
		LocalFrameSafe,

		/// <summary>
		/// Local thread visible and thread-safe obviously.
		/// </summary>
		LocalThreadSafe,

		/// <summary>
		/// Local logical async execution flow visible and async-safe (perhaps thread-safe ??).
		/// </summary>
		LocalAsyncSafe,

		/// <summary>
		/// Local request visble, context-agility-safe, and not thread-safe; commonly used with web server frameworks that are not guaranteed to be exhibit thread affinity (e.g. context can be forklifted thread to thread under load).
		/// </summary>
		LocalRequestSafe
	}
}