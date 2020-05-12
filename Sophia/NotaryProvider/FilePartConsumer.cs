using Configuration;
using DataProvider.Infrastructure.Extantions;
using MassTransit;
using MessageModelLib;
using NLog;
using NotaryProvider.Services;
using System.IO;
using System.Threading.Tasks;

namespace NotaryProvider
{
    public class FilePartConsumer : IConsumer<MessageFileModel>
    {
        Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        static Logger LoggerStatic { get; } = LogManager.GetCurrentClassLogger();
        static string currentDirectory = AppSettings.VolumePath;
        IFileComposeService _fileComposeService;

        public FilePartConsumer(IFileComposeService fileComposeService)
        {
            _fileComposeService = fileComposeService;
            FolderExist(currentDirectory);
        }

        public FilePartConsumer() { }

        public async Task Consume(ConsumeContext<MessageFileModel> context)
        {            
            MessageFileModel partyDocument = new MessageFileModel(
                context.Message.IdFile,
                context.Message.IdPart,
                context.Message.Quantity,
                context.Message.Data,
                context.Message.FileName,
                context.Message.SourceId,
                context.Message.LengthFile,
                context.Message.LengthPart,
                context.Message.LengthLastPart);

            LoggerStatic.AddLogStart($"RECEIVING Id_part: {context.Message.IdPart} of {context.Message.Quantity} parts from model: {context.Message.IdFile}");

            //_fileComposeService.AddOrUpdate(partyDocument);
            await new FileComposeService().AddOrUpdate(partyDocument);
        }


        static void FolderExist(string currentDirectory)
        {
            if (!Directory.Exists(currentDirectory))
            {
                int counter = 0;

                while (!Directory.Exists(currentDirectory) && counter < 10)
                {
                    counter++;
                    Directory.CreateDirectory(currentDirectory);
                }
            }

            LoggerStatic.AddLogStart(Directory.Exists(currentDirectory) ? "Folder exists." : "! ! !   --> There is no folder and it is not created <--   ! ! !");
        }
    }
}
