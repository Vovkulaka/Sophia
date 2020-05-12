
using DataProvider.Models.AbstractLayer.AbstractProducts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataProvider.Models.ConcreteLayer.ConcreteProducts.Notary
{
    public class NotaryDataToUpload : AbstractDataToUpload
    {
        public override string XmlDataToUpload { get; set; }
        public override string JsonDataToUpload { get; set; }
        public override DataTable CsvDataToUpload { get; set; }
        public override string PathToFile { get; set; }
    }
}
