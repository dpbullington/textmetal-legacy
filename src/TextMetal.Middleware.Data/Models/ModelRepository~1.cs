/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using TextMetal.Middleware.Common.Fascades.AdoNet.UoW;

namespace TextMetal.Middleware.Data.Models
{
	public abstract class ModelRepository<TDataContext> : ModelRepository, IModelRepository<TDataContext>
		where TDataContext : class, IDisposable
	{
		#region Constructors/Destructors

		protected ModelRepository()
		{
		}

		#endregion

		#region Methods/Operators

		[Conditional("DEBUG")]
		private void __DEBUG_Print(string message)
		{
			/* THIS METHOD SHOULD NOT BE DEFINED IN RELEASE/PRODUCTION BUILDS */
			Debug.WriteLine(message);
		}

		public abstract bool Discard<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new();

		public virtual bool Discard<TTableModelObject>(TTableModelObject tableModelObject)
			where TTableModelObject : class, ITableModelObject, new()
		{
			bool retval;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork(true))
				{
					// eager load by default
					retval = this.Discard<TTableModelObject>(unitOfWork, tableModelObject);

					unitOfWork.Complete();
				}
			}
			else
			{
				// eager load by default
				retval = this.Discard<TTableModelObject>(UnitOfWork.Current, tableModelObject);
			}

			return retval;
		}

		public abstract TReturnProcedureModelObject Execute<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, TCallProcedureModelObject callProcedureModel)
			where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new();

		public virtual TReturnProcedureModelObject Execute<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(TCallProcedureModelObject callProcedureModel)
			where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new()
		{
			TReturnProcedureModelObject returnProcedureModel;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork(true))
				{
					returnProcedureModel = this.Execute<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(unitOfWork, callProcedureModel);

					returnProcedureModel.Resultsets = returnProcedureModel.Resultsets.ToList(); // FORCE EAGER LOAD

					unitOfWork.Complete();
				}
			}
			else
			{
				returnProcedureModel = this.Execute<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(UnitOfWork.Current, callProcedureModel);

				if (this.ForceEagerLoading)
					returnProcedureModel.Resultsets = returnProcedureModel.Resultsets.ToList(); // FORCE EAGER LOAD

				// DO NOT FORCE EAGER LOAD
			}

			return returnProcedureModel;
		}

		public abstract bool Fill<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new();

		public virtual bool Fill<TTableModelObject>(TTableModelObject tableModelObject)
			where TTableModelObject : class, ITableModelObject, new()
		{
			bool retval;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork(false))
				{
					// eager load by default
					retval = this.Fill<TTableModelObject>(unitOfWork, tableModelObject);

					unitOfWork.Complete();
				}
			}
			else
			{
				// eager load by default
				retval = this.Fill<TTableModelObject>(UnitOfWork.Current, tableModelObject);
			}

			return retval;
		}

		public abstract IEnumerable<TTableModelObject> Find<TTableModelObject>(IUnitOfWork unitOfWork, ITableModelQuery tableModelQuery) where TTableModelObject : class, ITableModelObject, new();

		public virtual IEnumerable<TTableModelObject> Find<TTableModelObject>(ITableModelQuery tableModelQuery)
			where TTableModelObject : class, ITableModelObject, new()
		{
			IEnumerable<TTableModelObject> tableModelObjects;

			if ((object)tableModelQuery == null)
				throw new ArgumentNullException("tableModelQuery");

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork(false))
				{
					tableModelObjects = this.Find<TTableModelObject>(unitOfWork, tableModelQuery);

					tableModelObjects = tableModelObjects.ToList(); // FORCE EAGER LOAD

					unitOfWork.Complete();
				}
			}
			else
			{
				tableModelObjects = this.Find<TTableModelObject>(UnitOfWork.Current, tableModelQuery);

				if (this.ForceEagerLoading)
					tableModelObjects = tableModelObjects.ToList(); // FORCE EAGER LOAD

				// DO NOT FORCE EAGER LOAD
			}

			return tableModelObjects;
		}

		public abstract TTableModelObject Load<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject prototypeTableModel) where TTableModelObject : class, ITableModelObject, new();

		public virtual TTableModelObject Load<TTableModelObject>(TTableModelObject prototypeTableModel)
			where TTableModelObject : class, ITableModelObject, new()
		{
			TTableModelObject tableModelObject;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork(false))
				{
					// eager load by default
					tableModelObject = this.Load<TTableModelObject>(unitOfWork, prototypeTableModel);

					unitOfWork.Complete();
				}
			}
			else
			{
				// eager load by default
				tableModelObject = this.Load<TTableModelObject>(UnitOfWork.Current, prototypeTableModel);
			}

			return tableModelObject;
		}

		protected virtual void OnDiscardConflictTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			this.__DEBUG_Print(string.Format("OnDiscardConflictTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnPostDeleteTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			this.__DEBUG_Print(string.Format("OnPostDeleteTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnPostExecuteProcedureModel<TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, TReturnProcedureModelObject returnProcedureModelObject)
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)returnProcedureModelObject == null)
				throw new ArgumentNullException("returnProcedureModelObject");

			this.__DEBUG_Print(string.Format("OnPostExecuteProcedureModel <{0}, {1}>", typeof(TResultProcedureModelObject).Name, typeof(TReturnProcedureModelObject).Name));
		}

		protected virtual void OnPostInsertTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			this.__DEBUG_Print(string.Format("OnPostInsertTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnPostResultsProcedureModel<TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, TReturnProcedureModelObject returnProcedureModelObject)
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)returnProcedureModelObject == null)
				throw new ArgumentNullException("returnProcedureModelObject");

			this.__DEBUG_Print(string.Format("OnPostResultsProcedureModel <{0}, {1}>", typeof(TResultProcedureModelObject).Name, typeof(TReturnProcedureModelObject).Name));
		}

		protected virtual void OnPostSelectionTableModel<TTableModelObject>(IUnitOfWork unitOfWork) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			this.__DEBUG_Print(string.Format("OnPostSelectionTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnPostUpdateTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			this.__DEBUG_Print(string.Format("OnPostUpdateTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnPreDeleteTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			this.__DEBUG_Print(string.Format("OnPreDeleteTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnPreExecuteProcedureModel<TCallProcedureModelObject>(IUnitOfWork unitOfWork, TCallProcedureModelObject callProcedureModelObject) where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)callProcedureModelObject == null)
				throw new ArgumentNullException("callProcedureModelObject");

			this.__DEBUG_Print(string.Format("OnPreExecuteProcedureModel <{0}>", typeof(TCallProcedureModelObject).Name));
		}

		public virtual void OnPreInsertTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			this.__DEBUG_Print(string.Format("OnPreInsertTableModel <{0}>", typeof(TTableModelObject).Name));
			tableModelObject.Mark();
		}

		protected virtual void OnPreResultsProcedureModel<TCallProcedureModelObject>(IUnitOfWork unitOfWork, TCallProcedureModelObject callProcedureModelObject)
			where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)callProcedureModelObject == null)
				throw new ArgumentNullException("callProcedureModelObject");

			this.__DEBUG_Print(string.Format("OnPreResultsProcedureModel <{0}>", typeof(TCallProcedureModelObject).Name));
		}

		protected virtual void OnPreSelectionTableModel<TTableModelObject>(IUnitOfWork unitOfWork) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			this.__DEBUG_Print(string.Format("OnPreSelectionTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnPreUpdateTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			this.__DEBUG_Print(string.Format("OnPreUpdateTableModel <{0}>", typeof(TTableModelObject).Name));
			tableModelObject.Mark();
		}

		protected virtual void OnResultProcedureModel<TResultProcedureModelObject>(IUnitOfWork unitOfWork, TResultProcedureModelObject resultProcedureModelObject) where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)resultProcedureModelObject == null)
				throw new ArgumentNullException("resultProcedureModelObject");

			this.__DEBUG_Print(string.Format("OnResultProcedureModel <{0}>", typeof(TResultProcedureModelObject).Name));
		}

		protected virtual void OnResultsetProcedureModel<TResultsetModelObject, TResultProcedureModelObject>(IUnitOfWork unitOfWork, TResultsetModelObject resultsetModelObject)
			where TResultsetModelObject : class, IResultsetModelObject<TResultProcedureModelObject>, new()
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)resultsetModelObject == null)
				throw new ArgumentNullException("resultsetModelObject");

			this.__DEBUG_Print(string.Format("OnResultsetProcedureModel <{0}>", resultsetModelObject.Index));
		}

		protected virtual void OnSaveConflictTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			this.__DEBUG_Print(string.Format("OnSaveConflictTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnSelectTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			this.__DEBUG_Print(string.Format("OnSelectTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		public TProjection Query<TProjection>(Func<TDataContext, TProjection> contextQueryCallback)
		{
			TProjection projection;

			if ((object)contextQueryCallback == null)
				throw new ArgumentNullException("contextQueryCallback");

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork(false))
				{
					projection = this.Query<TProjection>(unitOfWork, contextQueryCallback);

					// HACK ALERT: will this work as expected?
					if (projection is IEnumerable)
						((IEnumerable)projection).Cast<object>().ToList(); // FORCE EAGER LOAD

					unitOfWork.Complete();
				}
			}
			else
			{
				projection = this.Query<TProjection>(UnitOfWork.Current, contextQueryCallback);

				if (this.ForceEagerLoading)
				{
					// HACK ALERT: will this work as expected?
					if (projection is IEnumerable)
						((IEnumerable)projection).Cast<object>().ToList(); // FORCE EAGER LOAD
				}

				// DO NOT FORCE EAGER LOAD
			}

			return projection;
		}

		public abstract TProjection Query<TProjection>(IUnitOfWork unitOfWork, Func<TDataContext, TProjection> dataContextQueryCallback);

		public abstract bool Save<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new();

		public virtual bool Save<TTableModelObject>(TTableModelObject tableModelObject)
			where TTableModelObject : class, ITableModelObject, new()
		{
			bool retval;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork(true))
				{
					// eager load by default
					retval = this.Save<TTableModelObject>(unitOfWork, tableModelObject);

					unitOfWork.Complete();
				}
			}
			else
			{
				// eager load by default
				retval = this.Save<TTableModelObject>(UnitOfWork.Current, tableModelObject);
			}

			return retval;
		}

		#endregion
	}
}