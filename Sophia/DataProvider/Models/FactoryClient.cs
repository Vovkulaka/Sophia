using DataProvider.Models.AbstractLayer.AbstractFactory;
using DataProvider.Models.AbstractLayer.AbstractProducts;

namespace DataProvider.Models
{
    public class FactoryClient
    {
        private AbstractFactory _factory;
        private AbstractFactorySettings _settings;
        public AbstractUrlList _urlList;
        private AbstractRawData _stream;
        public AbstractDataToUpload _dataToUpload;

        public FactoryClient(AbstractFactory factory)
        {
            _factory = factory;
            _settings = factory.CreateFactorySettings();
            _urlList = factory.CreateUrlList(_settings.SourceId).Result;
            _stream = factory.CreateRawData(_urlList);
            _dataToUpload = factory.CreateDataToUpload(_stream);
        }

        //public void UploadDataToDb() // ПЕРЕДАЧА В БД
        //{
        //    _factory.UploadData(_settings.TableName, _settings.ProcedureName, _dataToUpload);
        //    //int uploadrowcount = _factory.GetRecordCount(_settings.TableName);
        //    //return uploadrowcount;
        //}
    }
}
