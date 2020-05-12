using Configuration;
using DataProvider.Infrastructure.Extantions;
using MassTransit;
using MessageModelLib;
using NLog;
using NotaryProvider.Services;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NotaryProvider
{
    public class FilePartConsumer : IConsumer<MessageFileModel>
    {
        Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        static string currentDirectory = AppSettings.VolumePath;
        IFileComposeService _fileComposeService;

        public FilePartConsumer(IFileComposeService fileComposeService) // [FromServices] FileComposeService fileComposeService 
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

            Logger.AddLogStart($"RECEIVING Id_part: {context.Message.IdPart} of {context.Message.Quantity} parts from model: {context.Message.IdFile}");

            //await _fileComposeService.AddOrUpdate(partyDocument);
            await new FileComposeService().AddOrUpdate(partyDocument);
        }


        void FolderExist(string currentDirectory)
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

            Logger.AddLogStart(Directory.Exists(currentDirectory) ? "Folder exists." : "! ! !   --> There is no folder and it is not created <--   ! ! !");
        }
    }
}
