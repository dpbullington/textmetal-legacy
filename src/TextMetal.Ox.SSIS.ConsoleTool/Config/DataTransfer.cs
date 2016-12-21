/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Ox.SSIS.ConsoleTool.Config
{
	public class DataTransfer
	{
		#region Constructors/Destructors

		public DataTransfer()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly FourPartName destination = new FourPartName();
		private readonly List<string> excludeMemberNames = new List<string>();
		private readonly FourPartName source = new FourPartName();

		#endregion

		#region Properties/Indexers/Events

		public FourPartName Destination
		{
			get
			{
				return this.destination;
			}
		}

		public List<string> ExcludeMemberNames
		{
			get
			{
				return this.excludeMemberNames;
			}
		}

		public FourPartName Source
		{
			get
			{
				return this.source;
			}
		}

		#endregion
	}
}