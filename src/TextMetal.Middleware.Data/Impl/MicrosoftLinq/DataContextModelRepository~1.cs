/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

using TextMetal.Middleware.Common.Fascades.AdoNet.UoW;
using TextMetal.Middleware.Data.Models.Tabular;

namespace TextMetal.Middleware.Data.Impl.MicrosoftLinq
{
	/// <summary>
	/// LINQ to SQL
	/// NOTE: TDataContext must support GetConstructor(new Type[] { typeof(IDbConnection) }).
	/// </summary>
	/// <typeparam name="TDataContext"> </typeparam>
	public abstract class DataContextModelRepository<TDataContext> : ContextModelRepository<TDataContext>
		where TDataContext : DataContext
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the DataContextModelRepository`1 class.
		/// </summary>
		protected DataContextModelRepository()
		{
		}

		#endregion

		#region Methods/Operators

		protected static XElement ToXElement(XmlDocument xmlDocument)
		{
			if ((object)xmlDocument == null)
				throw new ArgumentNullException("xmlDocument");

			using (XmlNodeReader nodeReader = new XmlNodeReader(xmlDocument))
			{
				nodeReader.MoveToContent();
				return XElement.Load(nodeReader);
			}
		}

		protected static XmlDocument ToXmlDocument(XElement xElement)
		{
			XmlDocument xmlDocument;

			if ((object)xElement == null)
				throw new ArgumentNullException("xElement");

			xmlDocument = new XmlDocument();

			using (XmlReader xmlReader = xElement.CreateReader())
				xmlDocument.Load(xmlReader);

			return xmlDocument;
		}

		public override bool Discard<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject)
		{
			bool wasNew;
			Table<TTableModelObject> linqTable;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			wasNew = tableModelObject.IsNew;
			tableModelObject.Mark();

			if (wasNew)
				return true;

			using (AmbientUnitOfWorkAwareContextWrapper<TDataContext> wrapper = this.GetContext(unitOfWork))
			{
				this.OnPreDeleteTableModel<TTableModelObject>(unitOfWork, tableModelObject);

				linqTable = wrapper.DisposableContext.GetTable<TTableModelObject>();
				linqTable.DeleteOnSubmit(tableModelObject);

				try
				{
					wrapper.DisposableContext.SubmitChanges(ConflictMode.FailOnFirstConflict);
				}
				catch (ChangeConflictException ccex)
				{
					this.OnDiscardConflictTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					return false;
				}

				this.OnPostDeleteTableModel<TTableModelObject>(unitOfWork, tableModelObject);

				tableModelObject.IsNew = false;

				return true;
			}
		}

		public override TReturnProcedureModelObject Execute<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, TCallProcedureModelObject callProcedureModel)
		{
			throw new NotSupportedException(string.Format("Execute is not supported."));
		}

		public override bool Fill<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject)
		{
			Table<TTableModelObject> linqTable;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			using (AmbientUnitOfWorkAwareContextWrapper<TDataContext> wrapper = this.GetContext(unitOfWork))
			{
				linqTable = wrapper.DisposableContext.GetTable<TTableModelObject>();
				wrapper.DisposableContext.Refresh(RefreshMode.OverwriteCurrentValues, tableModelObject);

				return true;
			}
		}

		public override IEnumerable<TTableModelObject> Find<TTableModelObject>(IUnitOfWork unitOfWork, ITableModelQuery tableModelQuery)
		{
			IQueryable<TTableModelObject> tableModelObjects;
			Table<TTableModelObject> linqTable;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			using (AmbientUnitOfWorkAwareContextWrapper<TDataContext> wrapper = this.GetContext(unitOfWork))
			{
				linqTable = wrapper.DisposableContext.GetTable<TTableModelObject>();
				tableModelObjects = linqTable.Where(((LinqTableModelQuery<TTableModelObject>)tableModelQuery).Predicate);

				if ((object)tableModelObjects == null)
					throw new InvalidOperationException(string.Format("The table models returned were invalid."));

				// DOES NOT FORCE EXECUTION AGAINST STORE
				foreach (TTableModelObject tableModelObject in tableModelObjects)
				{
					this.OnSelectTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					yield return tableModelObject; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}
			}
		}

		protected override TDataContext GetContext(Type contextType, DbConnection dbConnection, DbTransaction dbTransaction)
		{
			TDataContext dataContext;
			ConstructorInfo constructorInfo;

			if ((object)contextType == null)
				throw new ArgumentNullException("contextType");

			if ((object)dbConnection == null)
				throw new ArgumentNullException("dbConnection");

			constructorInfo = contextType.GetConstructor(new Type[] { typeof(IDbConnection) });

			if ((object)constructorInfo == null)
				throw new InvalidOperationException("constructorInfo");

			dataContext = (TDataContext)constructorInfo.Invoke(new object[] { dbConnection });

			if ((object)dbTransaction != null)
				dataContext.Transaction = (DbTransaction)dbTransaction;

			return dataContext;
		}

		public override TTableModelObject Load<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject prototypeTableModel)
		{
			TTableModelObject tableModelObject;
			Table<TTableModelObject> linqTable;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			using (AmbientUnitOfWorkAwareContextWrapper<TDataContext> wrapper = this.GetContext(unitOfWork))
			{
				linqTable = wrapper.DisposableContext.GetTable<TTableModelObject>();

				linqTable.Attach(prototypeTableModel);
				wrapper.DisposableContext.Refresh(RefreshMode.OverwriteCurrentValues, prototypeTableModel);

				//tableModelObject = new TTableModelObject();
				tableModelObject = prototypeTableModel;

				return tableModelObject;
			}
		}

		protected override bool OnCreateNativeDatabaseFile(string databaseFilePath)
		{
			throw new NotSupportedException(string.Format("OnCreateNativeDatabaseFile is not supported."));
		}

		public override bool Save<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject)
		{
			bool wasNew;
			Table<TTableModelObject> linqTable;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			wasNew = tableModelObject.IsNew;
			tableModelObject.Mark();

			using (AmbientUnitOfWorkAwareContextWrapper<TDataContext> wrapper = this.GetContext(unitOfWork))
			{
				linqTable = wrapper.DisposableContext.GetTable<TTableModelObject>();

				if (wasNew)
				{
					this.OnPreInsertTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					linqTable.InsertOnSubmit(tableModelObject);
				}
				else
				{
					this.OnPreUpdateTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					//linqTable.Nop(Model);
				}

				try
				{
					wrapper.DisposableContext.SubmitChanges(ConflictMode.FailOnFirstConflict);
				}
				catch (ChangeConflictException ccex)
				{
					this.OnSaveConflictTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					return false;
				}

				if (wasNew)
					this.OnPostInsertTableModel<TTableModelObject>(unitOfWork, tableModelObject);
				else
					this.OnPostUpdateTableModel<TTableModelObject>(unitOfWork, tableModelObject);

				return true;
			}
		}

		#endregion
	}
}