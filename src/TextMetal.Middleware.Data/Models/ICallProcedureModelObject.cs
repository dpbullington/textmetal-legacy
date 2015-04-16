/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Common.ValueObjects;

namespace TextMetal.Middleware.Data.Models
{
	/// <summary>
	/// Provides a contract for call procedure model objects (procedure, function, packages, etc.).
	/// </summary>
	public interface ICallProcedureModelObject : IModelObject, IValidate
	{
	}
}