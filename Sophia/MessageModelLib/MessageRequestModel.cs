using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageModelLib
{
    public class MessageRequestModel : IMessageRequestModel
    {
        public MessageRequestModel(string providerName, string idRequest, List<JObject> data)
        {
            ProviderName = providerName;
            IdRequest = idRequest;
            Data = data;
        }

        public string ProviderName { get; }
        public string IdRequest { get; }
        public List<JObject> Data { get; }
    }
}
