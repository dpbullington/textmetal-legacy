/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace TextMetal.Middleware.Datazoid
{
	public class RecordDbDataReader : DbDataReader
	{
		#region Constructors/Destructors

		public RecordDbDataReader(IEnumerable<IColumn> upstreamMetadata, IEnumerable<IRecord> targetEnumerable)
		{
			if ((object)upstreamMetadata == null)
				throw new ArgumentNullException(nameof(upstreamMetadata));

			if ((object)targetEnumerable == null)
				throw new ArgumentNullException(nameof(targetEnumerable));

			this.targetEnumerator = targetEnumerable.GetEnumerator();

			if ((object)this.targetEnumerator == null)
				throw new InvalidOperationException(nameof(targetEnumerator));

			this.ordinalLookup = upstreamMetadata.Select((mc, i) => new
																	{
																		Index = i,
																		Name = mc.ColumnName
																	}).ToDictionary(
																		p => p.Name,
																		p => p.Index,
																		StringComparer.CurrentCultureIgnoreCase);
		}

		#endregion

		#region Fields/Constants

		private readonly Dictionary<string, int> ordinalLookup;
		private readonly IEnumerator<IRecord> targetEnumerator;
		private string[] currentKeys;
		private object[] currentValues;
		private bool? isEnumerableClosed;

		#endregion

		#region Properties/Indexers/Events

		public override object this[string name]
		{
			get
			{
				return this.HasRecord ? this.TargetEnumerator.Current[name] : null;
			}
		}

		public override object this[int i]
		{
			get
			{
				return this.HasRecord ? this.CurrentValues[i] : null;
			}
		}

		public override int Depth
		{
			get
			{
				return 1;
			}
		}

		public override int FieldCount
		{
			get
			{
				return this.HasRecord ? this.TargetEnumerator.Current.Keys.Count : -1;
			}
		}

		public override bool HasRows
		{
			get
			{
				return this.HasRecord;
			}
		}

		private bool HasRecord
		{
			get
			{
				return (object)this.TargetEnumerator.Current != null;
			}
		}

		public override bool IsClosed
		{
			get
			{
				return this.IsEnumerableClosed ?? false;
			}
		}

		private Dictionary<string, int> OrdinalLookup
		{
			get
			{
				return this.ordinalLookup;
			}
		}

		public override int RecordsAffected
		{
			get
			{
				return -1;
			}
		}

		protected IEnumerator<IRecord> TargetEnumerator
		{
			get
			{
				return this.targetEnumerator;
			}
		}

		private string[] CurrentKeys
		{
			get
			{
				return this.currentKeys;
			}
			set
			{
				this.currentKeys = value;
			}
		}

		private object[] CurrentValues
		{
			get
			{
				return this.currentValues;
			}
			set
			{
				this.currentValues = value;
			}
		}

		protected bool? IsEnumerableClosed
		{
			get
			{
				return this.isEnumerableClosed;
			}
			set
			{
				this.isEnumerableClosed = value;
			}
		}

		#endregion

		#region Methods/Operators

		public virtual void Close()
		{
			this.TargetEnumerator.Dispose();
		}

		public virtual void Dispose()
		{
			this.TargetEnumerator.Dispose();
		}

		public override bool GetBoolean(int i)
		{
			return this.HasRecord ? (Boolean)this.CurrentValues[i] : default(Boolean);
		}

		public override byte GetByte(int i)
		{
			return this.HasRecord ? (Byte)this.CurrentValues[i] : default(Byte);
		}

		public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return 0;
		}

		public override char GetChar(int i)
		{
			return this.HasRecord ? (Char)this.CurrentValues[i] : default(Char);
		}

		public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return 0;
		}

		//public override DbDataReader GetData(int i)
		//{
		//	return null;
		//}

		public override string GetDataTypeName(int i)
		{
			return null;
		}

		public override DateTime GetDateTime(int i)
		{
			return this.HasRecord ? (DateTime)this.CurrentValues[i] : default(DateTime);
		}

		public override decimal GetDecimal(int i)
		{
			return this.HasRecord ? (Decimal)this.CurrentValues[i] : default(Decimal);
		}

		public override double GetDouble(int i)
		{
			return this.HasRecord ? (Double)this.CurrentValues[i] : default(Double);
		}

		public override IEnumerator GetEnumerator()
		{
			return this.TargetEnumerator;
		}

		public override Type GetFieldType(int i)
		{
			return this.HasRecord && (object)this.CurrentValues[i] != null ? this.CurrentValues[i].GetType() : null;
		}

		public override float GetFloat(int i)
		{
			return this.HasRecord ? (Single)this.CurrentValues[i] : default(Single);
		}

		public override Guid GetGuid(int i)
		{
			return this.HasRecord ? (Guid)this.CurrentValues[i] : default(Guid);
		}

		public override short GetInt16(int i)
		{
			return this.HasRecord ? (Int16)this.CurrentValues[i] : default(Int16);
		}

		public override int GetInt32(int i)
		{
			return this.HasRecord ? (Int32)this.CurrentValues[i] : default(Int32);
		}

		public override long GetInt64(int i)
		{
			return this.HasRecord ? (Int64)this.CurrentValues[i] : default(Int64);
		}

		public override string GetName(int i)
		{
			return this.HasRecord ? this.CurrentKeys[i] : null;
		}

		public override int GetOrdinal(string name)
		{
			int value;

			if (this.OrdinalLookup.TryGetValue(name, out value))
				return value;

			return -1;
		}

		//public override DataTable GetSchemaTable()
		//{
		//	return null;
		//}

		public override string GetString(int i)
		{
			return this.HasRecord ? (String)this.CurrentValues[i] : default(String);
		}

		public override object GetValue(int i)
		{
			return this.HasRecord ? (Object)this.CurrentValues[i] : default(Object);
		}

		public override int GetValues(object[] values)
		{
			return 0;
		}

		public override bool IsDBNull(int i)
		{
			return this.HasRecord ? (object)this.CurrentValues[i] == null : true;
		}

		public override bool NextResult()
		{
			return false;
		}

		public override bool Read()
		{
			if (!(this.IsEnumerableClosed ?? false))
			{
				var retval = !(bool)(this.IsEnumerableClosed = !this.TargetEnumerator.MoveNext());

				if (retval && this.HasRecord)
				{
					this.CurrentKeys = this.TargetEnumerator.Current.Keys.ToArray();
					this.CurrentValues = this.TargetEnumerator.Current.Values.ToArray();
				}

				return retval;
			}
			else
				return true;
		}

		#endregion
	}
}