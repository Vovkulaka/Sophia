using DataLyingInDB;
using DataProvider.Infrastructure.Extantions;
using NLog;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace NotaryProvider.Services
{
    public class SavingToDBService
    {
        private Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public void UpdateSourceInDB (byte[] compiledFile, int SourceId)
        {
            try
            {
                string procedureName = "[dbo].[proc_AddOrUpdateOpenDataNotaryRegistry]";
                string strCompiledFile = Encoding.UTF32.GetString(compiledFile.ToArray());

                Thread.Sleep(1000);
                int resoultUpdate = new LieInDB().UpdateSource(SourceId, procedureName, strCompiledFile);

                if (resoultUpdate == 1) Logger.AddLogStart("DOWNLOAD IN DATABASE SUCCESSFULLY!!!");
                else Logger.AddLogStart("DOWNLOAD IN DATABASE FALSE!!!");
            }
            catch (Exception ex)
            {
                Logger.AddLogException(ex);
            }
        }
    }
}
