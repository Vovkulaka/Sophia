using DataProvider.DataLayer;
using DataProvider.LogicLayer.Services;
using DataProvider.Models.AbstractLayer.AbstractFactory;
using DataProvider.Models.AbstractLayer.AbstractProducts;
using DataProvider.Models.ConcreteLayer.ConcreteProducts.Notary;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataProvider.Models.ConcreteLayer.ConcreteFactories
{
    public class NotaryFactory : AbstractFactory
    {
        //DataConverterService DataConverterService { get; }
        //WebRequestService WebRequestService { get; }
        //XmlServise XmlServise { get; }
        //ZipService ZipService { get; }

        public NotaryFactory()
        {
            //DataConverterService = Startup.ServiceProvider.GetService<DataConverterService>();
            //WebRequestService = Startup.ServiceProvider.GetService<WebRequestService>();
            //XmlServise = Startup.ServiceProvider.GetService<XmlServise>();
            //ZipService = Startup.ServiceProvider.GetService<ZipService>();
        }

        public override AbstractFactorySettings CreateFactorySettings()
        {
            return new NotaryFactorySettings
            {
                SourceId = 2,
                TableName = "[dbo].[OpenDataNotaryRegistry]",
                ProcedureName = "[dbo].[proc_AddOrUpdateOpenDataNotaryRegistry]"
            };
        }

        public override async Task<AbstractUrlList> CreateUrlList(int sourceId)
        {
            AbstractUrlList abstractUrlList = new NotaryUrlList();
            string sourcePackageLink = await DbAdapter.GetSourcePackageLink(sourceId);
            string sourceJsonData = new WebRequestService().GetHttpData(sourcePackageLink);
            abstractUrlList.UrlList = new XmlServise().ParseDataGovUaJson(sourceJsonData);
            return abstractUrlList;
        }

        public override AbstractRawData CreateRawData(AbstractUrlList urls)
        {
            AbstractRawData stream = new NotaryRawData();
            string url = urls.UrlList.Last();
            Stream zipStream = new WebRequestService().GetStreamFromUrl(url);
            stream.DataStream = new ZipService().ExtractStreamFromZipStream(zipStream);
            return stream;
        }

        public override AbstractDataToUpload CreateDataToUpload(AbstractRawData stream)
        {
            AbstractDataToUpload abstractData = new NotaryDataToUpload();
            string xmlData = new DataConverterService().StreamToStringWin1251(stream.DataStream);
            if (xmlData.Contains("<?xml version=\"1.0\" encoding=\"windows-1251\"?>"))
            {
                xmlData = xmlData.Replace("<?xml version=\"1.0\" encoding=\"windows-1251\"?>", "");
            }
            abstractData.XmlDataToUpload = xmlData;
            return abstractData;
        }

        public override async Task UploadDataAsync(string tableName, string procName, AbstractDataToUpload dataToUpload)
        {
            await DataUploader.UploadXmlDataAsync(procName, dataToUpload.XmlDataToUpload);
        }

        public override async Task<int> GetRecordCount(string tableName)
        {
            return await DbAdapter.GetTableRowsCount(tableName);
        }
    }
}
