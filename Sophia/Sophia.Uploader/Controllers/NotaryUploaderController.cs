using Configuration;
using DataProvider;
using DataProvider.Infrastructure.Extantions;
using MassTransit;
using MessageModelLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sophia.Uploader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotaryUploaderController : ControllerBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        private ISendEndpoint sendEndpoint;
        private ISendEndpointProvider _sendProvider;
        private readonly Uri _serviceAddress;
        private readonly IConfiguration _configuration; // ПОПРОБУЙ ВИДАЛИТИ    

        public NotaryUploaderController(ILogger<NotaryUploaderController> logger, ISendEndpointProvider sendProvider, IConfiguration configuration)
        {
            Logger.AddLogStart($"START in constructor NotaryUploaderController");
            _sendProvider = sendProvider;
            _configuration = configuration; // ПОПРОБУЙ ВИДАЛИТИ
            _serviceAddress = new Uri($"rabbitmq://{ConfigRabbit.HostAddress}/{ConfigRabbit.RabbitMqQueueName}"); // rabbitmq://localhost/SophiaUploader 
            //_serviceAddress = new Uri("rabbitmq://sbdp-center-i52.bank.lan:5672/other/SophiaUploader");
            Logger.AddLogStart($"QUEUE: Sophia.Uploader NotaryUploaderController _serviceAddress: " + _serviceAddress);
        }

        [HttpPost("UpdateOpenDataSources")]
        public async Task<IActionResult> UpdateOpenDataSources()
        {
            int item = 2;
            string responce = "";
            string fileSource = @"D:\DEVELOPMENT\Sophia\app\Source files\Words from New World Tanslation.csv";
            //string fileSource = @"https://data.gov.ua/dataset/995361b0-47a6-4b19-8f0e-35216e64e416/resource/db00f7cc-da63-4e2a-b9dc-b440a6c1932f/download/26_07_19-tafu.ods";
            //string fileSource = @"https://data.gov.ua/dataset/de142932-b782-4925-87eb-b8934a431df3/resource/ceaf1653-f70b-4f53-8de8-533916f77a40/download/zp_25022020.csv";  

            try
            {
                #region ВОСТОК
                //var factoryClient = await new DataProviderMain().DownloadSource(item);
                //Logger.AddLogStart($"factoryClient: " + factoryClient.ToString());
                //SendFileMessage(factoryClient, item);
                #endregion
                
                // З Б Е Р Е Ж Е Н Н Я   П Р А Ц Ю Є   П Р И   К О Ж Н О М У   З А П У С К У ! ! ! 
                int vostok = 1;

                // З Е Б Е Р Е Ж Е Н Н Я   П Р А Ц Ю Є   Л И Ш Е   Р А З !!!
                // П Р И   П О В Т О Р Н І Й   С П Р О Б І   П Е Р Е С Т А Р Є   П Р А Ц Ю В А Т И   М Е Т О Д   FileComposeService.Read() !!!
                // Д Л Я   В І Д Н О В Л Е Н Н Я   П Р А Ц Е З Д А Т Н О С Т І   П О Т Р І Б Н О   З А П У С Т И Т И   SendFileMessage(vostok); 
                int home = 2;

                SendFileMessage(vostok);
                //SendFileMessage(fileSource, item);
                responce = "Ok";

                Logger.AddLogStart("UpdateOpenDataSources responce = Ok");
            }
            catch (Exception ex)
            {
                responce = "Fell";
                Logger.AddLogException(ex);
            }

            return Ok($"{responce}");
        }

        //private async void SendFileMessage(IList<string> factoryClient, int item)  // ВОСТОК
        //private async void SendFileMessage(string fileSource, int item)
        private async void SendFileMessage(int whoWillWork, IList<string> factoryClient = null, int item = 2)  
        {
            sendEndpoint = await _sendProvider.GetSendEndpoint(_serviceAddress);
            dynamic document = null;
            string currentDirectory = string.Empty;
            string fileName = string.Empty;
            #region
            //await _bus.Publish<MyEvent>(...);

            //var sendEndpoint = await _bus.GetSendEndpoint(new Uri(ConfigurationManager.AppSettings["MyCommandQueueFullUri"]));
            //await sendEndpoint.Send<MyCommand>(...);
            #endregion

            if (whoWillWork == 1)// ВОСТОК
            {
                string fileSource = @"https://data.gov.ua/dataset/995361b0-47a6-4b19-8f0e-35216e64e416/resource/db00f7cc-da63-4e2a-b9dc-b440a6c1932f/download/26_07_19-tafu.ods";
                //string fileSource = factoryClient[0];
                Logger.AddLogStart($"Length fileSource: " + fileSource.Length);
                document = Encoding.UTF32.GetBytes(fileSource);
                //string fileName = SelectFileNameFromURL(factoryClient[1]); 
                fileName = SelectFileNameFromURL(fileSource); 
            }
            else if (whoWillWork == 2)
            {
                string fileSource = @"D:\DEVELOPMENT\Sophia\app\Source files\Words from New World Tanslation.csv";
                document = System.IO.File.ReadAllBytes(fileSource);
                fileName = SelectFileNameFromLocalPath(fileSource);
            }

            Logger.AddLogStart($"fileName: " + fileName);
            currentDirectory = AppSettings.VolumePath; // ПАПКА "VolumePath": "\\app\\files\\Sophia.Uploader",
            Logger.AddLogStart($"currentDirectory: " + currentDirectory);

            FolderExist(currentDirectory);

            string path = Path.Combine(currentDirectory, fileName); // DEVELOPMENT
            Logger.AddLogStart($"path: " + path);
            System.IO.File.WriteAllBytes(path, document); // ЗБЕРЕЖЕННЯ 
            Logger.AddLogStart($"ЗБЕРЕЖЕННЯ ФАЙЛУ: " + fileName);
            Logger.AddLogStart(System.IO.File.Exists(path) ? "File save." : "! ! !   --> File does not save <--   ! ! !");
            string idFile = CalculateMD5(path);
            Logger.AddLogStart($"id_model: " + idFile);

            int sizePart = 25000; // 1 024 | 1 048 576 | 1 073 741 824 | 1 099 511 627 776 |

            int numberByteEnd = 0;
            int idPart = 0;
            int quantity = document.Length / sizePart;
            int lastPaty = document.Length - sizePart * quantity;

            for (int i = 0; i <= quantity; i++)
            {
                if (idPart == quantity && lastPaty < sizePart) sizePart = lastPaty;
                ArraySegment<byte> localArray = new ArraySegment<byte>(document, numberByteEnd, sizePart);
                byte[] data = localArray.ToArray();
                await sendEndpoint.Send(new MessageFileModel(idFile, idPart, quantity, data, fileName, item, document.Length, sizePart, lastPaty));
                Logger.AddLogStart($"SENDING id_part: " + idPart);
                idPart += 1;
                numberByteEnd += sizePart;
            }
        }

        static string CalculateMD5(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(fileName))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        static string SelectFileNameFromLocalPath(string fileSource)
        {
            string fileName = "";
            string[] splitString = Regex.Split(fileSource, @"\\");

            foreach (var item in splitString)
            {
                fileName = item;
            }

            return fileName;
        }

        static string SelectFileNameFromURL(string fileSource)
        {
            string fileName = "";
            string[] splitString = Regex.Split(fileSource, @"/");

            foreach (var item in splitString)
            {
                fileName = item;
            }

            return fileName;
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

            Logger.AddLogStart(Directory.Exists(currentDirectory) ? "Folder Sophia.Uploader exists." : "! ! !   --> There is no folder and it is not created <--   ! ! !");
        }
    }
}