using System.Text.Json;
using AutoFixture;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Models.Gmrs;
using Defra.TradeImportsProcessor.Processor.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SlimMessageBus.Host;
using static Defra.TradeImportsProcessor.TestFixtures.GmrFixtures;
using DataApiGvms = Defra.TradeImportsDataApi.Domain.Gvms;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class AsbGmrConsumerTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ILogger<AsbGmrsConsumer> _mockLogger = Substitute.For<ILogger<AsbGmrsConsumer>>();
    private readonly IGmrProcessingService _mockService = Substitute.For<IGmrProcessingService>();

    private static ConsumerContext GetConsumerContext()
    {
        return new ConsumerContext
        {
            Properties =
            {
                new KeyValuePair<string, object>(
                    "ServiceBus_Message",
                    ServiceBusModelFactory.ServiceBusReceivedMessage()
                ),
            },
        };
    }

    [Fact]
    public async Task OnHandle_WhenValidGmrIsReceived_CallsProcessingService()
    {
        var gmr = GmrFixture().Create();
        var consumer = new AsbGmrsConsumer(_mockLogger, _mockService) { Context = GetConsumerContext() };

        await consumer.OnHandle(JsonSerializer.SerializeToElement(gmr), _cancellationToken);

        await _mockService.Received(1).ProcessGmr(Arg.Is<DataApiGvms.Gmr>(g => g.Id == gmr.GmrId), _cancellationToken);
    }

    [Fact]
    public async Task OnHandle_WhenGmrIsNull_ThrowsGmrMessageException()
    {
        var consumer = new AsbGmrsConsumer(_mockLogger, _mockService) { Context = GetConsumerContext() };
        var nullJson = JsonSerializer.SerializeToElement<Gmr?>(null);

        var act = async () => await consumer.OnHandle(nullJson, _cancellationToken);
        await act.Should().ThrowAsync<Exceptions.GmrMessageException>();

        await _mockService.DidNotReceiveWithAnyArgs().ProcessGmr(null!, CancellationToken.None);
    }
}
