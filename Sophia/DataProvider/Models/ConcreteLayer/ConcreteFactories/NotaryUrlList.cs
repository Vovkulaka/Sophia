using DataProvider.Models.AbstractLayer.AbstractProducts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataProvider.Models.ConcreteLayer.ConcreteFactories
{
    public class NotaryUrlList : AbstractUrlList
    {
        public override List<string> UrlList { get; set; }
    }
}
