/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Reflection;

using LeastViable.Common.Fascades.Utilities;

using TextMetal.Framework.Core;

namespace TextMetal.Framework.InputOutput
{
	public class NullInputMechanism : InputMechanism
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the NullInputMechanism class.
		/// </summary>
		public NullInputMechanism()
		{
		}

		#endregion

		#region Methods/Operators

		protected override Assembly CoreLoadAssembly(string assemblyName)
		{
			Assembly assembly;

			if ((object)assemblyName == null)
				throw new ArgumentNullException("assemblyName");

			if (DataTypeFascade.Instance.IsWhiteSpace(assemblyName))
				throw new ArgumentOutOfRangeException("assemblyName");

			assembly = Assembly.Load(assemblyName);

			return assembly;
		}

		protected override string CoreLoadContent(string contentName)
		{
			if ((object)contentName == null)
				throw new ArgumentNullException("contentName");

			if (DataTypeFascade.Instance.IsWhiteSpace(contentName))
				throw new ArgumentOutOfRangeException("contentName");

			return null;
		}

		protected override object CoreLoadSource(string sourceName, IDictionary<string, IList<string>> properties)
		{
			if ((object)sourceName == null)
				throw new ArgumentNullException("sourceName");

			if ((object)properties == null)
				throw new ArgumentNullException("properties");

			if (DataTypeFascade.Instance.IsWhiteSpace(sourceName))
				throw new ArgumentOutOfRangeException("sourceName");

			return DBNull.Value;
		}

		protected override ITemplateXmlObject CoreLoadTemplate(string templateName)
		{
			if ((object)templateName == null)
				throw new ArgumentNullException("templateName");

			if (DataTypeFascade.Instance.IsWhiteSpace(templateName))
				throw new ArgumentOutOfRangeException("templateName");

			return null;
		}

		#endregion
	}
}