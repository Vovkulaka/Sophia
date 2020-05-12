using System;
using System.Collections.Generic;
using System.Text;

namespace DataProvider.LogicLayer.Services
{
    public class XmlServise
    {
        public List<string> ParseDataGovUaJson(string sourceJsonData)
        {
            List<string> urlList = new List<string>
            {
                "https://data.gov.ua/dataset/85a68e3e-8cb0-41b8-a764-58d005063b52/resource/65e9ad78-0e65-4672-ba42-f7613e0fa493/download/18-ex_xml_wern.zip"
            };

            //string rootWrap = "{\"root\": " + sourceJsonData + "}";

            // XmlDocument xmlDoc = JsonConvert.DeserializeXmlNode(rootWrap);
            ////var xmlDoc = XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(
            ////    Encoding.ASCII.GetBytes(rootWrap), new XmlDictionaryReaderQuotas()));
            ////XmlDocument xmlDoc;

            //XmlElement root = xmlDoc.DocumentElement;
            //XmlNodeList resourceList = root.GetElementsByTagName("resources");
            //foreach (XmlNode resource in resourceList)
            //{
            //    XmlNodeList pathList = (resource as XmlElement).GetElementsByTagName("path");
            //    foreach (XmlNode item in pathList)
            //    {
            //        urlList.Add(item.InnerText);
            //    }
            //} 
            return urlList;
        }
    }
}
