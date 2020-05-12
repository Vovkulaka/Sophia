using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataProvider.Models.AbstractLayer.AbstractProducts
{
    public abstract class AbstractRawData
    {
        public abstract Stream DataStream { get; set; }
        public abstract string PathToFile { get; set; }
    }
}
