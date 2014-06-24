/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Xml.Serialization;

namespace TextMetal.Framework.SourceModel.DatabaseSchema
{
	[Serializable]
	public class Trigger
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the Trigger class.
		/// </summary>
		public Trigger()
		{
		}

		#endregion

		#region Fields/Constants

		private bool isClrTrigger;
		private bool isInsteadOfTrigger;
		private bool isTriggerDisabled;
		private bool isTriggerNotForReplication;
		private int triggerId;

		private string triggerName;
		private string triggerNameCamelCase;
		private string triggerNameConstantCase;
		private string triggerNamePascalCase;
		private string triggerNamePluralCamelCase;
		private string triggerNamePluralConstantCase;
		private string triggerNamePluralPascalCase;
		private string triggerNameSingularCamelCase;
		private string triggerNameSingularConstantCase;
		private string triggerNameSingularPascalCase;
		private string triggerNameSqlMetalCamelCase;
		private string triggerNameSqlMetalPascalCase;
		private string triggerNameSqlMetalPascalCase1;
		private string triggerNameSqlMetalPluralCamelCase;
		private string triggerNameSqlMetalPluralPascalCase;
		private string triggerNameSqlMetalSingularCamelCase;
		private string triggerNameSqlMetalSingularPascalCase;

		#endregion

		#region Properties/Indexers/Events

		[XmlAttribute]
		public bool IsClrTrigger
		{
			get
			{
				return this.isClrTrigger;
			}
			set
			{
				this.isClrTrigger = value;
			}
		}

		[XmlAttribute]
		public bool IsInsteadOfTrigger
		{
			get
			{
				return this.isInsteadOfTrigger;
			}
			set
			{
				this.isInsteadOfTrigger = value;
			}
		}

		[XmlAttribute]
		public bool IsTriggerDisabled
		{
			get
			{
				return this.isTriggerDisabled;
			}
			set
			{
				this.isTriggerDisabled = value;
			}
		}

		[XmlAttribute]
		public bool IsTriggerNotForReplication
		{
			get
			{
				return this.isTriggerNotForReplication;
			}
			set
			{
				this.isTriggerNotForReplication = value;
			}
		}

		[XmlAttribute]
		public int TriggerId
		{
			get
			{
				return this.triggerId;
			}
			set
			{
				this.triggerId = value;
			}
		}

		[XmlAttribute]
		public string TriggerName
		{
			get
			{
				return this.triggerName;
			}
			set
			{
				this.triggerName = value;
			}
		}

		[XmlAttribute]
		public string TriggerNameCamelCase
		{
			get
			{
				return this.triggerNameCamelCase;
			}
			set
			{
				this.triggerNameCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TriggerNameConstantCase
		{
			get
			{
				return this.triggerNameConstantCase;
			}
			set
			{
				this.triggerNameConstantCase = value;
			}
		}

		[XmlAttribute]
		public string TriggerNamePascalCase
		{
			get
			{
				return this.triggerNamePascalCase;
			}
			set
			{
				this.triggerNamePascalCase = value;
			}
		}

		[XmlAttribute]
		public string TriggerNamePluralCamelCase
		{
			get
			{
				return this.triggerNamePluralCamelCase;
			}
			set
			{
				this.triggerNamePluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TriggerNamePluralConstantCase
		{
			get
			{
				return this.triggerNamePluralConstantCase;
			}
			set
			{
				this.triggerNamePluralConstantCase = value;
			}
		}

		[XmlAttribute]
		public string TriggerNamePluralPascalCase
		{
			get
			{
				return this.triggerNamePluralPascalCase;
			}
			set
			{
				this.triggerNamePluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TriggerNameSingularCamelCase
		{
			get
			{
				return this.triggerNameSingularCamelCase;
			}
			set
			{
				this.triggerNameSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TriggerNameSingularConstantCase
		{
			get
			{
				return this.triggerNameSingularConstantCase;
			}
			set
			{
				this.triggerNameSingularConstantCase = value;
			}
		}

		[XmlAttribute]
		public string TriggerNameSingularPascalCase
		{
			get
			{
				return this.triggerNameSingularPascalCase;
			}
			set
			{
				this.triggerNameSingularPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TriggerNameSqlMetalCamelCase
		{
			get
			{
				return this.triggerNameSqlMetalCamelCase;
			}
			set
			{
				this.triggerNameSqlMetalCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TriggerNameSqlMetalPascalCase
		{
			get
			{
				return this.triggerNameSqlMetalPascalCase1;
			}
			set
			{
				this.triggerNameSqlMetalPascalCase1 = value;
			}
		}

		[XmlAttribute]
		public string TriggerNameSqlMetalPluralCamelCase
		{
			get
			{
				return this.triggerNameSqlMetalPluralCamelCase;
			}
			set
			{
				this.triggerNameSqlMetalPluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TriggerNameSqlMetalPluralPascalCase
		{
			get
			{
				return this.triggerNameSqlMetalPluralPascalCase;
			}
			set
			{
				this.triggerNameSqlMetalPluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TriggerNameSqlMetalSingularCamelCase
		{
			get
			{
				return this.triggerNameSqlMetalSingularCamelCase;
			}
			set
			{
				this.triggerNameSqlMetalSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TriggerNameSqlMetalSingularPascalCase
		{
			get
			{
				return this.triggerNameSqlMetalSingularPascalCase;
			}
			set
			{
				this.triggerNameSqlMetalSingularPascalCase = value;
			}
		}

		#endregion
	}
}