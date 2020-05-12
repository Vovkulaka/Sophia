using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageModelLib
{
    interface IMessageRequestModel 
    {
        string ProviderName { get; }
        string IdRequest { get; }
        List<JObject> Data { get; }
    }
}
