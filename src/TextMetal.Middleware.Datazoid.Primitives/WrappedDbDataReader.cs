/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Datazoid.Primitives
{
	public abstract class WrappedDbDataReader : DbDataReader, IDbColumnSchemaGenerator
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

		/// <summary> Gets the value of the specified column as an instance of <see cref="T:System.Object" />. </summary>
		/// <returns> The value of the specified column. </returns>
		/// <param name="name"> The name of the column. </param>
		/// <exception cref="T:System.IndexOutOfRangeException"> No column with the specified name was found. </exception>
		/// <filterpriority> 1 </filterpriority>
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

		/// <summary> Gets a value indicating the depth of nesting for the current row. </summary>
		/// <returns> The depth of nesting for the current row. </returns>
		/// <filterpriority> 1 </filterpriority>
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

		/// <summary> Gets a value that indicates whether this <see cref="T:System.Data.Common.DbDataReader" /> contains one or more rows. </summary>
		/// <returns> true if the <see cref="T:System.Data.Common.DbDataReader" /> contains one or more rows; otherwise false. </returns>
		/// <filterpriority> 1 </filterpriority>
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

		/// <summary> Gets a value indicating whether the <see cref="T:System.Data.Common.DbDataReader" /> is closed. </summary>
		/// <returns> true if the <see cref="T:System.Data.Common.DbDataReader" /> is closed; otherwise false. </returns>
		/// <exception cref="T:System.InvalidOperationException"> The <see cref="T:System.Data.SqlClient.SqlDataReader" /> is closed. </exception>
		/// <filterpriority> 1 </filterpriority>
		public override bool IsClosed
		{
			get
			{
				return this.InnerDbDataReader.IsClosed;
			}
		}

		/// <summary> Gets the number of rows changed, inserted, or deleted by execution of the SQL statement. </summary>
		/// <returns> The number of rows changed, inserted, or deleted. -1 for SELECT statements; 0 if no rows were affected or the statement failed. </returns>
		/// <filterpriority> 1 </filterpriority>
		public override int RecordsAffected
		{
			get
			{
				return this.InnerDbDataReader.RecordsAffected;
			}
		}

		/// <summary> Gets the number of fields in the <see cref="T:System.Data.Common.DbDataReader" /> that are not hidden. </summary>
		/// <returns> The number of fields that are not hidden. </returns>
		public override int VisibleFieldCount
		{
			get
			{
				return this.InnerDbDataReader.VisibleFieldCount;
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

		/// <summary> Releases the managed resources used by the <see cref="T:System.Data.Common.DbDataReader" /> and optionally releases the unmanaged resources. </summary>
		/// <param name="disposing"> true to release managed and unmanaged resources; false to release only unmanaged resources. </param>
		protected override void Dispose(bool disposing)
		{
			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::Dispose(...): enter", typeof(WrappedDbDataReader).Name));

			if (disposing)
				this.InnerDbDataReader.Dispose();

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::Dispose(...): leave", typeof(WrappedDbDataReader).Name));
		}

		/// <summary> Gets the value of the specified column as a Boolean. </summary>
		/// <returns> The value of the specified column. </returns>
		/// <param name="ordinal"> The zero-based column ordinal. </param>
		/// <exception cref="T:System.InvalidCastException"> The specified cast is not valid. </exception>
		/// <filterpriority> 1 </filterpriority>
		public override bool GetBoolean(int i)
		{
			return this.InnerDbDataReader.GetBoolean(i);
		}

		/// <summary> Gets the value of the specified column as a byte. </summary>
		/// <returns> The value of the specified column. </returns>
		/// <param name="ordinal"> The zero-based column ordinal. </param>
		/// <exception cref="T:System.InvalidCastException"> The specified cast is not valid. </exception>
		/// <filterpriority> 1 </filterpriority>
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

		public ReadOnlyCollection<DbColumn> GetColumnSchema()
		{
			IDbColumnSchemaGenerator dbColumnSchemaGenerator;

			dbColumnSchemaGenerator = this.InnerDbDataReader as IDbColumnSchemaGenerator;

			if ((object)dbColumnSchemaGenerator != null)
				return dbColumnSchemaGenerator.GetColumnSchema();

			return null;
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

		/// <summary> Returns a <see cref="T:System.Data.Common.DbDataReader" /> object for the requested column ordinal that can be overridden with a provider-specific implementation. </summary>
		/// <returns> A <see cref="T:System.Data.Common.DbDataReader" /> object. </returns>
		/// <param name="ordinal"> The zero-based column ordinal. </param>
		protected override DbDataReader GetDbDataReader(int ordinal)
		{
			// CANNOT USE INNER INSTANCE HERE
			return base.GetDbDataReader(ordinal);
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
			return this.InnerDbDataReader.GetFieldValue<T>(ordinal);
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
			return this.InnerDbDataReader.GetFieldValueAsync<T>(ordinal, cancellationToken);
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

		/// <summary> Returns the provider-specific field type of the specified column. </summary>
		/// <returns> The <see cref="T:System.Type" /> object that describes the data type of the specified column. </returns>
		/// <param name="ordinal"> The zero-based column ordinal. </param>
		public override Type GetProviderSpecificFieldType(int ordinal)
		{
			return this.InnerDbDataReader.GetProviderSpecificFieldType(ordinal);
		}

		/// <summary> Gets the value of the specified column as an instance of <see cref="T:System.Object" />. </summary>
		/// <returns> The value of the specified column. </returns>
		/// <param name="ordinal"> The zero-based column ordinal. </param>
		public override object GetProviderSpecificValue(int ordinal)
		{
			return this.InnerDbDataReader.GetProviderSpecificValue(ordinal);
		}

		/// <summary> Gets all provider-specific attribute columns in the collection for the current row. </summary>
		/// <returns> The number of instances of <see cref="T:System.Object" /> in the array. </returns>
		/// <param name="values"> An array of <see cref="T:System.Object" /> into which to copy the attribute columns. </param>
		public override int GetProviderSpecificValues(object[] values)
		{
			return this.InnerDbDataReader.GetProviderSpecificValues(values);
		}

		/// <summary> Retrieves data as a <see cref="T:System.IO.Stream" />. </summary>
		/// <returns> The returned object. </returns>
		/// <param name="ordinal"> Retrieves data as a <see cref="T:System.IO.Stream" />. </param>
		/// <exception cref="T:System.InvalidOperationException"> The connection drops or is closed during the data retrieval.The <see cref="T:System.Data.Common.DbDataReader" /> is closed during the data retrieval.There is no data ready to be read (for example, the first <see cref="M:System.Data.Common.DbDataReader.Read" /> hasn't been called, or returned false).Tried to read a previously-read column in sequential mode.There was an asynchronous operation in progress. This applies to all Get* methods when running in sequential mode, as they could be called while reading a stream. </exception>
		/// <exception cref="T:System.IndexOutOfRangeException"> Trying to read a column that does not exist. </exception>
		/// <exception cref="T:System.InvalidCastException"> The returned type was not one of the types below:binaryimagevarbinaryudt </exception>
		public override Stream GetStream(int ordinal)
		{
			return this.InnerDbDataReader.GetStream(ordinal);
		}

		//public override DataTable GetSchemaTable()
		//{
		//	return this.InnerDbDataReader.GetSchemaTable();
		//}

		public override string GetString(int i)
		{
			return this.InnerDbDataReader.GetString(i);
		}

		/// <summary> Retrieves data as a <see cref="T:System.IO.TextReader" />. </summary>
		/// <returns> The returned object. </returns>
		/// <param name="ordinal"> Retrieves data as a <see cref="T:System.IO.TextReader" />. </param>
		/// <exception cref="T:System.InvalidOperationException"> The connection drops or is closed during the data retrieval.The <see cref="T:System.Data.Common.DbDataReader" /> is closed during the data retrieval.There is no data ready to be read (for example, the first <see cref="M:System.Data.Common.DbDataReader.Read" /> hasn't been called, or returned false).Tried to read a previously-read column in sequential mode.There was an asynchronous operation in progress. This applies to all Get* methods when running in sequential mode, as they could be called while reading a stream. </exception>
		/// <exception cref="T:System.IndexOutOfRangeException"> Trying to read a column that does not exist. </exception>
		/// <exception cref="T:System.InvalidCastException"> The returned type was not one of the types below:charncharntextnvarchartextvarchar </exception>
		public override TextReader GetTextReader(int ordinal)
		{
			return this.InnerDbDataReader.GetTextReader(ordinal);
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

		/// <summary> An asynchronous version of <see cref="M:System.Data.Common.DbDataReader.IsDBNull(System.Int32)" />, which gets a value that indicates whether the column contains non-existent or missing values. Optionally, sends a notification that operations should be cancelled. </summary>
		/// <returns> true if the specified column value is equivalent to DBNull otherwise false. </returns>
		/// <param name="ordinal"> The zero-based column to be retrieved. </param>
		/// <param name="cancellationToken"> The cancellation instruction, which propagates a notification that operations should be canceled. This does not guarantee the cancellation. A setting of CancellationToken.None makes this method equivalent to <see cref="M:System.Data.Common.DbDataReader.IsDBNullAsync(System.Int32)" />. The returned task must be marked as cancelled. </param>
		/// <exception cref="T:System.InvalidOperationException"> The connection drops or is closed during the data retrieval.The <see cref="T:System.Data.Common.DbDataReader" /> is closed during the data retrieval.There is no data ready to be read (for example, the first <see cref="M:System.Data.Common.DbDataReader.Read" /> hasn't been called, or returned false).Trying to read a previously read column in sequential mode.There was an asynchronous operation in progress. This applies to all Get* methods when running in sequential mode, as they could be called while reading a stream. </exception>
		/// <exception cref="T:System.IndexOutOfRangeException"> Trying to read a column that does not exist. </exception>
		public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
		{
			return this.InnerDbDataReader.IsDBNullAsync(ordinal, cancellationToken);
		}

		public override bool NextResult()
		{
			bool retval;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::NextResult(...): enter", typeof(WrappedDbDataReader).Name));

			retval = this.InnerDbDataReader.NextResult();

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::NextResult(...): return flag", typeof(WrappedDbDataReader).Name));

			return retval;
		}

		/// <summary> This is the asynchronous version of <see cref="M:System.Data.Common.DbDataReader.NextResult" />. Providers should override with an appropriate implementation. The <paramref name="cancellationToken" /> may optionally be ignored.The default implementation invokes the synchronous <see cref="M:System.Data.Common.DbDataReader.NextResult" /> method and returns a completed task, blocking the calling thread. The default implementation will return a cancelled task if passed an already cancelled <paramref name="cancellationToken" />. Exceptions thrown by <see cref="M:System.Data.Common.DbDataReader.NextResult" /> will be communicated via the returned Task Exception property.Other methods and properties of the DbDataReader object should not be invoked while the returned Task is not yet completed. </summary>
		/// <returns> A task representing the asynchronous operation. </returns>
		/// <param name="cancellationToken"> The cancellation instruction. </param>
		/// <exception cref="T:System.Data.Common.DbException"> An error occurred while executing the command text. </exception>
		public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
		{
			return this.InnerDbDataReader.NextResultAsync(cancellationToken);
		}

		public override bool Read()
		{
			bool retval;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::Read(...): enter", typeof(WrappedDbDataReader).Name));

			retval = this.InnerDbDataReader.Read();

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::Read(...): return flag", typeof(WrappedDbDataReader).Name));

			return retval;
		}

		/// <summary> This is the asynchronous version of <see cref="M:System.Data.Common.DbDataReader.Read" />.  Providers should override with an appropriate implementation. The cancellationToken may optionally be ignored.The default implementation invokes the synchronous <see cref="M:System.Data.Common.DbDataReader.Read" /> method and returns a completed task, blocking the calling thread. The default implementation will return a cancelled task if passed an already cancelled cancellationToken.  Exceptions thrown by Read will be communicated via the returned Task Exception property.Do not invoke other methods and properties of the DbDataReader object until the returned Task is complete. </summary>
		/// <returns> A task representing the asynchronous operation. </returns>
		/// <param name="cancellationToken"> The cancellation instruction. </param>
		/// <exception cref="T:System.Data.Common.DbException"> An error occurred while executing the command text. </exception>
		public override Task<bool> ReadAsync(CancellationToken cancellationToken)
		{
			return this.InnerDbDataReader.ReadAsync(cancellationToken);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		internal sealed class __ : WrappedDbDataReader
		{
			#region Constructors/Destructors

			public __(DbDataReader innerDbDataReader)
				: base(innerDbDataReader)
			{
			}

			#endregion
		}

		#endregion
	}
}