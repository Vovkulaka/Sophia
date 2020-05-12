using Configuration;
using DataProvider.Infrastructure.Extantions;
using MessageModelLib;
using Microsoft.Extensions.Hosting;
using NLog;
using NotaryProvider.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace NotaryProvider.HostedServices
{
    public class FileBuilderHostedService : IHostedService//<T> : IHostedService where T : IHostedService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        private FileComposeService _fileComposeService { get; }
        private SavingToDBService SavingToDBService { get; }
        //readonly T backgroundService;
        static string currentDirectory = AppSettings.VolumePath;
        static string checksumFile;

        public FileBuilderHostedService(FileComposeService fileComposeService, SavingToDBService savingToDB) // (T _backgroundService, 
        {
            //backgroundService = _backgroundService;
            _fileComposeService = fileComposeService;
            SavingToDBService = savingToDB;
        }

        public Task CreatingFile(FileBuilderHostedService service)
        {
            FolderExist(currentDirectory);

            while (true)
            {
                var concurrentBagModel = service._fileComposeService.Read();
                foreach (IEnumerable<MessageFileModel> parts in concurrentBagModel)
                {
                    // Здесь ты получишь по очерёдно все части всех файлов, что запишет тебе consumer.
                    // Далее сравниваешь количество частей которые есть в parts и которые должны там быть (DocumentModel.IdPart),
                    //  если все части есть - формируешь byte[] из частей и пишешь в БД вызывая ОТДЕЛЬНЫЙ сервис. ConcurrentDictionary<int, ConcurrentBag<MessageModel>>

                    var part = parts;
                    string path = Path.Combine(currentDirectory, parts.FirstOrDefault().FileName);
                    IEnumerable<Byte> compiledFile = parts.OrderBy(d => d.IdPart).SelectMany(d => d.Data);

                    File.WriteAllBytes(path, compiledFile.ToArray());
                    Logger.AddLogStart(File.Exists(path) ? "File exists." : "! ! !   --> File does not exist <--   ! ! !");
                    checksumFile = CalculateMD5(path);

                    if (parts.FirstOrDefault().IdFile == checksumFile)
                    {
                        Logger.AddLogStart("SAVED FILE MATCHES SOURCE FILE!!!");
                        service._fileComposeService.DeleteCBModel(parts.FirstOrDefault().IdFile);
                    }
                    else
                    {
                        Logger.AddLogStart("SAVED FILE DOES NOT MATCH SOURCE FILE!!!");
                    }

                    service.SavingToDBService.UpdateSourceInDB(compiledFile.ToArray(), parts.FirstOrDefault().SourceId);
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.AddLogStart($"START MassTransit_Consumer bus...");
            Logger.AddLogStart($"Starting separate thread");

            Thread thr = new Thread(data => {CreatingFile(this);}); // Зроби через Task
            thr.Start();
            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.AddLogStart($"STOP MassTransit_Consumer bus...");
            return Task.CompletedTask; //return backgroundService.StopAsync(cancellationToken);
        }

        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
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

            Logger.AddLogStart(Directory.Exists(currentDirectory) ? "Folder NotaryProvider exists." : "! ! !   --> There is no folder and it is not created <--   ! ! !");
        }
    }
}
