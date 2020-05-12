using DataProvider.Models.AbstractLayer.AbstractProducts;
using System.Threading.Tasks;

namespace DataProvider.Models.AbstractLayer.AbstractFactory
{
    public abstract class AbstractFactory
    {
        public abstract AbstractFactorySettings CreateFactorySettings();
        public abstract Task<AbstractUrlList> CreateUrlList(int sourceId);
        public abstract AbstractRawData CreateRawData(AbstractUrlList urlList);
        public abstract AbstractDataToUpload CreateDataToUpload(AbstractRawData stream);

        public abstract Task UploadDataAsync(string tableName, string procName, AbstractDataToUpload dataToUpload);
        public abstract Task<int> GetRecordCount(string tableName);
    }
}
