/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Middleware.Datazoid.Models.Functional
{
	public interface IResultsetModelObject : IModelObject
	{
		#region Properties/Indexers/Events

		int Index
		{
			get;
			set;
		}

		#endregion
	}
}