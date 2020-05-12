using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataProvider.Models.AbstractLayer.AbstractProducts
{
    public abstract class AbstractDataToUpload
    {
        public abstract string XmlDataToUpload { get; set; }
        public abstract string JsonDataToUpload { get; set; }
        public abstract DataTable CsvDataToUpload { get; set; }
        public abstract string PathToFile { get; set; }
    }
}
