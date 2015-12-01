/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Globalization;
using System.Text;

using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Framework.Naming
{
	public class SqlMetalCanonicalNaming : ICanonicalNaming
	{
		#region Constructors/Destructors

		private SqlMetalCanonicalNaming()
		{
		}

		#endregion

		#region Fields/Constants

		private static readonly SqlMetalCanonicalNaming instance = new SqlMetalCanonicalNaming();

		#endregion

		#region Properties/Indexers/Events

		public static SqlMetalCanonicalNaming Instance
		{
			get
			{
				return instance;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Gets the camel (e.g. 'myVariableName') form of a name. This method mimics SqlMetal.exe.
		/// </summary>
		/// <param name="value"> The value to which to get the camel case form. </param>
		/// <returns> The camel case, valid C# identifier form of the specified value. </returns>
		public string GetCamelCase(string value)
		{
			StringBuilder sb;
			bool flag = true;

			if ((object)value == null)
				throw new ArgumentNullException("value");

			value = StandardCanonicalNaming.GetValidCSharpIdentifier(value);

			if (value.Length < 1)
				return value;

			sb = new StringBuilder();

			for (int index = 0; index < value.Length; ++index)
			{
				char ch = value[index];

				if ((int)ch >= 97 && (int)ch <= 122 ||
					(int)ch >= 65 && (int)ch <= 90 ||
					((int)ch >= 48 && (int)ch <= 57 ||
					(int)ch == 95))
				{
					if (flag)
						ch = char.ToUpper(ch, CultureInfo.InvariantCulture);

					flag = false;
					sb.Append(ch);
				}
				else
				{
					flag = true;
					sb.Append(ch);
				}
			}

			value = sb.ToString();
			return value;
		}

		/// <summary>
		/// Gets the contant (e.g. 'MY_VARIABLE_NAME') form of a name. This method adds underscores at case change boundaries.
		/// </summary>
		/// <param name="value"> The value to which to get the constant case form. </param>
		/// <returns> The constant case, valid C# identifier form of the specified value. </returns>
		public string GetConstantCase(string value)
		{
			return StandardCanonicalNaming.Instance.GetConstantCase(value);
		}

		/// <summary>
		/// SqlMetal specific.
		/// </summary>
		/// <param name="schemaName"> </param>
		/// <param name="objectName"> </param>
		/// <returns> </returns>
		public string GetObjectNamePascalCase(string schemaName, string objectName)
		{
			string value;
			const string DBO_SCHEMA_NAME = "dbo";

			if ((object)schemaName == null)
				throw new ArgumentNullException("schemaName");

			if ((object)objectName == null)
				throw new ArgumentNullException("objectName");

			if (!DataTypeFascade.Instance.IsNullOrWhiteSpace(schemaName) && schemaName.ToLower() != DBO_SCHEMA_NAME)
				value = string.Format("{0}_{1}", this.GetPascalCase(schemaName), this.GetPascalCase(objectName));
			else
				value = string.Format("{0}", this.GetPascalCase(objectName));

			return value;
		}

		/// <summary>
		/// Gets the Pascal (e.g. 'MyVariableName') form of a name. This method mimics SqlMetal.exe.
		/// </summary>
		/// <param name="value"> The value to which to get the Pascal case form. </param>
		/// <returns> The Pascal case, valid C# identifier form of the specified value. </returns>
		public string GetPascalCase(string value)
		{
			StringBuilder sb;

			if ((object)value == null)
				throw new ArgumentNullException("value");

			value = StandardCanonicalNaming.GetValidCSharpIdentifier(value);

			if (value.Length < 1)
				return value;

			value = this.GetCamelCase(value);
			sb = new StringBuilder(value);

			sb[0] = char.ToUpper(sb[0]);

			return sb.ToString();
		}

		/// <summary>
		/// Gets the plural (e.g. 'myVariableNames') form of a name. This method uses basic stemming.
		/// </summary>
		/// <param name="value"> The value to which to get the plural form. </param>
		/// <returns> The plural, valid C# identifier form of the specified value. </returns>
		public string GetPluralForm(string value)
		{
			return StandardCanonicalNaming.Instance.GetPluralForm(value);
		}

		/// <summary>
		/// Gets the singular (e.g. 'myVariableName') form of a name. This method uses basic stemming.
		/// </summary>
		/// <param name="value"> The value to which to get the singular form. </param>
		/// <returns> The singular, valid C# identifier form of the specified value. </returns>
		public string GetSingularForm(string value)
		{
			return StandardCanonicalNaming.Instance.GetSingularForm(value);
		}

		#endregion
	}
}