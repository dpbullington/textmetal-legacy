/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.HostImpl.SsisGenPkg.ConsoleTool.Config
{
	public class FourPartName : ThreePartName
	{
		#region Constructors/Destructors

		public FourPartName()
		{
		}

		#endregion

		#region Fields/Constants

		private string serverName;

		#endregion

		#region Properties/Indexers/Events

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
			set
			{
				this.serverName = value;
			}
		}

		#endregion

		#region Methods/Operators

		public override string ToString()
		{
			return string.Format("[{0}].[{1}].[{2}].[{3}]", this.ServerName, this.DatabaseName, this.SchemaName, this.ObjectName);
		}

		#endregion
	}
}