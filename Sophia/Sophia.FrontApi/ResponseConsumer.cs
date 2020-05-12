using DataProvider.Infrastructure.Extantions;
using MassTransit;
using MessageModelLib;
using NLog;
using Sophia.FrontApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sophia.FrontApi
{
    public class ResponseConsumer : IConsumer<MessageResponseModel>
    {
        static Logger LoggerStatic { get; } = LogManager.GetCurrentClassLogger();
        IResponseComposeService responseComposeService;

        public ResponseConsumer (IResponseComposeService _responseComposeService)
        {
            responseComposeService = _responseComposeService;
        }
        public ResponseConsumer () { }

        public async Task Consume(ConsumeContext<MessageResponseModel> context)
        {
            LoggerStatic.AddLogStart($"RECEIVING ProviderName: {context.Message.ProviderName}");

            if (context.Message.ProviderName == "SophiaFrontApi")
            {
                MessageResponseModel partyDocument = new MessageResponseModel(
                    context.Message.ProviderName,
                    context.Message.IdRequest,
                    context.Message.Data);

                await responseComposeService.AddOrUpdate(partyDocument);
            }
        }
    }
}