/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Datazoid.Primitives
{
	public class RecordAdapterDbDataReader : DbDataReader
	{
		#region Constructors/Destructors

		public RecordAdapterDbDataReader(IEnumerable<IColumn> upstreamMetadata, IEnumerable<IRecord> targetEnumerable)
		{
			if ((object)upstreamMetadata == null)
				throw new ArgumentNullException(nameof(upstreamMetadata));

			if ((object)targetEnumerable == null)
				throw new ArgumentNullException(nameof(targetEnumerable));

			this.targetEnumerator = targetEnumerable.GetEnumerator();

			if ((object)this.targetEnumerator == null)
				throw new InvalidOperationException(nameof(this.targetEnumerator));

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
		private int visibleFieldCount;

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

		private bool HasRecord
		{
			get
			{
				return (object)this.TargetEnumerator.Current != null;
			}
		}

		public override bool HasRows
		{
			get
			{
				return this.HasRecord;
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

		/// <summary> Gets the number of fields in the <see cref="T:System.Data.Common.DbDataReader" /> that are not hidden. </summary>
		/// <returns> The number of fields that are not hidden. </returns>
		public override int VisibleFieldCount
		{
			get
			{
				return this.visibleFieldCount;
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

		/// <summary> Synchronously gets the value of the specified column as a type. </summary>
		/// <returns> The column to be retrieved. </returns>
		/// <param name="ordinal"> The column to be retrieved. </param>
		/// <typeparam name="T"> Synchronously gets the value of the specified column as a type. </typeparam>
		/// <exception cref="T:System.InvalidOperationException"> The connection drops or is closed during the data retrieval.The <see cref="T:System.Data.SqlClient.SqlDataReader" /> is closed during the data retrieval.There is no data ready to be read (for example, the first <see cref="M:System.Data.SqlClient.SqlDataReader.Read" /> hasn't been called, or returned false).Tried to read a previously-read column in sequential mode.There was an asynchronous operation in progress. This applies to all Get* methods when running in sequential mode, as they could be called while reading a stream. </exception>
		/// <exception cref="T:System.IndexOutOfRangeException"> Trying to read a column that does not exist. </exception>
		/// <exception cref="T:System.InvalidCastException">
		/// <paramref name="T" /> doesn’t match the type returned by SQL Server or cannot be cast.
		/// </exception>
		public override T GetFieldValue<T>(int ordinal)
		{
			return base.GetFieldValue<T>(ordinal);
		}

		/// <summary> Asynchronously gets the value of the specified column as a type. </summary>
		/// <returns> The type of the value to be returned. </returns>
		/// <param name="ordinal"> The type of the value to be returned. </param>
		/// <param name="cancellationToken"> The cancellation instruction, which propagates a notification that operations should be canceled. This does not guarantee the cancellation. A setting of CancellationToken.None makes this method equivalent to <see cref="M:System.Data.Common.DbDataReader.GetFieldValueAsync``1(System.Int32)" />. The returned task must be marked as cancelled. </param>
		/// <typeparam name="T"> The type of the value to be returned. See the remarks section for more information. </typeparam>
		/// <exception cref="T:System.InvalidOperationException"> The connection drops or is closed during the data retrieval.The <see cref="T:System.Data.Common.DbDataReader" /> is closed during the data retrieval.There is no data ready to be read (for example, the first <see cref="M:System.Data.Common.DbDataReader.Read" /> hasn't been called, or returned false).Tried to read a previously-read column in sequential mode.There was an asynchronous operation in progress. This applies to all Get* methods when running in sequential mode, as they could be called while reading a stream. </exception>
		/// <exception cref="T:System.IndexOutOfRangeException"> Trying to read a column that does not exist. </exception>
		/// <exception cref="T:System.InvalidCastException">
		/// <paramref name="T" /> doesn’t match the type returned by the data source or cannot be cast.
		/// </exception>
		public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
		{
			return base.GetFieldValueAsync<T>(ordinal, cancellationToken);
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

		/// <summary> Returns the provider-specific field type of the specified column. </summary>
		/// <returns> The <see cref="T:System.Type" /> object that describes the data type of the specified column. </returns>
		/// <param name="ordinal"> The zero-based column ordinal. </param>
		public override Type GetProviderSpecificFieldType(int ordinal)
		{
			return base.GetProviderSpecificFieldType(ordinal);
		}

		/// <summary> Gets the value of the specified column as an instance of <see cref="T:System.Object" />. </summary>
		/// <returns> The value of the specified column. </returns>
		/// <param name="ordinal"> The zero-based column ordinal. </param>
		public override object GetProviderSpecificValue(int ordinal)
		{
			return base.GetProviderSpecificValue(ordinal);
		}

		/// <summary> Gets all provider-specific attribute columns in the collection for the current row. </summary>
		/// <returns> The number of instances of <see cref="T:System.Object" /> in the array. </returns>
		/// <param name="values"> An array of <see cref="T:System.Object" /> into which to copy the attribute columns. </param>
		public override int GetProviderSpecificValues(object[] values)
		{
			return base.GetProviderSpecificValues(values);
		}

		/// <summary> Retrieves data as a <see cref="T:System.IO.Stream" />. </summary>
		/// <returns> The returned object. </returns>
		/// <param name="ordinal"> Retrieves data as a <see cref="T:System.IO.Stream" />. </param>
		/// <exception cref="T:System.InvalidOperationException"> The connection drops or is closed during the data retrieval.The <see cref="T:System.Data.Common.DbDataReader" /> is closed during the data retrieval.There is no data ready to be read (for example, the first <see cref="M:System.Data.Common.DbDataReader.Read" /> hasn't been called, or returned false).Tried to read a previously-read column in sequential mode.There was an asynchronous operation in progress. This applies to all Get* methods when running in sequential mode, as they could be called while reading a stream. </exception>
		/// <exception cref="T:System.IndexOutOfRangeException"> Trying to read a column that does not exist. </exception>
		/// <exception cref="T:System.InvalidCastException"> The returned type was not one of the types below:binaryimagevarbinaryudt </exception>
		public override Stream GetStream(int ordinal)
		{
			return base.GetStream(ordinal);
		}

		public override string GetString(int i)
		{
			return this.HasRecord ? (String)this.CurrentValues[i] : default(String);
		}

		/// <summary> Retrieves data as a <see cref="T:System.IO.TextReader" />. </summary>
		/// <returns> The returned object. </returns>
		/// <param name="ordinal"> Retrieves data as a <see cref="T:System.IO.TextReader" />. </param>
		/// <exception cref="T:System.InvalidOperationException"> The connection drops or is closed during the data retrieval.The <see cref="T:System.Data.Common.DbDataReader" /> is closed during the data retrieval.There is no data ready to be read (for example, the first <see cref="M:System.Data.Common.DbDataReader.Read" /> hasn't been called, or returned false).Tried to read a previously-read column in sequential mode.There was an asynchronous operation in progress. This applies to all Get* methods when running in sequential mode, as they could be called while reading a stream. </exception>
		/// <exception cref="T:System.IndexOutOfRangeException"> Trying to read a column that does not exist. </exception>
		/// <exception cref="T:System.InvalidCastException"> The returned type was not one of the types below:charncharntextnvarchartextvarchar </exception>
		public override TextReader GetTextReader(int ordinal)
		{
			return base.GetTextReader(ordinal);
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

		/// <summary> An asynchronous version of <see cref="M:System.Data.Common.DbDataReader.IsDBNull(System.Int32)" />, which gets a value that indicates whether the column contains non-existent or missing values. Optionally, sends a notification that operations should be cancelled. </summary>
		/// <returns> true if the specified column value is equivalent to DBNull otherwise false. </returns>
		/// <param name="ordinal"> The zero-based column to be retrieved. </param>
		/// <param name="cancellationToken"> The cancellation instruction, which propagates a notification that operations should be canceled. This does not guarantee the cancellation. A setting of CancellationToken.None makes this method equivalent to <see cref="M:System.Data.Common.DbDataReader.IsDBNullAsync(System.Int32)" />. The returned task must be marked as cancelled. </param>
		/// <exception cref="T:System.InvalidOperationException"> The connection drops or is closed during the data retrieval.The <see cref="T:System.Data.Common.DbDataReader" /> is closed during the data retrieval.There is no data ready to be read (for example, the first <see cref="M:System.Data.Common.DbDataReader.Read" /> hasn't been called, or returned false).Trying to read a previously read column in sequential mode.There was an asynchronous operation in progress. This applies to all Get* methods when running in sequential mode, as they could be called while reading a stream. </exception>
		/// <exception cref="T:System.IndexOutOfRangeException"> Trying to read a column that does not exist. </exception>
		public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
		{
			return base.IsDBNullAsync(ordinal, cancellationToken);
		}

		public override bool NextResult()
		{
			return false;
		}

		/// <summary> This is the asynchronous version of <see cref="M:System.Data.Common.DbDataReader.NextResult" />. Providers should override with an appropriate implementation. The <paramref name="cancellationToken" /> may optionally be ignored.The default implementation invokes the synchronous <see cref="M:System.Data.Common.DbDataReader.NextResult" /> method and returns a completed task, blocking the calling thread. The default implementation will return a cancelled task if passed an already cancelled <paramref name="cancellationToken" />. Exceptions thrown by <see cref="M:System.Data.Common.DbDataReader.NextResult" /> will be communicated via the returned Task Exception property.Other methods and properties of the DbDataReader object should not be invoked while the returned Task is not yet completed. </summary>
		/// <returns> A task representing the asynchronous operation. </returns>
		/// <param name="cancellationToken"> The cancellation instruction. </param>
		/// <exception cref="T:System.Data.Common.DbException"> An error occurred while executing the command text. </exception>
		public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
		{
			return base.NextResultAsync(cancellationToken);
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

		/// <summary> This is the asynchronous version of <see cref="M:System.Data.Common.DbDataReader.Read" />.  Providers should override with an appropriate implementation. The cancellationToken may optionally be ignored.The default implementation invokes the synchronous <see cref="M:System.Data.Common.DbDataReader.Read" /> method and returns a completed task, blocking the calling thread. The default implementation will return a cancelled task if passed an already cancelled cancellationToken.  Exceptions thrown by Read will be communicated via the returned Task Exception property.Do not invoke other methods and properties of the DbDataReader object until the returned Task is complete. </summary>
		/// <returns> A task representing the asynchronous operation. </returns>
		/// <param name="cancellationToken"> The cancellation instruction. </param>
		/// <exception cref="T:System.Data.Common.DbException"> An error occurred while executing the command text. </exception>
		public override Task<bool> ReadAsync(CancellationToken cancellationToken)
		{
			return base.ReadAsync(cancellationToken);
		}

		#endregion
	}
}