using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageModelLib
{
    interface IMessageResponseModel
    {
        string ProviderName { get; }
        string IdRequest { get; }
        List<JObject> Data { get; }
    }
}
