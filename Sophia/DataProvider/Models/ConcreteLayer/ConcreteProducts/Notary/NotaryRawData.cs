using DataProvider.Models.AbstractLayer.AbstractProducts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataProvider.Models.ConcreteLayer.ConcreteProducts.Notary
{
    public class NotaryRawData : AbstractRawData
    {
        public override Stream DataStream { get; set; }
        public override string PathToFile { get; set; }
    }
}
