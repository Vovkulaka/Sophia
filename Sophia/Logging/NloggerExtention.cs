using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Logging
{
	public static class NloggerExtention
    {
        public static void AddLogStart(
			this Logger NLogger,
			object request = null,
			[CallerMemberName]string callerMethodName = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0)
		{
			string strParameter = string.Empty;
			if (request == null)
			{
				strParameter = "Started without parameters.";
			}
			else
			{
				strParameter = JsonConvert.SerializeObject(
                    new { Request = request }, Newtonsoft.Json.Formatting.None);
			}

			LogEventInfo logEvent = new LogEventInfo(
                LogLevel.Info, NLogger.Name, strParameter);

            logEvent.Properties["method"] = callerMethodName;
			logEvent.Properties["filePath"] = sourceFilePath;
			logEvent.Properties["lineNumber"] = sourceLineNumber;

			NLogger.Log(logEvent);
		}

		public static void AddLogFinish(
			this Logger NLogger,
			object response = null,
			[CallerMemberName]string callerMethodName = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0)
		{
			string strParameter = string.Empty;

			if (response == null)
			{
				strParameter = "Finished without result.";
			}
			else
			{
				strParameter = JsonConvert.SerializeObject(new { Response = response }, Newtonsoft.Json.Formatting.None);
			}

			LogEventInfo logEvent = new LogEventInfo(LogLevel.Info, NLogger.Name, strParameter);

			logEvent.Properties["method"] = callerMethodName;
			logEvent.Properties["filePath"] = sourceFilePath;
			logEvent.Properties["lineNumber"] = sourceLineNumber;

			NLogger.Log(logEvent);
		}

		public static void AddLogException(this Logger NLogger,
			Exception exception,
			[CallerMemberName]string callerMethodName = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0)
		{
			LogEventInfo logEvent = new LogEventInfo(LogLevel.Error, NLogger.Name, "Exception");
			logEvent.Exception = exception;
			logEvent.Properties["method"] = callerMethodName;
			logEvent.Properties["filePath"] = sourceFilePath;
			logEvent.Properties["lineNumber"] = sourceLineNumber;
			NLogger.Log(logEvent);
		}
	}
}
