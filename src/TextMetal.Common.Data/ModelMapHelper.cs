/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data
{
	/// <summary>
	/// Provides static helper and/or extension methods for ModelMap O/RM.
	/// </summary>
	public static class ModelMapHelper
	{
		#region Methods/Operators

		public static TResponse ExecuteReReRe<TRequest, TResult, TResponse>(this IUnitOfWork unitOfWork, TRequest request, CommandType commandType, string commandText, IList<IDataParameter> commandParameters, bool executeAsCud, int thisOrThatRecordsAffected, Action<IDictionary<string, object>, TResult> mapToCallback)
			where TRequest : class, new()
			where TResult : class, new()
			where TResponse : class, new()
		{
			TResponse response;
			TResult result;
			int recordsAffected;
			IList<IDictionary<string, object>> results;
			IList<TResult> list = null;
			object value;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)request == null)
				throw new ArgumentNullException("request");

			if ((object)commandText == null)
				throw new ArgumentNullException("commandText");

			if ((object)commandParameters == null)
				throw new ArgumentNullException("commandParameters");

			if ((object)mapToCallback == null)
				throw new ArgumentNullException("mapToCallback");

			if (DataType.IsNullOrWhiteSpace(commandText))
				throw new ArgumentOutOfRangeException("commandText");

			results = unitOfWork.ExecuteDictionary(commandType, commandText, commandParameters, out recordsAffected);

			if (executeAsCud && recordsAffected <= thisOrThatRecordsAffected)
			{
				// concurrency failure
				unitOfWork.Divergent();
				throw new InvalidOperationException(string.Format("Data concurrency failure occurred during a CUD request-result-response execution; expected records affected '{0}', actual records affected '{1}'.", thisOrThatRecordsAffected, recordsAffected));
			}
			else if (!executeAsCud && recordsAffected != thisOrThatRecordsAffected)
			{
				// idempotency failure
				unitOfWork.Divergent();
				throw new InvalidOperationException(string.Format("Data concurrency failure occurred during a non-CUD request-result-response execution; expected records affected '{0}', actual records affected '{1}'.", thisOrThatRecordsAffected, recordsAffected));
			}

			if ((object)results == null)
				throw new InvalidOperationException(string.Format("Results collection was invalid."));

			response = new TResponse();

			if (Reflexion.GetLogicalPropertyValue(response, "Results", out value))
				list = value as IList<TResult>;

			if ((object)list != null)
				list.Clear();

			foreach (IDictionary<string, object> _result in results)
			{
				result = new TResult();

				mapToCallback(_result, result);

				if ((object)list != null)
					list.Add(result);
			}

			return response;
		}

		public static TModel FetchModel<TModel>(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IList<IDataParameter> commandParameters, int queryExpectedRecordsAffected, Action<IDictionary<string, object>, TModel> mapToCallback)
			where TModel : class, new()
		{
			TModel model;
			int recordsAffected;
			IList<IDictionary<string, object>> results;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)commandText == null)
				throw new ArgumentNullException("commandText");

			if ((object)commandParameters == null)
				throw new ArgumentNullException("commandParameters");

			if ((object)mapToCallback == null)
				throw new ArgumentNullException("mapToCallback");

			if (DataType.IsNullOrWhiteSpace(commandText))
				throw new ArgumentOutOfRangeException("commandText");

			results = unitOfWork.ExecuteDictionary(commandType, commandText, commandParameters, out recordsAffected);

			if (recordsAffected != queryExpectedRecordsAffected)
			{
				// idempotency failure
				unitOfWork.Divergent();
				throw new InvalidOperationException(string.Format("Data concurrency failure occurred during a fetch-model execution; expected records affected '{0}', actual records affected '{1}'.", queryExpectedRecordsAffected, recordsAffected));
			}

			if ((object)results == null)
				throw new InvalidOperationException(string.Format("Results collection was invalid."));

			if (results.Count != 1)
				return null;

			model = new TModel();

			mapToCallback(results[0], model);

			return model;
		}

		/// <summary>
		/// Allows for easy scalar query execution over a unitOfWork.
		/// </summary>
		/// <typeparam name="TValue"> The scalar type. </typeparam>
		/// <param name="unitOfWork"> The target unitOfWork. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <returns> The scalar value (with type conversion) or null. </returns>
		public static TValue FetchScalar<TValue>(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<IDataParameter> commandParameters)
		{
			int recordsAffected;
			IList<IDictionary<string, object>> results;
			object dbValue;

			results = unitOfWork.ExecuteDictionary(commandType, commandText, commandParameters, out recordsAffected);

			if ((object)results == null || results.Count != 1 || results[0].Count != 1)
				return default(TValue);

			dbValue = results[0][results[0].Keys.First()];

			return dbValue.ChangeType<TValue>();
		}

		public static void FillModel<TModel>(this IUnitOfWork unitOfWork, TModel model, CommandType commandType, string commandText, IList<IDataParameter> commandParameters, int queryExpectedRecordsAffected, Action<IDictionary<string, object>, TModel> mapToCallback)
			where TModel : class, new()
		{
			int recordsAffected;
			IList<IDictionary<string, object>> results;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			if ((object)commandText == null)
				throw new ArgumentNullException("commandText");

			if ((object)commandParameters == null)
				throw new ArgumentNullException("commandParameters");

			if ((object)mapToCallback == null)
				throw new ArgumentNullException("mapToCallback");

			if (DataType.IsNullOrWhiteSpace(commandText))
				throw new ArgumentOutOfRangeException("commandText");

			results = unitOfWork.ExecuteDictionary(commandType, commandText, commandParameters, out recordsAffected);

			if (recordsAffected != queryExpectedRecordsAffected)
			{
				// idempotency failure
				unitOfWork.Divergent();
				throw new InvalidOperationException(string.Format("Data concurrency failure occurred during a fill-model execution; expected records affected '{0}', actual records affected '{1}'.", queryExpectedRecordsAffected, recordsAffected));
			}

			if ((object)results == null)
				throw new InvalidOperationException(string.Format("Results collection was invalid."));

			if (results.Count != 1)
				throw new InvalidOperationException(string.Format("Results collection count was not equal to one; count is equal to '{0}'.", results.Count));

			mapToCallback(results[0], model);
		}

		public static bool PersistModel<TModel>(this IUnitOfWork unitOfWork, TModel model, CommandType commandType, string commandText, IList<IDataParameter> commandParameters, int persistNotExpectedRecordsAffected, Action<IDictionary<string, object>, TModel> mapToCallback)
			where TModel : class, new()
		{
			int recordsAffected;
			IList<IDictionary<string, object>> results;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			if ((object)commandText == null)
				throw new ArgumentNullException("commandText");

			if ((object)commandParameters == null)
				throw new ArgumentNullException("commandParameters");

			if ((object)mapToCallback == null)
				throw new ArgumentNullException("mapToCallback");

			if (DataType.IsNullOrWhiteSpace(commandText))
				throw new ArgumentOutOfRangeException("commandText");

			results = unitOfWork.ExecuteDictionary(commandType, commandText, commandParameters, out recordsAffected);

			if (recordsAffected <= persistNotExpectedRecordsAffected)
			{
				// concurrency failure
				unitOfWork.Divergent();
				return false;
			}

			if ((object)results == null)
				throw new InvalidOperationException(string.Format("Results collection was invalid."));

			if (results.Count == 1)
				mapToCallback(results[0], model);

			return true;
		}

		public static IList<TModel> QueryModel<TModel>(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IList<IDataParameter> commandParameters, int queryExpectedRecordsAffected, Action<IDictionary<string, object>, TModel> mapToCallback)
			where TModel : class, new()
		{
			IList<TModel> models;
			TModel model;
			int recordsAffected;
			IList<IDictionary<string, object>> results;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)commandText == null)
				throw new ArgumentNullException("commandText");

			if ((object)commandParameters == null)
				throw new ArgumentNullException("commandParameters");

			if ((object)mapToCallback == null)
				throw new ArgumentNullException("mapToCallback");

			if (DataType.IsNullOrWhiteSpace(commandText))
				throw new ArgumentOutOfRangeException("commandText");

			results = unitOfWork.ExecuteDictionary(commandType, commandText, commandParameters, out recordsAffected);

			if (recordsAffected != queryExpectedRecordsAffected)
			{
				// idempotency failure
				unitOfWork.Divergent();
				throw new InvalidOperationException(string.Format("Data concurrency failure occurred during a query-model execution; expected records affected '{0}', actual records affected '{1}'.", queryExpectedRecordsAffected, recordsAffected));
			}

			if ((object)results == null)
				throw new InvalidOperationException(string.Format("Results collection was invalid."));

			models = new List<TModel>();

			foreach (IDictionary<string, object> result in results)
			{
				model = new TModel();
				mapToCallback(result, model);
				models.Add(model);
			}

			return models;
		}

		#endregion
	}
}