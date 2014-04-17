/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

using Microsoft.SqlServer.Server;

using TextMetal.Common.Core;

namespace TextMetal.Common.SqlServerClr
{
	public static class TableValueFunctions
	{
		#region Fields/Constants

		private const string CONNECTION_STRING = @"data source=(local);initial catalog=textmetal_ods_dev;integrated security=SSPI;enlist=false";

		#endregion

		#region Methods/Operators

		private static void DbConnection_Disposed(object sender, EventArgs e)
		{
			((Component)sender).Disposed -= DbConnection_Disposed;

			//WriteEventLog("DbConnection_Disposed");
		}

		private static void WriteEventLog(object state)
		{
			using (IDbConnection dbConnection = new SqlConnection())
			{
				dbConnection.ConnectionString = CONNECTION_STRING;
				//((Component)dbConnection).Disposed += DbConnection_Disposed;
				dbConnection.Open();

				using (IDbCommand dbCommmand = dbConnection.CreateCommand())
				{
					// yes this is sucky SQL injection code
					dbCommmand.CommandText = string.Format(@"INSERT INTO [textmetal_ods_dev_Proxy].[dbo].[EventLog] VALUES ('{0}', GetUtcDate())", state.SafeToString());

					dbCommmand.ExecuteNonQuery();
				}
			}
		}

		[SqlFunction(
			DataAccess = DataAccessKind.None,
			SystemDataAccess = SystemDataAccessKind.None,
			TableDefinition = @"[UserId] [int] NULL,
								[EmailAddress] [nvarchar](255) NULL,
								[UserName] [nvarchar](255) NULL,
								[SaltValue] [nvarchar](255) NULL,
								[PasswordHash] [nvarchar](255) NULL,	
								[Question] [nvarchar](255) NULL,
								[AnswerHash] [nvarchar](255) NULL,
								[LastLoginSuccessTimestamp] [datetime] NULL,
								[LastLoginFailureTimestamp] [datetime] NULL,
								[FailedLoginCount] [smallint] NULL,
								[MustChangePassword] [bit] NULL,
								[SortOrder] [tinyint] NULL,
								[CreationTimestamp] [datetime] NULL,
								[ModificationTimestamp] [datetime] NULL,
								[CreationUserId] [int] NULL,
								[ModificationUserId] [int] NULL,
								[LogicalDelete] [bit] NULL",
			FillRowMethodName = "stv_fn_GlobalUserFillRow"
			)]
		public static IEnumerable stv_fn_GlobalUser(int? top)
		{
			using (IDbConnection dbConnection = new SqlConnection())
			{
				dbConnection.ConnectionString = CONNECTION_STRING;
				((Component)dbConnection).Disposed += DbConnection_Disposed;
				dbConnection.Open();

				using (IDbCommand dbCommmand = dbConnection.CreateCommand())
				{
					// yes this is sucky SQL injection code
					dbCommmand.CommandText = @"SELECT " +
											((object)top != null ? @"TOP (" + top + @") " : "") +
											@"t.[UserId],
												t.[EmailAddress],
												t.[UserName],
												t.[SaltValue],
												t.[PasswordHash],
												t.[Question],
												t.[AnswerHash],
												t.[LastLoginSuccessTimestamp],
												t.[LastLoginFailureTimestamp],
												t.[FailedLoginCount],
												t.[MustChangePassword],
												t.[SortOrder],
												t.[CreationTimestamp],
												t.[ModificationTimestamp],
												t.[CreationUserId],
												t.[ModificationUserId],
												t.[LogicalDelete]
												FROM [global].[User] t"; /* [textmetal_ods_dev]*/

					using (IDataReader dataReader = dbCommmand.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (dataReader.Read())
						{
							yield return new object[]
										{
											dataReader.GetValue(0).ChangeType<Int32?>(),
											dataReader.GetValue(1).ChangeType<String>(),
											dataReader.GetValue(2).ChangeType<String>(),
											dataReader.GetValue(3).ChangeType<String>(),
											dataReader.GetValue(4).ChangeType<String>(),
											dataReader.GetValue(5).ChangeType<String>(),
											dataReader.GetValue(6).ChangeType<String>(),
											dataReader.GetValue(7).ChangeType<DateTime?>(),
											dataReader.GetValue(8).ChangeType<DateTime?>(),
											dataReader.GetValue(9).ChangeType<Int16?>(),
											dataReader.GetValue(10).ChangeType<Boolean?>(),
											dataReader.GetValue(11).ChangeType<Byte?>(),
											dataReader.GetValue(12).ChangeType<DateTime?>(),
											dataReader.GetValue(13).ChangeType<DateTime?>(),
											dataReader.GetValue(14).ChangeType<Int32?>(),
											dataReader.GetValue(15).ChangeType<Int32?>(),
											dataReader.GetValue(16).ChangeType<Boolean?>()
										};
						}
					}
				}
			}
		}

		public static void stv_fn_GlobalUserFillRow(object obj,
			out int? userId,
			out string emailAddress,
			out string userName,
			out string saltValue,
			out string passwordHash,
			out string question,
			out string answerHash,
			out DateTime? lastLoginSuccessTimestamp,
			out DateTime? lastLoginFailureTimestamp,
			out short? failedLoginCount,
			out bool? mustChangePassword,
			out byte? sortOrder,
			out DateTime? creationTimestamp,
			out DateTime? modificationTimestamp,
			out int? creationUserId,
			out int? modificationUserId,
			out bool? logicalDelete)
		{
			userId = (int?)((object[])obj)[0];
			emailAddress = (string)((object[])obj)[1];
			userName = (string)((object[])obj)[2];
			saltValue = (string)((object[])obj)[3];
			passwordHash = (string)((object[])obj)[4];
			question = (string)((object[])obj)[5];
			answerHash = (string)((object[])obj)[6];
			lastLoginSuccessTimestamp = (DateTime?)((object[])obj)[7];
			lastLoginFailureTimestamp = (DateTime?)((object[])obj)[8];
			failedLoginCount = (short?)((object[])obj)[9];
			mustChangePassword = (bool?)((object[])obj)[10];
			sortOrder = (byte?)((object[])obj)[11];
			creationTimestamp = (DateTime?)((object[])obj)[12];
			modificationTimestamp = (DateTime?)((object[])obj)[13];
			creationUserId = (int?)((object[])obj)[14];
			modificationUserId = (int?)((object[])obj)[15];
			logicalDelete = (bool?)((object[])obj)[16];

			//WriteEventLog("stv_fn_GlobalUserFillRow");
		}

		#endregion
	}
}