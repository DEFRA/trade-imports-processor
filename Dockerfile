# Base dotnet image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Add curl to template.
# CDP PLATFORM HEALTHCHECK REQUIREMENT
RUN apt update && \
    apt upgrade -y && \
    apt install curl -y && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY .config/dotnet-tools.json .config/dotnet-tools.json
COPY .csharpierrc .csharpierrc

RUN dotnet tool restore

COPY src/Processor/Processor.csproj src/Processor/Processor.csproj
COPY tests/Testing/Testing.csproj tests/Testing/Testing.csproj
COPY tests/TestFixtures/TestFixtures.csproj tests/TestFixtures/TestFixtures.csproj
COPY tests/Processor.Tests/Processor.Tests.csproj tests/Processor.Tests/Processor.Tests.csproj
COPY tests/Processor.IntegrationTests/Processor.IntegrationTests.csproj tests/Processor.IntegrationTests/Processor.IntegrationTests.csproj
COPY Defra.TradeImportsProcessor.sln Defra.TradeImportsProcessor.sln
COPY Directory.Build.props Directory.Build.props

RUN dotnet restore

COPY src/Processor src/Processor
COPY tests/Testing tests/Testing
COPY tests/TestFixtures tests/TestFixtures
COPY tests/Processor.Tests tests/Processor.Tests

RUN dotnet csharpier --check .

RUN dotnet build src/Processor/Processor.csproj --no-restore -c Release

RUN dotnet test --no-restore tests/Processor.Tests

FROM build AS publish

RUN dotnet publish src/Processor -c Release -o /app/publish /p:UseAppHost=false

ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8085
ENTRYPOINT ["dotnet", "Defra.TradeImportsProcessor.Processor.dll"]
