/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Expressions
{
	public sealed class VoidExpression : IExpression
	{
		#region Constructors/Destructors

		private VoidExpression()
		{
		}

		#endregion

		#region Fields/Constants

		private static readonly VoidExpression instance = new VoidExpression();

		#endregion

		#region Properties/Indexers/Events

		public static VoidExpression Instance
		{
			get
			{
				return instance;
			}
		}

		#endregion
	}
}