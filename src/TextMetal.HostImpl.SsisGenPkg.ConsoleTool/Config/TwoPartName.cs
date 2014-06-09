/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.HostImpl.SsisGenPkg.ConsoleTool.Config
{
	public class TwoPartName : OnePartName
	{
		#region Constructors/Destructors

		public TwoPartName()
		{
		}

		#endregion

		#region Fields/Constants

		private string schemaName;

		#endregion

		#region Properties/Indexers/Events

		public string SchemaName
		{
			get
			{
				return this.schemaName;
			}
			set
			{
				this.schemaName = value;
			}
		}

		#endregion

		#region Methods/Operators

		public override string ToString()
		{
			return string.Format("[{0}].[{1}]", this.SchemaName, this.ObjectName);
		}

		#endregion
	}
}