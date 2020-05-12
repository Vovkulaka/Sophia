using DataProvider.DataLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.LogicLayer.Services
{
    public enum LogMessageType { Success, Info, Warning, Error }

    public static class BlackListLogService
    {
        public static async Task AddToLogAsync(LogMessage message)
        {
            string procName = "[dbo].[proc_AddMessageToLog]";
            SqlParameter[] parameters =
            {
                new SqlParameter("@i_sourceId", message.SourceId.ToString()),
                new SqlParameter("@i_messageType", message.MessageType.ToString()),
                new SqlParameter("@i_message", message.Message)
            };
            await SqlProcExecuter.ExecProcAsync(procName, parameters);
        }

        public static void AddToLog(LogMessage message)
        {
            string procName = "[dbo].[proc_AddMessageToLog]";
            SqlParameter[] parameters =
            {
                        new SqlParameter("@i_sourceId", message.SourceId.ToString()),
                        new SqlParameter("@i_messageType", message.MessageType.ToString()),
                        new SqlParameter("@i_message", message.Message)
                    };
            SqlProcExecuter.ExecProcVoid(procName, parameters);
        }
    }

    public class LogMessage
    {
        public int SourceId { get; set; }
        public LogMessageType MessageType { get; set; }
        public string Message { get; set; }
        public int Count { get; set; }
    }
}
