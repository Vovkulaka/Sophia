using DataProvider.DataLayer;
using DataProvider.Infrastructure.Extantions;
using DataProvider.Models;
using DataProvider.Models.ConcreteLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataProvider
{
	public class DataProviderMain
    {
		private Logger Logger { get; } = LogManager.GetCurrentClassLogger();

		public async Task<List<string>> DownloadSource(int source)
        {
			Logger.AddLogStart("DataProviderMain: ");
			Logger.AddLogStart($"source: " + source);
			var collection = new List<string>();
			Logger.AddLogStart($"collection.Count: " + collection.Count);

			try
	        {
				Logger.AddLogStart($"DownloadSource -> try");
		        var sqlProcExecuter = new SqlProcExecuter();
				Logger.AddLogStart($"sqlProcExecuter: " + sqlProcExecuter.ToString());
				var factory = await FactoryMethod.GetFactory(source);
				Logger.AddLogStart($"factory1: " + factory.ToString());
				var client = new FactoryClient(factory);
				Logger.AddLogStart($"client: " + client.ToString());

				collection.Add(client._dataToUpload.XmlDataToUpload);
		        collection.Add(client._urlList.UrlList[0]);
				Logger.AddLogStart($"return collection.Count: " + collection.Count);


				return collection;
	        }
	        catch (Exception ex)
	        {
				Console.WriteLine(ex);
				Logger.AddLogException(ex);

				throw ex;
			} 
	        // додай логування
        }
    }
}
