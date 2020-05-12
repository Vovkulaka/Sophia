using Configuration;
using DataProvider.DataLayer;
using DataProvider.Infrastructure.Extantions;
using DataProvider.LogicLayer.Services;
using DataProvider.Models;
using DataProvider.Models.ConcreteLayer;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DataProvider.DataLayer
{
    public class SqlProcExecuter
    {
        private Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        private static Logger LoggerStatic { get; } = LogManager.GetCurrentClassLogger();


        // BlackListLogService BlackListLogService { get; }
        static string _connectionString = null;
        internal SqlProcExecuter()
        {
            Logger.AddLogStart($"Constructor class SqlProcExecuter: ");
            _connectionString = AppSettings.DbConnectionString;
            Logger.AddLogStart($"_connectionString: " + _connectionString);
        }

        public static async Task ExecProcAsync(string procName, SqlParameter[] parameters = null)
        {
            if (_connectionString == null) _connectionString = AppSettings.DbConnectionString;

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

        public static void ExecProcVoid(string procName, SqlParameter[] parameters = null)
        {
            LoggerStatic.AddLogStart($"ExecProcVoid: " + "11111111111");
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
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

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static async Task<DataTable> ExecProcDataSetAsync(string procName, SqlParameter[] parameters = null)
        {
            var table = new DataTable();

            await using (var connection = new SqlConnection(_connectionString))
            {
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
                var dataReader = await command.ExecuteReaderAsync();

                var da = new SqlDataAdapter
                {
                    SelectCommand = command
                };

                table.Load(dataReader);
            }
            return table;
        }


        public static DataSet ExecProcDataSet(string procName, SqlParameter[] parameters = null)
        {
            DataSet dataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
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

                        connection.Open();

                        SqlDataAdapter da = new SqlDataAdapter
                        {
                            SelectCommand = command
                        };
                        da.Fill(dataSet);
                    }
                }
            }
            catch (System.Exception)
            {
                //LogMessage logMessage = new LogMessage { SourceId = -1, MessageType = LogMessageType.Error, Message = $"ExecProcDataSet canch exception: {ex.Message}" };
                //BlackListLogService.AddToLog(logMessage);
            }
            return dataSet;
        }

        public static async Task<object> ExecProcOutParameterAsync(string procName, SqlParameter outParameter, SqlParameter[] parameters = null)
        {
            LoggerStatic.AddLogStart("ExecProcOutParameterAsync(): ");
            LoggerStatic.AddLogStart($"procName: " + procName);
            LoggerStatic.AddLogStart($"outParameter: " + outParameter.ToString());
            LoggerStatic.AddLogStart($"parameters: " + parameters.Length);

            await using (var connection = new SqlConnection(_connectionString))
	        {
                LoggerStatic.AddLogStart($"_connectionString: " + _connectionString);
                LoggerStatic.AddLogStart($"connection: " + connection.ToString());
		        if (connection.State == ConnectionState.Closed)
	            {
		            await connection.OpenAsync();
	            }
                LoggerStatic.AddLogStart($"connection.State: " + connection.State.ToString());

	            await using var command = new SqlCommand
	            {
		            Connection = connection,
		            CommandType = CommandType.StoredProcedure,
		            CommandText = procName,
		            CommandTimeout = 3000000
                };
                LoggerStatic.AddLogStart($"command0: " + command.ToString() + "quantity: " + command.Parameters.Count);

	            if (parameters != null)
	            {
		            foreach (var item in parameters)
		            {
			            command.Parameters.Add(item);
		            }
	            }

	            outParameter.Direction = ParameterDirection.Output;
	            command.Parameters.Add(outParameter);
                LoggerStatic.AddLogStart($"command1: " + command.ToString() + "quantity: " + command.Parameters.Count);

	            try
	            {
                    LoggerStatic.AddLogStart("ExecProcOutParameterAsync(): ");
		            await command.ExecuteNonQueryAsync();
	            }
	            catch (Exception ex)
	            {
                    LoggerStatic.AddLogException(ex);
                    throw new Exception(ex.Message, ex);
	            }
	            finally
	            {
		            await connection.CloseAsync();
	            }
            }
            return outParameter.Value;
        }
    }
}
