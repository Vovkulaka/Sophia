using DataProvider.Infrastructure.Extantions;
using NLog;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DataProvider.DataLayer
{
    public class DbAdapter
    {
        private static Logger LoggerStatic { get; } = LogManager.GetCurrentClassLogger();

        public static async Task<string> GetSourceName(int sourceId)
        {
            LoggerStatic.AddLogStart("GetSourceName(): ");
            string procName = "[dbo].[proc_GetLoadingStrategy]";
            SqlParameter[] parameters = //null;
            {
                new SqlParameter("@i_sourceId", sourceId.ToString())
            };
            LoggerStatic.AddLogStart($"parameters: " + parameters.Length);

            SqlParameter outParameter = new SqlParameter("@o_strategyName", SqlDbType.NVarChar, -1);
            //LoggerStatic.AddLogStart($"outParameter: " + outParameter.Value.ToString());
            LoggerStatic.AddLogStart($"outParameter" + outParameter.ToString());

            object obj = await SqlProcExecuter.ExecProcOutParameterAsync(procName, outParameter, parameters);
            LoggerStatic.AddLogStart($"obj: " + obj.ToString());

            return obj.ToString();
        }

        public static async Task<string> GetSourcePackageLink(int sourceId)
        {
            string procName = "[dbo].[proc_GetSourceById]";
            SqlParameter[] parameters =
            {
                new SqlParameter("@i_sourceId", sourceId)
            };
            var outParameter = new SqlParameter("@o_sourceURL", SqlDbType.NVarChar, -1);

            var obj = await SqlProcExecuter.ExecProcOutParameterAsync(procName, outParameter, parameters);

            return obj.ToString();
        }

        public static async Task<int> GetTableRowsCount(string tableName)
        {
            int rowCount = -1;
            
            SqlParameter[] parameters =
            {
                new SqlParameter("@i_tableName", tableName)
            };
            
            var outParameter = new SqlParameter("@o_count", SqlDbType.Int, -1);

            object obj = await SqlProcExecuter.ExecProcOutParameterAsync("[dbo].[proc_GetRecordCount]", outParameter, parameters);
            Int32.TryParse(obj.ToString(), out rowCount);

            return rowCount;
        }

        public static string SearchBlackListData(string document)
        {
            string procName = "[dbo].[proc_GetSophiaOpenDataSearchResult]";
            SqlParameter[] parameters =
            {
                new SqlParameter("@i_inputXml", document)
            };
            DataSet dataSet = SqlProcExecuter.ExecProcDataSet(procName, parameters);
            DataTable table = dataSet.Tables[0];
            string result = string.Empty;
            for (int i = 0; i < table.Rows.Count; ++i)
            {
                result += table.Rows[i].ItemArray[1].ToString();
            }
            result = "<SearchResult>" + result + "</SearchResult>";
            Console.WriteLine($"Длина ответа {result.Length}");
            return result;
        }

        public async static Task<string> SearchBlackListDataAsync(string document)
        {
            string procName = "[dbo].[proc_GetSophiaOpenDataSearchResult]";
            SqlParameter[] parameters =
            {
                new SqlParameter("@i_inputXml", document)
            };

            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();

            DataTable table = await SqlProcExecuter.ExecProcDataSetAsync(procName, parameters);

            //stopwatch.Stop();
            //long ms = stopwatch.ElapsedMilliseconds;
            //LogMessage logMessage = new LogMessage { SourceId = -1, MessageType = LogMessageType.Info, Message = $"time: {ms}" };
            //BlackListDataLogger.AddToLog(logMessage);

            string result = string.Empty;
            for (int i = 0; i < table.Rows.Count; ++i)
            {
                result += table.Rows[i].ItemArray[1].ToString();
            }
            result = "<SearchResult>" + result + "</SearchResult>";
            Console.WriteLine($"Длина ответа {result.Length}");
            return result;
        }
    }
}
