using DataProvider.Infrastructure.Extantions;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Configuration;

namespace NotaryProvider.HostedServices
{
    public class SenderProducerHostedService : IHostedService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        private ISendEndpointProvider _sendProvider; //Замість IPublishEndpoint було ISendEndpointProvider
        private Uri _serviceAddress;

        public SenderProducerHostedService(ISendEndpointProvider sendProvider, ILoggerFactory loggerFactory)
        {
            _sendProvider = sendProvider;
            _serviceAddress = new Uri($"rabbitmq://{ConfigRabbit.HostAddress}/Responce"); // rabbitmq://localhost/Responce - це ім'я черги
            //_serviceAddress = new Uri("rabbitmq://sbdp-center-i52.bank.lan:5672/other/Responce");
            Logger.AddLogStart($"Uploader SenderProducerHostedService _serviceAddress: " + _serviceAddress);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.AddLogStart($"START NotaryProvider.SenderProducer bus...");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.AddLogStart($"STOP NotaryProvider.SenderProducer bus...");
            return Task.CompletedTask;
        }
    }
}
