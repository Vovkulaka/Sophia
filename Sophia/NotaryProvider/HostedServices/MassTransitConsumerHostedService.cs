using DataProvider.Infrastructure.Extantions;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NotaryProvider.HostedServices;
using System.Threading;
using System.Threading.Tasks;

namespace NotaryProvider
{
    public class MassTransitConsumerHostedService : IHostedService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        public IBusControl BusControl { get; set; } 

        public MassTransitConsumerHostedService(IBusControl bus) 
        {
            BusControl = bus;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.AddLogStart($"START MassTransit_Consumer bus...");
            await BusControl.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.AddLogStart($"STOP MassTransit_Consumer bus...");
            await BusControl.StopAsync(cancellationToken).ConfigureAwait(false);
        }

        #region Пробував переробити під відправку повідомлень в чергу Responce, та згадав що даний сервіс використовується для завантаження даних з державного ресурсу.
        //private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        //// Це реалізація відправки повідомлення:
        //private ISendEndpointProvider _sendProvider; // це з Sophia.Uploader.SenderProducerHostedService()

        //public MassTransitConsumerHostedService(ISendEndpointProvider sendProvider)
        //{
        //    _sendProvider = sendProvider; // це з Sophia.Uploader.SenderProducerHostedService()
        //}

        ///// <inheritdoc />
        ////public async Task StartAsync(CancellationToken cancellationToken)
        ////{
        ////    Logger.AddLogStart($"START MassTransit_Consumer bus...");
        ////    await BusControl.StartAsync(cancellationToken).ConfigureAwait(false);
        ////}
        //public Task StartAsync(CancellationToken cancellationToken)
        //{
        //    Logger.AddLogStart($"START NotaryProvider.SenderProducer bus...");
        //    return Task.CompletedTask;
        //}

        ///// <inheritdoc />
        ////public async Task StopAsync(CancellationToken cancellationToken)
        ////{
        ////    Logger.AddLogStart($"STOP MassTransit_Consumer bus...");
        ////    await BusControl.StopAsync(cancellationToken).ConfigureAwait(false);
        ////}
        //public Task StopAsync(CancellationToken cancellationToken)
        //{
        //    Logger.AddLogStart($"STOP NotaryProvider.SenderProducer bus...");
        //    return Task.CompletedTask;
        //}
        #endregion
    }
}
