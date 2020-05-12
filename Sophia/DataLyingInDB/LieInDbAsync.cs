using Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataLyingInDB
{
    public class LieInDbAsync
    {
        public async Task<int> UpdateSourceAsync(int source, string procedureName, string _dataToUpload)
        {
            try
            {
                string _connectionString = AppSettings.DbConnectionString;
                await UploadDataAsync(procedureName, _dataToUpload, _connectionString);
                await SetUpdateDateAsync(source, _connectionString);
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private static async Task SetUpdateDateAsync(int sourceId, string _connectionString)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@sourceId", sourceId)
            };
            await ExecProcAsync(_connectionString, "[dbo].[proc_SetSorceUpdateDate]", parameters);
        }

        private async Task UploadDataAsync(string procName, string dataToUpload, string _connectionString)
        {
            await UploadXmlDataAsync(procName, dataToUpload, _connectionString);
        }

        private static async Task UploadXmlDataAsync(string procName, string xmlDataToUpload, string _connectionString)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@i_input", xmlDataToUpload)
                };

                await ExecProcAsync(_connectionString, procName, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task ExecProcAsync(string _connectionString, string procName, SqlParameter[] parameters = null)
        {
            await using var connection = new SqlConnection(_connectionString);
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }
            await using SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandType = CommandType.StoredProcedure,
                CommandText = procName,
                CommandTimeout = 3000000
            };

            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    command.Parameters.Add(item);
                }
            }
            await command.ExecuteNonQueryAsync();
        }
    }
}
