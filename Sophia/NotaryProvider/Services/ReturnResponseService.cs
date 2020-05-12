using DataProvider.Infrastructure.Extantions;
using MassTransit;
using MessageModelLib;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Configuration;

namespace NotaryProvider.Services
{
    public interface IReturnResponseService
    {
        public Task<List<JObject>> SendResponseInQueue(List<JObject> ConsumerResponse, string ProviderName, string IdRequest);
    }

    public class ReturnResponseService : IReturnResponseService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        private ISendEndpoint sendEndpoint;
        private ISendEndpointProvider _sendProvider;
        private readonly Uri _serviceAddress;
        private readonly IConfiguration _configuration; // ПОПРОБУЙ ВИДАЛИТИ

        public ReturnResponseService(ISendEndpointProvider sendProvider, IConfiguration configuration)  
        {
            Logger.AddLogStart($"START in constructor NotaryUploaderController");
            _sendProvider = sendProvider;
            _configuration = configuration; // ПОПРОБУЙ ВИДАЛИТИ
            _serviceAddress = new Uri($"rabbitmq://{ConfigRabbit.HostAddress}/Responce"); // rabbitmq://localhost/Responce - це ім'я черги
            //_serviceAddress = new Uri("rabbitmq://sbdp-center-i52.bank.lan:5672/other/Responce");
            Logger.AddLogStart($"QUEUE: NotaryProvider ReturnResponseService _serviceAddress: " + _serviceAddress);
        }

        public async Task<List<JObject>> SendResponseInQueue(List<JObject> consumerResponse, string providerName, string IdRequest)
        {
            sendEndpoint = await _sendProvider.GetSendEndpoint(_serviceAddress);
            //string providerName = "NotaryProvider";
            await sendEndpoint.Send(new MessageResponseModel(providerName, IdRequest, consumerResponse));
            //return Task.CompletedTask;
            return await Task.FromResult<List<JObject>>(consumerResponse);
            //https://docs.microsoft.com/ru-ru/dotnet/standard/parallel-programming/how-to-create-pre-computed-tasks
            //List<JObject> responce;
            //return Task.Run(async () =>
            //{
            //    responce = await sendEndpoint.Send(new MessageResponseModel(providerName, ConsumerResponse));
            //});
        }
    }
}
