/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Data.Common;

using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Datazoid
{
	public class WrappedDbDataReader : DbDataReader
	{
		#region Constructors/Destructors

		public WrappedDbDataReader(DbDataReader innerDbDataReader)
		{
			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::.ctor(...)", typeof(WrappedDbDataReader).Name));

			if ((object)innerDbDataReader == null)
				throw new ArgumentNullException(nameof(innerDbDataReader));

			this.innerDbDataReader = innerDbDataReader;
		}

		#endregion

		#region Fields/Constants

		private readonly DbDataReader innerDbDataReader;

		#endregion

		#region Properties/Indexers/Events

		public override object this[string name]
		{
			get
			{
				return this.InnerDbDataReader[name];
			}
		}

		public override object this[int i]
		{
			get
			{
				return this.InnerDbDataReader[i];
			}
		}

		public override int Depth
		{
			get
			{
				return this.InnerDbDataReader.Depth;
			}
		}

		public override int FieldCount
		{
			get
			{
				return this.InnerDbDataReader.FieldCount;
			}
		}

		public override bool HasRows
		{
			get
			{
				return this.InnerDbDataReader.HasRows;
			}
		}

		protected DbDataReader InnerDbDataReader
		{
			get
			{
				return this.innerDbDataReader;
			}
		}

		public override bool IsClosed
		{
			get
			{
				return this.InnerDbDataReader.IsClosed;
			}
		}

		public override int RecordsAffected
		{
			get
			{
				return this.InnerDbDataReader.RecordsAffected;
			}
		}

		#endregion

		#region Methods/Operators

		public virtual void Close()
		{
			this.Dispose(true);
			GC.SuppressFinalize((object)this);
		}

		public virtual void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize((object)this);
		}

		protected override void Dispose(bool disposing)
		{
			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::Dispose(...): enter", typeof(WrappedDbDataReader).Name));

			if (disposing)
				this.InnerDbDataReader.Dispose();

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::Dispose(...): leave", typeof(WrappedDbDataReader).Name));
		}

		public override bool GetBoolean(int i)
		{
			return this.InnerDbDataReader.GetBoolean(i);
		}

		public override byte GetByte(int i)
		{
			return this.InnerDbDataReader.GetByte(i);
		}

		public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return this.InnerDbDataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		public override char GetChar(int i)
		{
			return this.InnerDbDataReader.GetChar(i);
		}

		public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return this.InnerDbDataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		//public override DbDataReader GetData(int i)
		//{
		//	return this.InnerDbDataReader.GetData(i);
		//}

		public override string GetDataTypeName(int i)
		{
			return this.InnerDbDataReader.GetDataTypeName(i);
		}

		public override DateTime GetDateTime(int i)
		{
			return this.InnerDbDataReader.GetDateTime(i);
		}

		public override decimal GetDecimal(int i)
		{
			return this.InnerDbDataReader.GetDecimal(i);
		}

		public override double GetDouble(int i)
		{
			return this.InnerDbDataReader.GetDouble(i);
		}

		public override IEnumerator GetEnumerator()
		{
			return this.InnerDbDataReader.GetEnumerator();
		}

		public override Type GetFieldType(int i)
		{
			Type retval;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetFieldType(...): enter", typeof(WrappedDbDataReader).Name));

			retval = this.InnerDbDataReader.GetFieldType(i);

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetFieldType(...): return name", typeof(WrappedDbDataReader).Name));

			return retval;
		}

		public override float GetFloat(int i)
		{
			return this.InnerDbDataReader.GetFloat(i);
		}

		public override Guid GetGuid(int i)
		{
			return this.InnerDbDataReader.GetGuid(i);
		}

		public override short GetInt16(int i)
		{
			return this.InnerDbDataReader.GetInt16(i);
		}

		public override int GetInt32(int i)
		{
			return this.InnerDbDataReader.GetInt32(i);
		}

		public override long GetInt64(int i)
		{
			return this.InnerDbDataReader.GetInt64(i);
		}

		public override string GetName(int i)
		{
			string retval;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetName(...): enter", typeof(WrappedDbDataReader).Name));

			retval = this.InnerDbDataReader.GetName(i);

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetName(...): return name", typeof(WrappedDbDataReader).Name));

			return retval;
		}

		public override int GetOrdinal(string name)
		{
			int retval;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetOrdinal(...): enter", typeof(WrappedDbDataReader).Name));

			retval = this.InnerDbDataReader.GetOrdinal(name);

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetOrdinal(...): return value", typeof(WrappedDbDataReader).Name));

			return retval;
		}

		//public override DataTable GetSchemaTable()
		//{
		//	return this.InnerDbDataReader.GetSchemaTable();
		//}

		public override string GetString(int i)
		{
			return this.InnerDbDataReader.GetString(i);
		}

		public override object GetValue(int i)
		{
			object retval;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetValue(...): enter", typeof(WrappedDbDataReader).Name));

			retval = this.InnerDbDataReader.GetValue(i);

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetValue(...): return value", typeof(WrappedDbDataReader).Name));

			return retval;
		}

		public override int GetValues(object[] values)
		{
			return this.InnerDbDataReader.GetValues(values);
		}

		public override bool IsDBNull(int i)
		{
			return this.InnerDbDataReader.IsDBNull(i);
		}

		public override bool NextResult()
		{
			bool retval;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::NextResult(...): enter", typeof(WrappedDbDataReader).Name));

			retval = this.InnerDbDataReader.NextResult();

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::NextResult(...): return flag", typeof(WrappedDbDataReader).Name));

			return retval;
		}

		public override bool Read()
		{
			bool retval;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::Read(...): enter", typeof(WrappedDbDataReader).Name));

			retval = this.InnerDbDataReader.Read();

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::Read(...): return flag", typeof(WrappedDbDataReader).Name));

			return retval;
		}

		#endregion
	}
}