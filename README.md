# Trade Imports Processor

The Trade Imports Processor is a .NET application which subscribes to a number of messages.

When messages are received the Imports Processor will map the messages to internal BTMS types and then use the Trade Imports API via a Http to persist the data.

* [Prerequisites](#prerequisites)
* [Setup Process](#setup-process)
* [How to run in development](#how-to-run-in-development)
* [How to run Tests](#how-to-run-tests)
* [Running](#running)
* [Deploying](#deploying)
* [SonarCloud](#sonarCloud)
* [Dependabot](#dependabot)
* [Message Consumption](#message-consumption)
* [Tracing](#tracing)
* [Licence Information](#licence-information)
* [About the Licence](#about-the-licence)

### Prerequisites

- .NET 9 SDK
- Docker
  - localstack - used for local queuing
  - wiremock - used for mocking out http requests
  - service-bus-emulator - used for local queuing


### Setup Process

- Install the .NET 9 SDK
- Install Docker
  - Run the following Docker Compose to set up locally running queues for testing
  ```bash
  docker compose -f compose.yml up -d
  ```

### How to run in development

Run the application with the command:

```bash
dotnet run --project src/Processor/Processor.csproj
```

### How to run Tests

Run the unit tests with:

```bash
dotnet test --filter "Category!=IntegrationTest"
```
Run the integration tests with:
```bash
dotnet test --filter "Category=IntegrationTest"
```
Run all tests with:
```bash
dotnet test
```

#### Unit Tests
Some unit tests may run an in memory instance service.

Unit tests that need to edit the value of an application setting can be done via the`appsettings.IntegrationTests.json`

#### Integration Tests
Integration tests run against the built docker image.

Because these use the built docker image, the `appsettings.json` will be used, should any values need to be overridden, then they can be injected as an environment variable via the`compose.yml`

### Deploying

Before deploying via CDP set the correct config for the environment as per the `appsettings.Development.json`.

### SonarCloud

Example SonarCloud configuration are available in the GitHub Action workflows.

### Dependabot

We are using dependabot.

Connection to the private Defra nuget packages is provided by a user generated PAT stored in this repo's settings - /settings/secrets/dependabot - see `DEPENDABOT_PAT` secret.

The PAT is a classic token and needs permissions of `read:packages`.

At time of writing, using PAT is the only way to make Dependabot work with private nuget feeds.

Should the user who owns the PAT leave Defra then another user on the team should create a new PAT and update the settings in this repo.

### Message Consumption
This service is using a framework called [Slim Message Bus](https://github.com/zarusz/SlimMessageBus), which maps queues/types to consumer classes.

SMB doesn't support [filtering on message headers](https://github.com/zarusz/SlimMessageBus/issues/402), so there is a custom `ConsumerMediator` class that has been implemented to facilitate this.

### Tracing
The out of the box CDP template doesn't provide any example of how to handle tracing for non Http communication.
This service expected the trace.id to be a header on the message, and it will use that and propagate that to the Trade Imports Api via Http Requests.

Getting the trace.id header is achieved via a SMB `TraceContextInterceptor`
Making sure that trace.id is then used in log messages is achieved via `TraceContextEnricher`
Setting the trace.id header on Http Request is achieved via Header Propagation


### Licence Information

THIS INFORMATION IS LICENSED UNDER THE CONDITIONS OF THE OPEN GOVERNMENT LICENCE found at:

<http://www.nationalarchives.gov.uk/doc/open-government-licence/version/3>

### About the licence

The Open Government Licence (OGL) was developed by the Controller of Her Majesty's Stationery Office (HMSO) to enable information providers in the public sector to license the use and re-use of their information under a common open licence.

It is designed to encourage use and re-use of information freely and flexibly, with only a few conditions.


