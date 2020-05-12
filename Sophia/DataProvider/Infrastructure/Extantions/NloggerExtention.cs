using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace DataProvider.Infrastructure.Extantions
{
	public static class NloggerExtention
	{
        private static string CutSringOnDot(string input, int number)
        {
            string sophiaUploader = String.Empty;
            string returWord = String.Empty;
            string[] resultwords = new string[40];
            String[] words = input.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            ConcurrentBag<string> collection = new ConcurrentBag<string>(); // ConcurrentBag не містить ключа. Ключ є в ConcurrentDictionary<int, MessageModel>
            //List<Task> resultwords = new List<Task>(); - https://docs.microsoft.com/ru-ru/dotnet/api/system.collections.concurrent.concurrentbag-1?view=netcore-3.1

            foreach (string match in words)
            {
                if (match == "Sophia") sophiaUploader = match;
                else if (sophiaUploader == "Sophia" && match == "Uploader")
                {
                    sophiaUploader += "." + match;
                    collection.Add(sophiaUploader);
                }
                else collection.Add(match);
            }

            resultwords = collection.ToArray();
            Array.Reverse(resultwords, 0, resultwords.Length);

			try
            {
                string value = String.Empty;

                if (resultwords[number] == "Sophia.FrontApi") value = $"1 {resultwords[number]}";
				else if(resultwords[number] == "Sophia.Uploader") value = $"2 {resultwords[number]}";
				else value = resultwords[number];

                returWord = value;
            }
            catch (Exception ex)
            {
                if (ex.Message == "Index was outside the bounds of the array.") returWord = resultwords[0];
            }
            return returWord;
        }

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
				strParameter = JsonConvert.SerializeObject(new { Request = request }, Formatting.None);
			}

			LogEventInfo logEvent = new LogEventInfo(LogLevel.Warn, NLogger.Name, strParameter); // LogLevel.Error - не працює!

			logEvent.Properties["service"] = CutSringOnDot(NLogger.Name, 0);
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
				strParameter = JsonConvert.SerializeObject(new { Response = response }, Formatting.None);
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
