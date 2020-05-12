using DataProvider.Infrastructure.Extantions;
using DataProvider.DataLayer;
using MessageModelLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace NotaryProvider.Services
{
    public interface IGetDataService
    {
        Task<List<JObject>> GetData(MessageRequestModel data);
    }
    public class GetDataService : IGetDataService
    {
        Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        IReturnResponseService _returnResponseService;

        public GetDataService(IReturnResponseService returnResponseService)
        {
            _returnResponseService = returnResponseService;
        }

        public GetDataService() {}

        public Task<List<JObject>> GetData(MessageRequestModel data)
        {
            Logger.AddLogStart($"Method: GetData(MessageRequestModel data)");

            List<JObject> responseData = new List<JObject>();
            ConcurrentBag<Task<string>> cBag = new ConcurrentBag<Task<string>>();

            foreach (var item in data.Data)
            {
                cBag.Add(SeachBlackListAsync(item));
            }

            Task.WaitAll(cBag.ToArray());

            foreach (var item in cBag)
            {
                responseData.Add(JObject.Parse(item.Result));
                Logger.AddLogStart($"RESPONSE IN GetData: {item.Result}");
            }

            return _returnResponseService.SendResponseInQueue(responseData, data.ProviderName, data.IdRequest);
        }

        private async Task<string> SeachBlackListAsync(JObject jsonObj)
        {

            Logger.AddLogStart($"Method: SeachBlackListAsync(JObject jsonObj)");

            string reqId = jsonObj["Request"]["RequestId"].ToString();
            string incomingStr = jsonObj.ToString();
            incomingStr = incomingStr.TrimStart('"').TrimEnd('"').Replace("\\", "").Replace("false", "0").Replace("true", "1");
            XDocument xml = XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(incomingStr), new XmlDictionaryReaderQuotas()));
            string xmlStr = CleanXmlString(xml.ToString());
            string jsonResult = await GetJsonResultAsync(xmlStr);
            JObject obj = JObject.Parse(jsonResult);

            obj.Property("SearchResult").AddAfterSelf(new JProperty("RequestId", reqId));
            return obj.ToString();
        }

        private static string CleanXmlString(string str)
        {           // for FZ module
            str = str.Replace("<SearchRequest type=\"string\">", "");
            str = str.Replace("</SearchRequest>", "");
            str = str.Replace("type=\"object\"", "").Replace("type=\"boolean\"", "").
                                                     Replace("type=\"string\"", "").
                                                     Replace("type=\"null\"", "").
                                                     Replace("type=\"number\"", "").
                                                     Replace(" >", ">").
                                                     Replace("<root>", "").
                                                     Replace("</root>", "");
            return str;
        }

        async Task<string> GetJsonResultAsync(string xmlStr)
        {
            Logger.AddLogStart($"Method: GetJsonResultAsync(string xmlStr)");
            string strRes = await DbAdapter.SearchBlackListDataAsync(xmlStr);

            XmlDocument searchResult = new XmlDocument();
            searchResult.LoadXml(strRes);
            string jsonResult = JsonConvert.SerializeXmlNode(searchResult);
            return jsonResult;
        }
    }
}
