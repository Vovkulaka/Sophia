using Configuration;
using NLog;
using DataProvider.Infrastructure.Extantions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DataLyingInDB
{
    public class LieInDB
    {
        Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        static Logger LoggerStatic { get; } = LogManager.GetCurrentClassLogger();
                
        public int UpdateSource(int source, string procedureName, string _dataToUpload) 
        {
            try
            {
                Logger.AddLogStart($"LieInDB --> UpdateSource  <--");
                string _connectionString = AppSettings.DbConnectionString;
                UploadData(procedureName, _dataToUpload, _connectionString); // ОНОВЛЮЄ ДАТУ ОНОВЛЕННЯ БД
                SetUpdateDate(source, _connectionString); 
                return 1;
            }
            catch (Exception ex)
            {
                // ПРИ ВИКОРИСТВАННІ ЗМІННОЇ ex - падає
                //Logger.AddLogStart($"EXCEPTION   -->   LieInDB.UpdateSource: "); - ЦЕЙ НЕ ПРОВІРЯВ
                //Logger.AddLogException(ex); - падає
                return 0;
            }
        }

        private void SetUpdateDate(int sourceId, string _connectionString)
        {
            Logger.AddLogStart($"LieInDB --> UpdateSource --> SetUpdateDate <--");
            SqlParameter[] parameters =
            {
                new SqlParameter("@sourceId", sourceId)
            };
            ExecProcVoid(_connectionString, "[dbo].[proc_SetSorceUpdateDate]", parameters);
        }

        private void UploadData(string procName, string dataToUpload, string _connectionString)
        {
            Logger.AddLogStart($"LieInDB --> UpdateSource --> UploadData <--");
            UploadXmlData(procName, dataToUpload, _connectionString);
        }

        private static void UploadXmlData(string procName, string xmlDataToUpload, string _connectionString) // ТУТ
        {
            try
            {
                LoggerStatic.AddLogStart("LieInDB --> UpdateSource --> UploadData --> UploadXmlData <--");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@i_input", xmlDataToUpload)
                };

                ExecProcVoid(_connectionString, procName, parameters);
            }
            catch (Exception ex)
            {
                // ПРИ ВИКОРИСТВАННІ ЗМІННОЇ ex - падає
                // throw ex; - падає
                //LoggerStatic.AddLogStart("EXCEPTION  -->  UploadXmlData"); - ЦЕЙ НЕ ПРОВІРЯВ
                //LoggerStatic.AddLogException(ex); - падає
            }
        }

        private static void ExecProcVoid(string _connectionString, string procName, SqlParameter[] parameters = null)
        {
            LoggerStatic.AddLogStart("LieInDB --> UpdateSource --> UploadData --> SetUpdateDate --> ExecProcVoid <--");
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                LoggerStatic.AddLogStart("using (SqlConnection connection = new SqlConnection(_connectionString))");
                using (SqlCommand command = new SqlCommand())
                {
                    LoggerStatic.AddLogStart("using (SqlCommand command = new SqlCommand())");
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = procName;
                    command.CommandTimeout = 3000000;

                    if (parameters != null)
                    {
                        foreach (var item in parameters)
                        {
                            command.Parameters.Add(item);
                        }
                    }

                    LoggerStatic.AddLogStart("BEFORE: connection.Open()");
                    connection.Open();
                    LoggerStatic.AddLogStart("AFTER: connection.Open(). BEFORE: command.ExecuteNonQuery()");
                    command.ExecuteNonQuery();
                    LoggerStatic.AddLogStart("AFTER: command.ExecuteNonQuery()");
                }
            }
        }
    }
}
