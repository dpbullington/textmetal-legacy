/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data.Framework
{
	/// <summary>
	/// Provides static helper and/or extension methods for ModelMap O/RM.
	/// </summary>
	public static class ModelMapHelper
	{
		#region Methods/Operators

		public static TModel Discard<TModel>(this IUnitOfWork unitOfWork, TModel model)
			where TModel : class, IModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			return null;
		}

		public static TResponseModel ExecuteImperative<TRequestModel, TResultModel, TResponseModel>(this IUnitOfWork unitOfWork, TRequestModel requestModel)
			where TRequestModel : class, IRequestModelObject
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>
		{
			return null;
		}

		public static TResponse ExecuteModel<TRequest, TResult, TResponse>(this IUnitOfWork unitOfWork, TRequest request, CommandType commandType, string commandText, IList<IDataParameter> commandParameters, bool executeAsCud, int thisOrThatRecordsAffected, Action<IDictionary<string, object>, TResult> mapToCallback, Func<TResponse> createNewResponseCallback, Func<TResult> createNewResultCallback)
			where TRequest : class, IRequestModelObject
			where TResult : class, IResultModelObject
			where TResponse : class, IResponseModelObject<TResult>
		{
			TResponse response;
			TResult result;
			int recordsAffected;
			IEnumerable<IDictionary<string, object>> results;
			IList<TResult> list;
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

			if ((object)createNewResponseCallback == null)
				throw new ArgumentNullException("createNewResponseCallback");

			if ((object)createNewResultCallback == null)
				throw new ArgumentNullException("createNewResultCallback");

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

			response = createNewResponseCallback();

			list = new List<TResult>();
			response.Results = list;

			foreach (IDictionary<string, object> _result in results)
			{
				result = createNewResultCallback();

				mapToCallback(_result, result);

				list.Add(result);
			}

			return response;
		}

		public static TModel FetchModel<TModel>(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IList<IDataParameter> commandParameters, int queryExpectedRecordsAffected, Action<IDictionary<string, object>, TModel> mapToCallback, Func<TModel> createNewCallback)
			where TModel : class, IModelObject
		{
			TModel model;
			int recordsAffected;
			IEnumerable<IDictionary<string, object>> results;
			IDictionary<string, object> result;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)commandText == null)
				throw new ArgumentNullException("commandText");

			if ((object)commandParameters == null)
				throw new ArgumentNullException("commandParameters");

			if ((object)mapToCallback == null)
				throw new ArgumentNullException("mapToCallback");

			if ((object)createNewCallback == null)
				throw new ArgumentNullException("createNewCallback");

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

			result = results.SingleOrDefault();

			if ((object)result == null)
				return null;

			model = createNewCallback();

			mapToCallback(result, model);

			return model;
		}

		/// <summary>
		/// Allows for easy scalar query execution over a unit of work.
		/// </summary>
		/// <typeparam name="TValue"> The scalar type. </typeparam>
		/// <param name="unitOfWork"> The target unit of work. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <returns> The scalar value (with type conversion) or null. </returns>
		public static TValue FetchScalar<TValue>(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<IDataParameter> commandParameters)
		{
			int recordsAffected;
			IEnumerable<IDictionary<string, object>> results;
			IDictionary<string, object> result;
			object dbValue;

			results = unitOfWork.ExecuteDictionary(commandType, commandText, commandParameters, out recordsAffected);

			if ((object)results == null)
				return default(TValue);

			result = results.SingleOrDefault();

			if ((object)result == null)
				return default(TValue);

			if (result.Count != 1)
				return default(TValue);

			if (result.Keys.Count != 1)
				return default(TValue);

			dbValue = result[result.Keys.First()];

			return dbValue.ChangeType<TValue>();
		}

		public static TModel Fill<TModel>(this IUnitOfWork unitOfWork, TModel model)
			where TModel : class, IModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			return null;
		}

		public static void FillModel<TModel>(this IUnitOfWork unitOfWork, TModel model, CommandType commandType, string commandText, IList<IDataParameter> commandParameters, int queryExpectedRecordsAffected, Action<IDictionary<string, object>, TModel> mapToCallback)
			where TModel : class, IModelObject
		{
			int recordsAffected;
			IEnumerable<IDictionary<string, object>> results;
			IDictionary<string, object> result;

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

			result = results.SingleOrDefault();

			if ((object)result != null)
				throw new InvalidOperationException(string.Format("Results collection count was not equal to one."));

			mapToCallback(result, model);
		}

		public static IEnumerable<TModel> Find<TModel>(this IUnitOfWork unitOfWork, IModelQuery query)
			where TModel : class, IModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)query == null)
				throw new ArgumentNullException("query");

			return null;
		}

		public static TModel Load<TModel>(this IUnitOfWork unitOfWork, TModel prototype)
			where TModel : class, IModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)prototype == null)
				throw new ArgumentNullException("prototype");

			return null;
		}

		public static bool PersistModel<TModel>(this IUnitOfWork unitOfWork, TModel model, CommandType commandType, string commandText, IList<IDataParameter> commandParameters, int persistNotExpectedRecordsAffected, Action<IDictionary<string, object>, TModel> mapToCallback)
			where TModel : class, IModelObject
		{
			int recordsAffected;
			IEnumerable<IDictionary<string, object>> results;
			IDictionary<string, object> result;

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

			result = results.SingleOrDefault();

			if ((object)result != null)
				mapToCallback(result, model);

			return true;
		}

		public static IList<TModel> QueryModel<TModel>(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IList<IDataParameter> commandParameters, int queryExpectedRecordsAffected, Action<IDictionary<string, object>, TModel> mapToCallback, Func<TModel> createNewCallback)
			where TModel : class, IModelObject
		{
			IList<TModel> models;
			TModel model;
			int recordsAffected;
			IEnumerable<IDictionary<string, object>> results;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)commandText == null)
				throw new ArgumentNullException("commandText");

			if ((object)commandParameters == null)
				throw new ArgumentNullException("commandParameters");

			if ((object)mapToCallback == null)
				throw new ArgumentNullException("mapToCallback");

			if ((object)createNewCallback == null)
				throw new ArgumentNullException("createNewCallback");

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
				model = createNewCallback();
				mapToCallback(result, model);
				models.Add(model);
			}

			return models;
		}

		public static TModel Save<TModel>(this IUnitOfWork unitOfWork, TModel model)
			where TModel : class, IModelObject
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			return null;
		}

		#endregion
	}
}