/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

using TextMetal.Common.Data.Framework.PoPimp.Strategy;

namespace TextMetal.Common.Data.Framework.PoPimp
{
	public class PoPimpModelRepository : ModelRepository
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the PoPimpModelRepository class.
		/// </summary>
		public PoPimpModelRepository()
		{
		}

		#endregion

		#region Methods/Operators

		public override TModel Discard<TModel>(TModel model)
		{
			throw new NotImplementedException();
		}

		public override TModel Discard<TModel>(IUnitOfWork unitOfWork, TModel model)
		{
			throw new NotImplementedException();
		}

		public override TResponseModel ExecuteImperative<TRequestModel, TResultModel, TResponseModel>(TRequestModel requestModel)
		{
			throw new NotImplementedException();
		}

		public override TResponseModel ExecuteImperative<TRequestModel, TResultModel, TResponseModel>(IUnitOfWork unitOfWork, TRequestModel requestModel)
		{
			throw new NotImplementedException();
		}

		public override TModel Fill<TModel>(TModel model)
		{
			throw new NotImplementedException();
		}

		public override TModel Fill<TModel>(IUnitOfWork unitOfWork, TModel model)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<TModel> Find<TModel>(IModelQuery query)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<TModel> Find<TModel>(IUnitOfWork unitOfWork, IModelQuery query)
		{
			throw new NotImplementedException();
		}

		public override TModel Load<TModel>(TModel prototype)
		{
			throw new NotImplementedException();
		}

		public override TModel Load<TModel>(IUnitOfWork unitOfWork, TModel prototype)
		{
			throw new NotImplementedException();
		}

		protected override bool OnCreateNativeDatabaseFile(string databaseFilePath)
		{
			return DataSourceTagStrategyFactory.Instance.GetDataSourceTagStrategy(this.DataSourceTag).CreateNativeDatabaseFile(databaseFilePath);
		}

		protected override void OnPreProcessConnectionString(ref string connectionString)
		{
			if (!this.UseDatabaseFile)
				return;

			// {0} = this.DatabaseFilePath
			connectionString = string.Format(connectionString ?? "", this.DatabaseFilePath);
		}

		[Conditional("DEBUG")]
		protected virtual void OnProfileCommand(Type modelType, CommandType commandType, string commandText, IList<IDataParameter> commandParameters, bool executeAsCud, int thisOrThatRecordsAffected)
		{
			/* THIS METHOD SHOULD NOT BE DEFINED IN RELEASE/PRODUCTION BUILDS */

			// these are by convention in the ExecuteDictionary(...) API
			const bool COMMAND_PREPARE = false;
			/* const */
			int? COMMAND_TIMEOUT = null;
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			string value = "";
			int i;

			value += "\r\n[+++ begin trace +++]\r\n";

			value += string.Format("[Command]: Type = '{0}'; Text = '{1}'; Prepare = '{2}'; Timeout = '{3}'; Behavior = '{4}'.",
				commandType, commandText, COMMAND_PREPARE, COMMAND_TIMEOUT, COMMAND_BEHAVIOR);

			i = 0;
			foreach (IDbDataParameter commandParameter in commandParameters)
			{
				value += string.Format("\r\n\t[Parameter{0:00}]: Direction = '{1}'; ParameterName = '{2}'; IsNullable = '{3}'; Precision = '{4}'; Scale = '{5}'; Size = '{6}'; DbType = '{7}'; Value = '{8}'.",
					i++, commandParameter.Direction, commandParameter.ParameterName, commandParameter.IsNullable, commandParameter.Precision, commandParameter.Scale, commandParameter.Size, commandParameter.DbType, (object)commandParameter != null ? commandParameter.Value : "<<null>>");
			}

			value += "\r\n[+++ end trace +++]\r\n";

			Trace.WriteLine(value);
		}

		public override TModel Save<TModel>(TModel model)
		{
			throw new NotImplementedException();
		}

		public override TModel Save<TModel>(IUnitOfWork unitOfWork, TModel model)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}