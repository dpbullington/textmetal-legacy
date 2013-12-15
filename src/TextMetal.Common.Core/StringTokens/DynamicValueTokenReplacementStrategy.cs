/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;

namespace TextMetal.Common.Core.StringTokens
{
	/// <summary>
	/// Provides a dynamic token replacement strategy which executes an on-demand callback method to obtain a replacement value.
	/// </summary>
	public class DynamicValueTokenReplacementStrategy : ITokenReplacementStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the DynamicValueTokenReplacementStrategy class.
		/// </summary>
		/// <param name="method"> The callback method to evaluate during token replacement. </param>
		public DynamicValueTokenReplacementStrategy(Func<string[], object> method)
		{
			if ((object)method == null)
				throw new ArgumentNullException("method");

			this.method = method;
		}

		#endregion

		#region Fields/Constants

		private readonly Func<string[], object> method;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the callback method to evaluate during token replacement.
		/// </summary>
		public Func<string[], object> Method
		{
			get
			{
				return this.method;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Used by the token model to execute public, static methods with zero parameters in a dynamic manner.
		/// </summary>
		/// <param name="parameters"> An array of parameters in the form: assembly-qualified-type-name, method-name, [argument-value, ...] </param>
		/// <returns> The return value of the executed method. </returns>
		public static object StaticMethodResolver(string[] parameters)
		{
			int index = 0;
			Type targetType = null;
			MethodInfo methodInfo;
			object methodValue = null;

			if ((object)parameters == null)
				throw new ArgumentNullException("parameters");

			foreach (string parameter in parameters)
			{
				if (DataType.IsNullOrWhiteSpace(parameter))
					throw new InvalidOperationException(string.Format("StaticMethodResolver paramter at index '{0}' was either null or zero length.", index));

				if (index == 0) // assembly-qualified-type-name
				{
					targetType = Type.GetType(parameter, false);

					if ((object)targetType == null)
						throw new InvalidOperationException(string.Format("StaticMethodResolver paramter at index '{0}' with value '{1}' was not a valid, loadable CLR type.", 0, parameters[0]));

					index++;
				}
				else if (index == 1) // method-name
				{
					methodInfo = targetType.GetMethod(parameter, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static, null, new Type[] { }, null);

					if ((object)methodInfo == null)
						throw new InvalidOperationException(string.Format("StaticMethodResolver paramter at index '{0}' with value '{1}' was not a valid, executable method name.", 1, parameters[1]));

					methodValue = methodInfo.Invoke(null, null);
					index++;
				}
				else // argument-value[0...n]
				{
					// TODO: Add support for method parameters and type coersion and edit documentation.
					throw new NotImplementedException();
				}
			}

			return methodValue;
		}

		/// <summary>
		/// Used by the token model to get the value of public, static properties with zero parameters in a dynamic manner.
		/// </summary>
		/// <param name="parameters"> An array of parameters in the form: assembly-qualified-type-name, property-name </param>
		/// <returns> The return value of the property getter. </returns>
		public static object StaticPropertyResolver(string[] parameters)
		{
			int index = 0;
			Type targetType = null;
			PropertyInfo propertyInfo;
			object propertyValue = null;

			if ((object)parameters == null)
				throw new ArgumentNullException("parameters");

			foreach (string parameter in parameters)
			{
				if (DataType.IsNullOrWhiteSpace(parameter))
					throw new InvalidOperationException(string.Format("StaticPropertyResolver paramter at index '{0}' was either null or zero length.", index));

				if (index == 0) // assembly-qualified-type-name
				{
					targetType = Type.GetType(parameter, false);

					if ((object)targetType == null)
						throw new InvalidOperationException(string.Format("StaticPropertyResolver paramter at index '{0}' with value '{1}' was not a valid, loadable CLR type.", 0, parameters[0]));

					index++;
				}
				else if (index == 1) // property-name
				{
					propertyInfo = targetType.GetProperty(parameter, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static, null, null, new Type[] { }, null);

					if ((object)propertyInfo == null)
						throw new InvalidOperationException(string.Format("StaticPropertyResolver paramter at index '{0}' with value '{1}' was not a valid, executable property name.", 1, parameters[1]));

					if (!propertyInfo.CanRead)
						throw new InvalidOperationException(string.Format("StaticPropertyResolver paramter at index '{0}' with value '{1}' was not a valid, readable property name.", 1, parameters[1]));

					propertyValue = propertyInfo.GetValue(null, null);
					index++;
				}
				else
					throw new InvalidOperationException(string.Format("StaticPropertyResolver paramter at index '{0}' cannot be specified for properties.", index));
			}

			return propertyValue;
		}

		/// <summary>
		/// Evaluate a token using any parameters specified.
		/// </summary>
		/// <param name="parameters"> Should be null for value semantics; or a valid string array for function semantics. </param>
		/// <returns> An approapriate token replacement value. </returns>
		public object Evaluate(string[] parameters)
		{
			return this.Method(parameters);
		}

		#endregion
	}
}