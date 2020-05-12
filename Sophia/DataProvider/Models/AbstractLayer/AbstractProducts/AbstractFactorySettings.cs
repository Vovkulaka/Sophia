using System;
using System.Collections.Generic;
using System.Text;

namespace DataProvider.Models.AbstractLayer.AbstractProducts
{
    public abstract class AbstractFactorySettings
    {
        public abstract int SourceId { get; set; }
        public abstract string TableName { get; set; }
        public abstract string ProcedureName { get; set; }
    }
}
