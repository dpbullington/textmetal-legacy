/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

namespace TextMetal.Common.Data
{
	public class WrapperDataReader : IDataReader
	{
		#region Constructors/Destructors

		public WrapperDataReader(IDataReader dataReader)
		{
			if ((object)dataReader == null)
				throw new ArgumentNullException("dataReader");

			this.dataReader = dataReader;
		}

		#endregion

		#region Fields/Constants

		private readonly IDataReader dataReader;

		#endregion

		#region Properties/Indexers/Events

		public virtual object this[string name]
		{
			get
			{
				return this.DataReader[name];
			}
		}

		public virtual object this[int i]
		{
			get
			{
				return this.DataReader[i];
			}
		}

		protected IDataReader DataReader
		{
			get
			{
				return this.dataReader;
			}
		}

		public virtual int Depth
		{
			get
			{
				return this.DataReader.Depth;
			}
		}

		public virtual int FieldCount
		{
			get
			{
				return this.DataReader.FieldCount;
			}
		}

		public virtual bool IsClosed
		{
			get
			{
				return this.DataReader.IsClosed;
			}
		}

		public virtual int RecordsAffected
		{
			get
			{
				return this.DataReader.RecordsAffected;
			}
		}

		#endregion

		#region Methods/Operators

		public virtual void Close()
		{
			this.DataReader.Close();
		}

		public virtual void Dispose()
		{
			this.DataReader.Dispose();
		}

		public virtual bool GetBoolean(int i)
		{
			return this.DataReader.GetBoolean(i);
		}

		public virtual byte GetByte(int i)
		{
			return this.DataReader.GetByte(i);
		}

		public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return this.DataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		public virtual char GetChar(int i)
		{
			return this.DataReader.GetChar(i);
		}

		public virtual long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return this.DataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		public virtual IDataReader GetData(int i)
		{
			return this.DataReader.GetData(i);
		}

		public virtual string GetDataTypeName(int i)
		{
			return this.DataReader.GetDataTypeName(i);
		}

		public virtual DateTime GetDateTime(int i)
		{
			return this.DataReader.GetDateTime(i);
		}

		public virtual decimal GetDecimal(int i)
		{
			return this.DataReader.GetDecimal(i);
		}

		public double GetDouble(int i)
		{
			return this.DataReader.GetDouble(i);
		}

		public virtual Type GetFieldType(int i)
		{
			return this.DataReader.GetFieldType(i);
		}

		public virtual float GetFloat(int i)
		{
			return this.DataReader.GetFloat(i);
		}

		public virtual Guid GetGuid(int i)
		{
			return this.DataReader.GetGuid(i);
		}

		public virtual short GetInt16(int i)
		{
			return this.DataReader.GetInt16(i);
		}

		public virtual int GetInt32(int i)
		{
			return this.DataReader.GetInt32(i);
		}

		public virtual long GetInt64(int i)
		{
			return this.DataReader.GetInt64(i);
		}

		public virtual string GetName(int i)
		{
			return this.DataReader.GetName(i);
		}

		public virtual int GetOrdinal(string name)
		{
			return this.DataReader.GetOrdinal(name);
		}

		public virtual DataTable GetSchemaTable()
		{
			return this.DataReader.GetSchemaTable();
		}

		public virtual string GetString(int i)
		{
			return this.DataReader.GetString(i);
		}

		public virtual object GetValue(int i)
		{
			return this.DataReader.GetValue(i);
		}

		public virtual int GetValues(object[] values)
		{
			return this.DataReader.GetValues(values);
		}

		public virtual bool IsDBNull(int i)
		{
			return this.DataReader.IsDBNull(i);
		}

		public virtual bool NextResult()
		{
			return this.DataReader.NextResult();
		}

		public virtual bool Read()
		{
			return this.DataReader.Read();
		}

		#endregion
	}
}