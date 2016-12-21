/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Ox.SSIS.ConsoleTool.Config
{
	public class OnePartName
	{
		#region Constructors/Destructors

		public OnePartName()
		{
		}

		#endregion

		#region Fields/Constants

		private string objectName;
		private ObjectType objectType;

		#endregion

		#region Properties/Indexers/Events

		public string ObjectName
		{
			get
			{
				return this.objectName;
			}
			set
			{
				this.objectName = value;
			}
		}

		public ObjectType ObjectType
		{
			get
			{
				return this.objectType;
			}
			set
			{
				this.objectType = value;
			}
		}

		#endregion

		#region Methods/Operators

		public override string ToString()
		{
			return string.Format("[{0}]", this.ObjectName);
		}

		#endregion
	}
}