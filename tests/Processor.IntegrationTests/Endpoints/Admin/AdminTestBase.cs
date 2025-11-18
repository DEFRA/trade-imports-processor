using System.Net;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;
using WireMock.Client;
using WireMock.Client.Extensions;
using Xunit.Abstractions;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Endpoints.Admin;

public class AdminTestBase(ITestOutputHelper output) : SqsTestBase(output) { }
