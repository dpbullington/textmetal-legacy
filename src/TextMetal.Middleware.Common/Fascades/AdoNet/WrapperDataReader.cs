/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

namespace TextMetal.Middleware.Common.Fascades.AdoNet
{
	public class WrapperDataReader : IDataReader
	{
		#region Constructors/Destructors

		public WrapperDataReader(IDataReader innerDataReader)
		{
			if ((object)innerDataReader == null)
				throw new ArgumentNullException("innerDataReader");

			this.innerDataReader = innerDataReader;
		}

		#endregion

		#region Fields/Constants

		private readonly IDataReader innerDataReader;

		#endregion

		#region Properties/Indexers/Events

		public virtual object this[string name]
		{
			get
			{
				return this.InnerDataReader[name];
			}
		}

		public virtual object this[int i]
		{
			get
			{
				return this.InnerDataReader[i];
			}
		}

		public virtual int Depth
		{
			get
			{
				return this.InnerDataReader.Depth;
			}
		}

		public virtual int FieldCount
		{
			get
			{
				return this.InnerDataReader.FieldCount;
			}
		}

		protected IDataReader InnerDataReader
		{
			get
			{
				return this.innerDataReader;
			}
		}

		public virtual bool IsClosed
		{
			get
			{
				return this.InnerDataReader.IsClosed;
			}
		}

		public virtual int RecordsAffected
		{
			get
			{
				return this.InnerDataReader.RecordsAffected;
			}
		}

		#endregion

		#region Methods/Operators

		public virtual void Close()
		{
			this.Dispose(true);
			GC.SuppressFinalize((object)this);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize((object)this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				this.InnerDataReader.Dispose();
		}

		public virtual bool GetBoolean(int i)
		{
			return this.InnerDataReader.GetBoolean(i);
		}

		public virtual byte GetByte(int i)
		{
			return this.InnerDataReader.GetByte(i);
		}

		public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return this.InnerDataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		public virtual char GetChar(int i)
		{
			return this.InnerDataReader.GetChar(i);
		}

		public virtual long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return this.InnerDataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		public virtual IDataReader GetData(int i)
		{
			return this.InnerDataReader.GetData(i);
		}

		public virtual string GetDataTypeName(int i)
		{
			return this.InnerDataReader.GetDataTypeName(i);
		}

		public virtual DateTime GetDateTime(int i)
		{
			return this.InnerDataReader.GetDateTime(i);
		}

		public virtual decimal GetDecimal(int i)
		{
			return this.InnerDataReader.GetDecimal(i);
		}

		public double GetDouble(int i)
		{
			return this.InnerDataReader.GetDouble(i);
		}

		public virtual Type GetFieldType(int i)
		{
			return this.InnerDataReader.GetFieldType(i);
		}

		public virtual float GetFloat(int i)
		{
			return this.InnerDataReader.GetFloat(i);
		}

		public virtual Guid GetGuid(int i)
		{
			return this.InnerDataReader.GetGuid(i);
		}

		public virtual short GetInt16(int i)
		{
			return this.InnerDataReader.GetInt16(i);
		}

		public virtual int GetInt32(int i)
		{
			return this.InnerDataReader.GetInt32(i);
		}

		public virtual long GetInt64(int i)
		{
			return this.InnerDataReader.GetInt64(i);
		}

		public virtual string GetName(int i)
		{
			return this.InnerDataReader.GetName(i);
		}

		public virtual int GetOrdinal(string name)
		{
			return this.InnerDataReader.GetOrdinal(name);
		}

		public virtual DataTable GetSchemaTable()
		{
			return this.InnerDataReader.GetSchemaTable();
		}

		public virtual string GetString(int i)
		{
			return this.InnerDataReader.GetString(i);
		}

		public virtual object GetValue(int i)
		{
			return this.InnerDataReader.GetValue(i);
		}

		public virtual int GetValues(object[] values)
		{
			return this.InnerDataReader.GetValues(values);
		}

		public virtual bool IsDBNull(int i)
		{
			return this.InnerDataReader.IsDBNull(i);
		}

		public virtual bool NextResult()
		{
			return this.InnerDataReader.NextResult();
		}

		public virtual bool Read()
		{
			return this.InnerDataReader.Read();
		}

		#endregion
	}
}