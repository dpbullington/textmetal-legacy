/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Tactics
{
	public interface ITacticParameter
	{
		#region Properties/Indexers/Events

		IDictionary<string, object> ParameterFixups
		{
			get;
		}

		DbType ParameterDbType
		{
			get;
			set;
		}

		ParameterDirection ParameterDirection
		{
			get;
			set;
		}

		string ParameterName
		{
			get;
			set;
		}

		bool ParameterNullable
		{
			get;
			set;
		}

		byte ParameterPrecision
		{
			get;
			set;
		}

		byte ParameterScale
		{
			get;
			set;
		}

		int ParameterSize
		{
			get;
			set;
		}

		object ParameterValue
		{
			get;
			set;
		}

		#endregion
	}
}