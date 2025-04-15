using System.Text.Json;
using AutoFixture;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using static Defra.TradeImportsProcessor.TestFixtures.ClearanceDecisionFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.ClearanceRequestFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.FinalisationFixtures;
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
    public async Task OnHandle_WhenClearanceRequestReceived_AndNoCustomsDeclarationRecordExistsInTheDataApi_ThenItIsCreated()
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
    public async Task OnHandle_WhenClearanceRequestReceived_AndACustomsDeclarationRecordAlreadyExists_ThenItIsUpdated()
    {
        var consumer = new CustomsDeclarationsConsumer(_mockLogger, _mockApi);

        var mrn = GenerateMrn();
        var clearanceDecision = DataApiClearanceDecisionFixture().Create();
        var clearanceRequest = ClearanceRequestFixture(mrn).Create();
        var finalisation = DataApiFinalisationFixture().Create();
        var existingClearanceRequest = DataApiClearanceRequestFixture().Create();
        const string expectedEtag = "12345";

        var response = new CustomsDeclarationResponse(
            mrn,
            existingClearanceRequest,
            clearanceDecision,
            finalisation,
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
                Arg.Is<DataApiCustomsDeclaration.CustomsDeclaration>(d =>
                    d.ClearanceRequest != null
                    && d.ClearanceDecision == clearanceDecision
                    && d.Finalisation == finalisation
                ),
                expectedEtag,
                _cancellationToken
            );
    }

    [Fact]
    public async Task OnHandle_WhenClearanceRequestReceived_ButExistingClearanceRequestIsNewer_ThenItIsSkipped()
    {
        var consumer = new CustomsDeclarationsConsumer(_mockLogger, _mockApi);

        var mrn = GenerateMrn();
        var clearanceRequest = ClearanceRequestFixture(mrn, 1).Create();
        var existingClearanceRequest = DataApiClearanceRequestFixture().With(cr => cr.ExternalVersion, 2).Create();
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
            .DidNotReceiveWithAnyArgs()
            .PutCustomsDeclaration(
                Arg.Any<string>(),
                Arg.Any<DataApiCustomsDeclaration.CustomsDeclaration>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }
}
