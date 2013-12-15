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
	using System;

	public abstract partial class DictionaryAdapterBase
	{
		#region Methods/Operators

		public T Coerce<T>() where T : class
		{
			return (T)this.Coerce(typeof(T));
		}

		public object Coerce(Type type)
		{
			if (type.IsAssignableFrom(this.Meta.Type))
				return this;

			if (this.This.CoerceStrategy != null)
			{
				var coerced = this.This.CoerceStrategy.Coerce(this, type);
				if (coerced != null)
					return coerced;
			}
			return this.This.Factory.GetAdapter(type, this.This.Dictionary, this.This.Descriptor);
		}

		#endregion
	}
}