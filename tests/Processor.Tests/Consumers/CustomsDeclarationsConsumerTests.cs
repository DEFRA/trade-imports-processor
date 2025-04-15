using System.Text.Json;
using AutoFixture;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using static Defra.TradeImportsProcessor.TestFixtures.ClearanceRequestFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class CustomsDeclarationsConsumerTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ITradeImportsDataApiClient _mockApi = Substitute.For<ITradeImportsDataApiClient>();

    private readonly ILogger<CustomsDeclarationsConsumer> _mockLogger = Substitute.For<
        ILogger<CustomsDeclarationsConsumer>
    >();

    [Fact]
    public async Task OnHandle_WhenClearanceRequestReceived_AndNoClearanceRequestExistsInTheDataApi_ThenItIsCreated()
    {
        var consumer = new CustomsDeclarationsConsumer(_mockLogger, _mockApi);

        var mrn = GenerateMrn();
        var clearanceRequest = ClearanceRequestFixture(mrn).Create();

        _mockApi.GetCustomsDeclaration(mrn, _cancellationToken).Returns(null as CustomsDeclarationResponse);

        await consumer.OnHandle(JsonSerializer.SerializeToElement(clearanceRequest), _cancellationToken);

        await _mockApi
            .Received()
            .PutCustomsDeclaration(
                mrn,
                Arg.Any<DataApiCustomsDeclaration.CustomsDeclaration>(),
                null,
                _cancellationToken
            );
    }

    [Fact]
    public async Task OnHandle_WhenClearanceRequestReceived_AndAClearanceRequestAlreadyExists_ThenItIsUpdated()
    {
        var consumer = new CustomsDeclarationsConsumer(_mockLogger, _mockApi);

        var mrn = GenerateMrn();
        var clearanceRequest = ClearanceRequestFixture(mrn).Create();
        var existingClearanceRequest = DataApiClearanceRequestFixture().Create();
        const string expectedEtag = "12345";

        var response = new CustomsDeclarationResponse(
            mrn,
            existingClearanceRequest,
            null,
            null,
            DateTime.Now,
            DateTime.Now,
            expectedEtag
        );

        _mockApi.GetCustomsDeclaration(mrn, _cancellationToken).Returns(response);

        await consumer.OnHandle(JsonSerializer.SerializeToElement(clearanceRequest), _cancellationToken);

        await _mockApi
            .Received()
            .PutCustomsDeclaration(
                mrn,
                Arg.Any<DataApiCustomsDeclaration.CustomsDeclaration>(),
                expectedEtag,
                _cancellationToken
            );
    }
}
