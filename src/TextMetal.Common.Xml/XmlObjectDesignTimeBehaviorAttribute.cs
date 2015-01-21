/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Drawing;
using System.IO;

using TextMetal.Common.Core;

namespace TextMetal.Common.Xml
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class XmlObjectDesignTimeBehaviorAttribute : Attribute
	{
		#region Constructors/Destructors

		public XmlObjectDesignTimeBehaviorAttribute(string description, bool showInToolbox, Type toolboxImageResourceType, string toolboxImageResourceName)
		{
			if ((object)description == null)
				throw new ArgumentNullException("description");

			if ((object)toolboxImageResourceType == null)
				throw new ArgumentNullException("toolboxImageResourceType");

			if ((object)toolboxImageResourceName == null)
				throw new ArgumentNullException("toolboxImageResourceName");

			if (DataType.Instance.IsWhiteSpace(description))
				throw new ArgumentOutOfRangeException("description");

			if (DataType.Instance.IsWhiteSpace(toolboxImageResourceName))
				throw new ArgumentOutOfRangeException("toolboxImageResourceName");

			this.description = description;
			this.showInToolbox = showInToolbox;
			this.toolboxImageResourceType = toolboxImageResourceType;
			this.toolboxImageResourceName = toolboxImageResourceName;
		}

		#endregion

		#region Fields/Constants

		private readonly string description;
		private readonly bool showInToolbox;
		private readonly string toolboxImageResourceName;
		private readonly Type toolboxImageResourceType;

		#endregion

		#region Properties/Indexers/Events

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		public bool ShowInToolbox
		{
			get
			{
				return this.showInToolbox;
			}
		}

		private string ToolboxImageResourceName
		{
			get
			{
				return this.toolboxImageResourceName;
			}
		}

		private Type ToolboxImageResourceType
		{
			get
			{
				return this.toolboxImageResourceType;
			}
		}

		#endregion

		#region Methods/Operators

		public Image GetToolboxImage()
		{
			Stream stream;
			Image image;

			stream = this.ToolboxImageResourceType.Assembly.GetManifestResourceStream(this.ToolboxImageResourceName);

			if ((object)stream == null)
				return null;

			image = Image.FromStream(stream);
			// DO NOT DISPOSE (owner cleans up)

			return image;
		}

		#endregion
	}
}