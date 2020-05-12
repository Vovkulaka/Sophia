using DataProvider.Infrastructure.Extantions;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Configuration;

namespace Sophia.Uploader
{
    public class SenderProducerHostedService : IHostedService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        private ISendEndpointProvider _sendProvider; //Замість IPublishEndpoint було ISendEndpointProvider
        private Uri _serviceAddress;

        // Замість IPublishEndpoint publishEndpoint було ISendEndpointProvider sendProvider
        public SenderProducerHostedService(ISendEndpointProvider sendProvider, ILoggerFactory loggerFactory)
        {
            _sendProvider = sendProvider;
            _serviceAddress = new Uri($"rabbitmq://{ConfigRabbit.HostAddress}/{ConfigRabbit.RabbitMqQueueName}"); // rabbitmq://localhost/SophiaUploade
            //_serviceAddress = new Uri("rabbitmq://sbdp-center-i52.bank.lan:5672/other/SophiaUploader");
            Logger.AddLogStart($"Uploader SenderProducerHostedService _serviceAddress: " + _serviceAddress);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.AddLogStart($"START Uploader.SenderProducer bus...");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.AddLogStart($"STOP Uploader.SenderProducer bus...");
            return Task.CompletedTask;
        }
    }
}
