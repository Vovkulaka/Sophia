using DataProvider.Infrastructure.Extantions;
using MassTransit;
using MessageModelLib;
using NLog;
using NotaryProvider.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotaryProvider
{
    public class RequestConsumer : IConsumer<MessageRequestModel>
    {
        Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        IGetDataService _getDataService;

        public RequestConsumer(IGetDataService getDataService)
        {
            _getDataService = getDataService;
        }
        public RequestConsumer() { }
        public async Task Consume(ConsumeContext<MessageRequestModel> context)
        {
            Logger.AddLogStart($"RECEIVING message from provider: {context.Message.ProviderName} with ID: {context.Message.IdRequest}");

            MessageRequestModel partyDocument = new MessageRequestModel(
                context.Message.ProviderName,
                context.Message.IdRequest,
                context.Message.Data);

            await _getDataService.GetData(partyDocument);
            //await new GetDataService().GetData(partyDocument);
        }
    }
}
