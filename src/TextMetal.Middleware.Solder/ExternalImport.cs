/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;

namespace TextMetal.Middleware.Solder
{
	/// <summary>
	/// This code should remain in sync with:
	/// .\src\TextMetal.Middleware.Common\Fascades\Utilities\ReflectionFascade.cs
	/// </summary>
	internal class ExternalImport
	{
		#region Methods/Operators

		public static TAttribute[] GetAllAttributes<TAttribute>(ICustomAttributeProvider target)
			where TAttribute : Attribute
		{
			object[] attributes;

			if ((object)target == null)
				throw new ArgumentNullException("target");

			attributes = target.GetCustomAttributes(typeof(TAttribute), true);

			if ((object)attributes != null && attributes.Length != 0 && attributes is TAttribute[])
				return attributes as TAttribute[];
			else
				return null;
		}

		public static TAttribute GetOneAttribute<TAttribute>(ICustomAttributeProvider target)
			where TAttribute : Attribute
		{
			TAttribute[] attributes;
			Type attributeType, targetType;

			if ((object)target == null)
				throw new ArgumentNullException("target");

			attributeType = typeof(TAttribute);
			targetType = target.GetType();

			attributes = GetAllAttributes<TAttribute>(target);

			if ((object)attributes == null || attributes.Length == 0)
				return null;
			else if (attributes.Length > 1)
				throw new InvalidOperationException(string.Format("Multiple custom attributes of type '{0}' are defined on type '{1}'.", attributeType.FullName, targetType.FullName));
			else
				return attributes[0];
		}

		#endregion
	}
}