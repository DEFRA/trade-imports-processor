using System.Text.Json;
using Amazon.SQS.Model;
using AutoFixture;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SlimMessageBus.Host;
using static Defra.TradeImportsProcessor.TestFixtures.MatchedGmrFixtures;
using DataApiGvms = Defra.TradeImportsDataApi.Domain.Gvms;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class MatchedGmrConsumerTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ILogger<MatchedGmrConsumer> _mockLogger = Substitute.For<ILogger<MatchedGmrConsumer>>();
    private readonly IGmrProcessingService _mockService = Substitute.For<IGmrProcessingService>();

    private static ConsumerContext GetConsumerContext()
    {
        return new ConsumerContext
        {
            Properties =
            {
                new KeyValuePair<string, object>("SQS_Message", new Message { MessageId = "test-message-id" }),
            },
        };
    }

    [Fact]
    public async Task OnHandle_WhenValidMatchedGmrIsReceived_CallsProcessingService()
    {
        var matchedGmr = MatchedGmrFixture().Create();
        var consumer = new MatchedGmrConsumer(_mockLogger, _mockService) { Context = GetConsumerContext() };

        await consumer.OnHandle(matchedGmr, _cancellationToken);

        await _mockService
            .Received(1)
            .ProcessGmr(Arg.Is<DataApiGvms.Gmr>(g => g.Id == matchedGmr.Gmr.GmrId), _cancellationToken);
    }
}
