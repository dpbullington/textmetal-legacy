// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.DictionaryAdapter
{
#if !SILVERLIGHT
	using System;
	using System.ComponentModel;

	public class BindingListInitializer<T> : IValueInitializer
	{
		#region Constructors/Destructors

		public BindingListInitializer(Func<int, object, object> addAt, Func<object> addNew, Func<int, object, object> setAt, Action<int> removeAt, Action reset)
		{
			this.addAt = addAt;
			this.addNew = addNew;
			this.setAt = setAt;
			this.removeAt = removeAt;
			this.reset = reset;
		}

		#endregion

		#region Fields/Constants

		private readonly Func<int, object, object> addAt;
		private readonly Func<object> addNew;
		private readonly Action<int> removeAt;
		private readonly Action reset;
		private readonly Func<int, object, object> setAt;

		private bool addingNew;

		#endregion

		#region Methods/Operators

		public void Initialize(IDictionaryAdapter dictionaryAdapter, object value)
		{
			var bindingList = (System.ComponentModel.BindingList<T>)value;
			if (this.addNew != null)
			{
				bindingList.AddingNew += (sender, args) =>
										{
											args.NewObject = this.addNew();
											this.addingNew = true;
										};
			}
			bindingList.ListChanged += (sender, args) =>
										{
											switch (args.ListChangedType)
											{
												case ListChangedType.ItemAdded:
													if (this.addingNew == false && this.addAt != null)
													{
														var item = this.addAt(args.NewIndex, bindingList[args.NewIndex]);
														if (item != null)
														{
															using (new SuppressListChangedEvents(bindingList))
																bindingList[args.NewIndex] = (T)item;
														}
													}
													this.addingNew = false;
													break;

												case ListChangedType.ItemChanged:
													if (this.setAt != null)
													{
														var item = this.setAt(args.NewIndex, bindingList[args.NewIndex]);
														if (item != null)
														{
															using (new SuppressListChangedEvents(bindingList))
																bindingList[args.NewIndex] = (T)item;
														}
													}
													break;

												case ListChangedType.ItemDeleted:
													if (this.removeAt != null)
														this.removeAt(args.NewIndex);
													break;

												case ListChangedType.Reset:
													if (this.reset != null)
														this.reset();
													break;
											}
										};
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class SuppressListChangedEvents : IDisposable
		{
			#region Constructors/Destructors

			public SuppressListChangedEvents(System.ComponentModel.BindingList<T> bindingList)
			{
				this.bindingList = bindingList;
				this.raiseEvents = this.bindingList.RaiseListChangedEvents;
				this.bindingList.RaiseListChangedEvents = false;
			}

			#endregion

			#region Fields/Constants

			private readonly System.ComponentModel.BindingList<T> bindingList;
			private readonly bool raiseEvents;

			#endregion

			#region Methods/Operators

			public void Dispose()
			{
				this.bindingList.RaiseListChangedEvents = this.raiseEvents;
			}

			#endregion
		}

		#endregion
	}

#endif
}