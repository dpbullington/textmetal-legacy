/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Solder.Utilities
{
	public static class UtilitiesExtensionMethods
	{
		#region Methods/Operators

		public static T ChangeType<T>(this object value)
		{
			return LegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<T>(value);
		}

		public static object ChangeType(this object value, Type conversionType)
		{
			return LegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType(value, conversionType);
		}

		public static string SafeToString<TValue>(this TValue value)
		{
			return LegacyInstanceAccessor.DataTypeFascadeLegacyInstance.SafeToString<TValue>(value);
		}

		public static string SafeToString<TValue>(this TValue value, string format)
		{
			return LegacyInstanceAccessor.DataTypeFascadeLegacyInstance.SafeToString<TValue>(value, format);
		}

		public static string SafeToString<TValue>(this TValue value, string format, string @default)
		{
			return LegacyInstanceAccessor.DataTypeFascadeLegacyInstance.SafeToString<TValue>(value, format, @default);
		}

		public static string SafeToString<TValue>(this TValue value, string format, string @default, bool dofvisnow)
		{
			return LegacyInstanceAccessor.DataTypeFascadeLegacyInstance.SafeToString<TValue>(value, format, @default, dofvisnow);
		}

		#endregion
	}
}