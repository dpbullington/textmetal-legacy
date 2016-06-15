/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Reflection;

using TextMetal.Framework.Template;
using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Utilities;

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
			return null;
		}

		protected override string CoreLoadContent(string contentName)
		{
			if ((object)contentName == null)
				throw new ArgumentNullException(nameof(contentName));

			if (LegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsWhiteSpace(contentName))
				throw new ArgumentOutOfRangeException(nameof(contentName));

			return null;
		}

		protected override object CoreLoadSource(string sourceName, IDictionary<string, IList<string>> properties)
		{
			if ((object)sourceName == null)
				throw new ArgumentNullException(nameof(sourceName));

			if ((object)properties == null)
				throw new ArgumentNullException(nameof(properties));

			if (LegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsWhiteSpace(sourceName))
				throw new ArgumentOutOfRangeException(nameof(sourceName));

			return DBNull.Value;
		}

		protected override ITemplateXmlObject CoreLoadTemplate(string templateName)
		{
			if ((object)templateName == null)
				throw new ArgumentNullException(nameof(templateName));

			if (LegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsWhiteSpace(templateName))
				throw new ArgumentOutOfRangeException(nameof(templateName));

			return null;
		}

		#endregion
	}
}