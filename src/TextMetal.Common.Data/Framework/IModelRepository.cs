/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace TextMetal.Common.Data.Framework
{
	public interface IModelRepository
	{

		#region Methods/Operators

		TModel CreateModel<TModel>()
			where TModel : class, IModelObject;

		TRequestModel CreateRequestModel<TRequestModel>()
			where TRequestModel : class, IRequestModelObject;

		TResponseModel CreateResponseModel<TResponseModel, TResultModel>()
			where TResponseModel : class, IResponseModelObject<TResultModel>
			where TResultModel : class, IResultModelObject;

		TResultModel CreateResultModel<TResultModel>()
			where TResultModel : class, IResultModelObject;

		TModel Discard<TModel>(TModel model)
			where TModel : class, IModelObject;

		TModel Discard<TModel>(IUnitOfWork unitOfWork, TModel model)
			where TModel : class, IModelObject;

		TResponseModel ExecuteImperative<TRequestModel, TResultModel, TResponseModel>(TRequestModel requestModel)
			where TRequestModel : class, IRequestModelObject
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>;

		TResponseModel ExecuteImperative<TRequestModel, TResultModel, TResponseModel>(IUnitOfWork unitOfWork, TRequestModel requestModel)
			where TRequestModel : class, IRequestModelObject
			where TResultModel : class, IResultModelObject
			where TResponseModel : class, IResponseModelObject<TResultModel>;

		TModel Fill<TModel>(TModel model)
			where TModel : class, IModelObject;

		TModel Fill<TModel>(IUnitOfWork unitOfWork, TModel model)
			where TModel : class, IModelObject;

		IEnumerable<TModel> Find<TModel>(IModelQuery query)
			where TModel : class, IModelObject;

		IEnumerable<TModel> Find<TModel>(IUnitOfWork unitOfWork, IModelQuery query)
			where TModel : class, IModelObject;

		IUnitOfWork GetUnitOfWork();

		TModel Load<TModel>(TModel prototype)
			where TModel : class, IModelObject;

		TModel Load<TModel>(IUnitOfWork unitOfWork, TModel prototype)
			where TModel : class, IModelObject;

		TModel Save<TModel>(TModel model)
			where TModel : class, IModelObject;

		TModel Save<TModel>(IUnitOfWork unitOfWork, TModel model)
			where TModel : class, IModelObject;

		#endregion
	}
}