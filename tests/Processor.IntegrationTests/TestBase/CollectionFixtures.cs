using Defra.TradeImportsProcessor.Processor.IntegrationTests.Clients;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;

[CollectionDefinition("UsesServiceBus")]
public class ServiceBusCollection : ICollectionFixture<ServiceBusFixture>;

[CollectionDefinition("UsesWireMockClient")]
public class WireMockClientCollection : ICollectionFixture<WireMockClient>;

[CollectionDefinition("UsesWireMockClientAndServiceBus")]
public class WiremockAndServiceBusCollection
    : ICollectionFixture<WireMockClient>,
        ICollectionFixture<ServiceBusFixture>;
