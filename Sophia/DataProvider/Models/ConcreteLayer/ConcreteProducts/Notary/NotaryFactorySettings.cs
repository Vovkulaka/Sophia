using DataProvider.Models.AbstractLayer.AbstractProducts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataProvider.Models.ConcreteLayer.ConcreteProducts.Notary
{
    public class NotaryFactorySettings : AbstractFactorySettings
    {
        public override int SourceId { get; set; }

        public override string TableName { get; set; }

        public override string ProcedureName { get; set; }
    }
}
