using DataProvider.Infrastructure.Extantions;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DataProvider.LogicLayer.Services
{
    public class WebRequestService
    {
        private Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        #region Old code
        //internal string Post(string url, string data, string conntentType = "application/json")
        //{
        //    string response = null;
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    try
        //    {
        //        byte[] byteArray = Encoding.UTF8.GetBytes(data);
        //        request.Method = "POST";
        //        request.ContentType = conntentType;
        //        request.KeepAlive = false;
        //        request.Proxy = null;
        //        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        //        request.Timeout = 100000002;

        //        using (Stream requestStream = request.GetRequestStream())
        //        {
        //            requestStream.Write(byteArray, 0, byteArray.Length);
        //        }
        //        using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
        //        {
        //            using (Stream responseStream = webResponse.GetResponseStream())
        //            {
        //                if (responseStream != null)
        //                {
        //                    using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
        //                    {
        //                        response = reader.ReadToEnd();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        request.Abort();
        //    }
        //    return response;
        //}
        #endregion

        public string GetHttpData(string url)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(url);

                Logger.AddLogStart("HttpWebRequest.Create(url)");
            }
            catch (Exception ex)
            {
                Logger.AddLogException(ex);
                throw ex;
            }

            #region Old
            try
            {
                Logger.AddLogStart($"request: " + request.ToString() + "-->");
                Logger.AddLogStart($"request.Address: " + request.Address.ToString());
                Logger.AddLogStart($"request.RequestUri: " + request.RequestUri.ToString());
                //request.Timeout = 3000000;

                response = (HttpWebResponse)request.GetResponse();
                Logger.AddLogStart($"request.GetResponse(): " + response.ToString() + "  -->");
                Logger.AddLogStart($"response.CharacterSet: " + response.CharacterSet.ToString());
                Logger.AddLogStart($"response.Headers: " + response.Headers.ToString());
                Logger.AddLogStart($"response.LastModified: " + response.LastModified.ToString());
                Logger.AddLogStart($"response.Method: " + response.Method.ToString());
                Logger.AddLogStart($"response.ResponseUri: " + response.ResponseUri.ToString());
                Logger.AddLogStart($"response.Server: " + response.Server.ToString());
                Logger.AddLogStart($"response.StatusCode: " + response.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                Logger.AddLogException(ex);
                throw ex;
            }
            #endregion            

            string requestedData = string.Empty;

            using (StreamReader stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                requestedData = stream.ReadToEnd();
            }
            return requestedData;
        }

        public Stream GetStreamFromUrl(string url)
        {
            byte[] data = null;

            using (var client = new WebClient())
            {
                data = client.DownloadData(url);
            }
            return new MemoryStream(data);
        }
    }
}
