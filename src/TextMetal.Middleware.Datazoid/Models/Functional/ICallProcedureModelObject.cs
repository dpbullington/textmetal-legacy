/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Datazoid.Models.Functional
{
	/// <summary>
	/// Provides a contract for call procedure model objects (procedure, function, packages, etc.).
	/// </summary>
	public interface ICallProcedureModelObject : IModelObject, IValidate
	{
	}
}