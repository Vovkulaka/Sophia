using Configuration;
using DataProvider.Infrastructure.Extantions;
using MassTransit;
using MessageModelLib;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Sophia.FrontApi.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Sophia.FrontApi.Controllers
{
    public class NotarySearchController : Controller
    {
        private ResponseComposeService ResponseComposeService { get; } // Якщо null - пробуй через [FromServices] - https://docs.microsoft.com/ru-ru/aspnet/core/mvc/controllers/dependency-injection?view=aspnetcore-3.1#action-injection-with-fromservices
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        //private readonly ILogger<NotarySearchController> _logger;
        private ISendEndpoint sendEndpoint;
        private ISendEndpointProvider _sendProvider;
        private readonly Uri _serviceAddress;

        public NotarySearchController (ISendEndpointProvider sendProvider, ResponseComposeService responseComposeService)
        {
            Logger.AddLogStart($"START in constructor NotarySearchController");
            ResponseComposeService = responseComposeService;
            _sendProvider = sendProvider;
            _serviceAddress = new Uri($"rabbitmq://{ConfigRabbit.HostAddress}/{ConfigRabbit.RabbitMqQueueName}"); // rabbitmq://localhost/SophiaFrontApi
            //_serviceAddress = new Uri("rabbitmq://sbdp-center-i52.bank.lan:5672/other/SophiaFrontApi");
            Logger.AddLogStart($"QUEUE: Sophia.FrontApi NotarySearchController _serviceAddress: " + _serviceAddress);
        }

        [HttpGet("UpdateDataNotary")]
        public IActionResult UpdateDataNotary()
        {
            return Ok($"Don't work");
        }

        [HttpPost("SeachBlackListDataPack")]
        public IActionResult SeachBlackListDataPack([FromBody] object data) //, NotarySearchController service) - тут кидає помилку!!! Про цю техніку впровадження залежності - https://docs.microsoft.com/ru-ru/aspnet/core/mvc/controllers/dependency-injection?view=aspnetcore-3.1#action-injection-with-fromservices
        {
            Logger.AddLogStart($"Controller SeachBlackListDataPack");
            dynamic responce = null;
            List<JObject> jsonObjs = null;
            try
            {
                JObject json = JObject.Parse(data.ToString());
                jsonObjs = JsonConvert.DeserializeObject<List<JObject>>(json["data"].ToString());

                SendRequestMessage(jsonObjs, "NotaryProvider");
            }
            catch (Exception ex)
            {
                responce = "Fell";
                Logger.AddLogException(ex);
            }

            if (jsonObjs != null)
            {
                responce = SelectResponseMessage(jsonObjs, ResponseComposeService);
            }

            return Ok($"{responce}");
        }

        private async void SendRequestMessage(List<JObject> request, string providerName)
        {
            Logger.AddLogStart($"Method: SendFileMessage(string ConsumerRequest, string providerName)");
            sendEndpoint = await _sendProvider.GetSendEndpoint(_serviceAddress);
            //byte[] request = Encoding.UTF32.GetBytes(ConsumerRequest);
            //byte[] request = ObjectToByteArray(ConsumerRequest);
            //byte[] data = document.ToArray();
            await sendEndpoint.Send(new MessageRequestModel(providerName, GetIdRequest(), request));
        }

        public string GetIdRequest()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 20)
              .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        private List<JObject> SelectResponseMessage(List<JObject> request, ResponseComposeService responseComposeService)
        {
            #region ЗАПУСК ЗАДАЧ В ЦИКЛІ
            //ConcurrentBag<Task<string>> taskCollection/= new ConcurrentBag<Task<string>>();
            ConcurrentBag<Task<IEnumerable<IEnumerable<MessageResponseModel>>>> taskCollection =
                new ConcurrentBag<Task<IEnumerable<IEnumerable<MessageResponseModel>>>>();
            ConcurrentBag<IEnumerable<IEnumerable<MessageResponseModel>>> responceMessage =
                new ConcurrentBag<IEnumerable<IEnumerable<MessageResponseModel>>>();
            for (; taskCollection.Count <= request.Count;)
            //for (int i = 0; i <= request.Count; i++)
            {
                //responseComposeService.AddOrUpdate(MessageResponseModel);
                responceMessage.Add(WaitAnswerAsync(responseComposeService).Result);
                taskCollection.Add(WaitAnswerAsync(responseComposeService));
                //taskCollection.Add(GetAnswer(ResponseComposeService));
            }

            Task.WaitAll(taskCollection.ToArray());
            #endregion

            //List<JObject> response = JsonConvert.DeserializeObject<List<JObject>>(json["data"].ToString());
            //Task<IEnumerable<IEnumerable<MessageResponseModel>>> responceMessage = WaitAnswer(ResponseComposeService) .Result.FirstOrDefault().FirstOrDefault().Data.ToString();
            IEnumerable<IEnumerable<MessageResponseModel>> responceMessage00 = responseComposeService.Read();
            ConcurrentBag<string> cBag = new ConcurrentBag<string>();
            List<JObject> responseData = new List<JObject>();

            foreach (var item in responceMessage.ToList())
            {
                cBag.Add(item.ToString());
            }

            //foreach (var item in responceMessage)
            //{
            //    responseData.Add(JObject.Parse(item.Result));
            //}

            return responseData;
        }

        // ВИДАЛИ async ТА await!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        private async Task<IEnumerable<IEnumerable<MessageResponseModel>>> WaitAnswerAsync(ResponseComposeService ResponseComposeService)
        {
            return ResponseComposeService.Read();
        }
        private IEnumerable<IEnumerable<MessageResponseModel>> GetAnswer(ResponseComposeService ResponseComposeService)
        {
            return ResponseComposeService.Read();
        }
    }
}