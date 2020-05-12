using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Logging;

namespace Configuration
{
	public class ConfigConnection : ConfigRabbit
	{
		private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

		public static string DbConnectionString
		{
			get
			{
				Logger.AddLogStart($"ConfigConnection -> connectionString: " + Environment.GetEnvironmentVariable("DbConnectionString"));

                return ($"{Environment.GetEnvironmentVariable("DbConnectionString")};");

                #region From Vostok
				//Logger.AddLogStart($"ConfigConnection -> connectionStringPassword: " + Environment.GetEnvironmentVariable("DbConnectionStringPassword"));

                //return ($"{Environment.GetEnvironmentVariable("DbConnectionString")};" +
                //                    $"Password={GetSecret("DbConnectionStringPassword")};").
                //                        Replace(";;", ";").Replace("; ;", ";");
                #endregion
            }
		}

		private static string GetSecret(string key)
		{
			string _value = Environment.GetEnvironmentVariable(key);
			Regex rgx = new Regex(@"^SECRETS:(?<path>.*)$");
			Match m = rgx.Match(_value);

			if (m.Success) _value = File.ReadAllText(m.Groups[rgx.GroupNumberFromName("path")].Value);

			return _value;
		}
	}
}
