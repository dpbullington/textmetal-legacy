/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Solder.Runtime;

namespace TextMetal.Middleware.Solder.Utilities
{
	public static class ExtensionMethods
	{
		#region Properties/Indexers/Events

		public static IAdoNetLiteFascade AdoNetLiteLegacyInstance
		{
			get
			{
				return null;
			}
		}

		[Obsolete("Stop using this")]
		public static IDataTypeFascade DataTypeFascadeLegacyInstance
		{
			get
			{
				return AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<IDataTypeFascade>(String.Empty, true);
			}
		}

		[Obsolete("Stop using this")]
		public static IReflectionFascade ReflectionFascadeLegacyInstance
		{
			get
			{
				return AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<IReflectionFascade>(String.Empty, true);
			}
		}

		#endregion

		#region Methods/Operators

		public static T ChangeType<T>(this object value)
		{
			return AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DataTypeFascade.ChangeType<T>(value);
		}

		public static object ChangeType(this object value, Type conversionType)
		{
			return AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DataTypeFascade.ChangeType(value, conversionType);
		}

		public static string SafeToString<TValue>(this TValue value)
		{
			return AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DataTypeFascade.SafeToString<TValue>(value);
		}

		public static string SafeToString<TValue>(this TValue value, string format)
		{
			return AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DataTypeFascade.SafeToString<TValue>(value, format);
		}

		public static string SafeToString<TValue>(this TValue value, string format, string @default)
		{
			return AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DataTypeFascade.SafeToString<TValue>(value, format, @default);
		}

		public static string SafeToString<TValue>(this TValue value, string format, string @default, bool dofvisnow)
		{
			return AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DataTypeFascade.SafeToString<TValue>(value, format, @default, dofvisnow);
		}

		#endregion
	}
}