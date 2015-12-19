/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Expressions
{
	/// <summary>
	/// Represents a symbol name.
	/// </summary>
	[Serializable]
	public sealed class SymbolName : IExpression
	{
		#region Constructors/Destructors

		public SymbolName(string name)
		{
			if ((object)name == null)
				throw new ArgumentNullException(nameof(name));

			this.name = name;
		}

		#endregion

		#region Fields/Constants

		private readonly string name;

		#endregion

		#region Properties/Indexers/Events

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		#endregion
	}
}