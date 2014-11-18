/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Common.Data.Framework
{
	public interface IModelRepository : IUnitOfWorkFactory
	{
		#region Methods/Operators

		TModel CreateModel<TModel>()
			where TModel : class, IModelObject;

		TModel CreateModel<TModel>(Action<TModel> initializionCallback)
			where TModel : class, IModelObject;

		TRequestModel CreateRequestModel<TRequestModel>()
			where TRequestModel : class, IRequestModelObject;

		TResponseModel CreateResponseModel<TResultModel, TResponseModel>()
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>;

		TResultModel CreateResultModel<TResultModel>()
			where TResultModel : class, IResultModelObject;

		bool Discard<TModel>(TModel model)
			where TModel : class, IModelObject;

		bool Discard<TModel>(IUnitOfWork unitOfWork, TModel model)
			where TModel : class, IModelObject;

		TResponseModel Execute<TRequestModel, TResultModel, TResponseModel>(TRequestModel requestModel)
			where TRequestModel : class, IRequestModelObject
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>;

		TResponseModel Execute<TRequestModel, TResultModel, TResponseModel>(IUnitOfWork unitOfWork, TRequestModel requestModel)
			where TRequestModel : class, IRequestModelObject
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>;

		bool Fill<TModel>(TModel model)
			where TModel : class, IModelObject;

		bool Fill<TModel>(IUnitOfWork unitOfWork, TModel model)
			where TModel : class, IModelObject;

		IEnumerable<TModel> Find<TModel>(IModelQuery modelQuery)
			where TModel : class, IModelObject;

		IEnumerable<TModel> Find<TModel>(IUnitOfWork unitOfWork, IModelQuery query)
			where TModel : class, IModelObject;

		TModel Load<TModel>(TModel prototype)
			where TModel : class, IModelObject;

		TModel Load<TModel>(IUnitOfWork unitOfWork, TModel prototype)
			where TModel : class, IModelObject;

		bool Save<TModel>(TModel model)
			where TModel : class, IModelObject;

		bool Save<TModel>(IUnitOfWork unitOfWork, TModel model)
			where TModel : class, IModelObject;

		#endregion
	}
}