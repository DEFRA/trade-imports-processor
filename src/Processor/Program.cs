using Defra.TradeImportsProcessor.Processor.Authentication;
using Defra.TradeImportsProcessor.Processor.Endpoints;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Health;
using Defra.TradeImportsProcessor.Processor.Metrics;
using Defra.TradeImportsProcessor.Processor.Utils;
using Defra.TradeImportsProcessor.Processor.Utils.Http;
using Defra.TradeImportsProcessor.Processor.Utils.Logging;
using Elastic.CommonSchema.Serilog;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console(new EcsTextFormatter()).CreateBootstrapLogger();

try
{
    var app = CreateWebApplication(args);
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    await Log.CloseAndFlushAsync();
}

return;

static WebApplication CreateWebApplication(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    ConfigureWebApplication(builder, args);

    return BuildWebApplication(builder);
}

static void ConfigureWebApplication(WebApplicationBuilder builder, string[] args)
{
    var integrationTest = args.Contains("--integrationTest=true");

    builder.Configuration.AddJsonFile(
        $"appsettings.cdp.{Environment.GetEnvironmentVariable("ENVIRONMENT")?.ToLower()}.json",
        integrationTest
    );
    builder.Configuration.AddEnvironmentVariables();

    // Load certificates into Trust Store - Note must happen before Mongo and Http client connections
    builder.Services.AddCustomTrustStore();

    builder.ConfigureLoggingAndTracing();

    builder.Services.AddAuthenticationAuthorization();
    builder.Services.AddProblemDetails();
    builder.Services.AddHealth(builder.Configuration);
    builder.Services.AddProcessorConfiguration(builder.Configuration);
    builder.Services.AddValidators();

    builder.Services.AddDataApiHttpClient();
    builder.Services.AddHttpProxyClient();

    builder.Services.AddConsumers(builder.Configuration);

    builder.Services.AddTransient<MetricsMiddleware>();
    builder.Services.AddSingleton<RequestMetrics>();
}

static WebApplication BuildWebApplication(WebApplicationBuilder builder)
{
    var app = builder.Build();

    app.UseEmfExporter();
    app.MapHealth();
    app.UseStatusCodePages();
    app.UseHeaderPropagation();
    app.UseMiddleware<MetricsMiddleware>();
    app.MapReplayEndpoints();
    app.UseExceptionHandler(
        new ExceptionHandlerOptions
        {
            AllowStatusCode404Response = true,
            ExceptionHandler = async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                var error = exceptionHandlerFeature?.Error;
                string? detail = null;

                if (error is BadHttpRequestException badHttpRequestException)
                {
                    context.Response.StatusCode = badHttpRequestException.StatusCode;
                    detail = badHttpRequestException.Message;
                }

                await context
                    .RequestServices.GetRequiredService<IProblemDetailsService>()
                    .WriteAsync(
                        new ProblemDetailsContext
                        {
                            HttpContext = context,
                            AdditionalMetadata = exceptionHandlerFeature?.Endpoint?.Metadata,
                            ProblemDetails = { Status = context.Response.StatusCode, Detail = detail },
                        }
                    );
            },
        }
    );

    return app;
}

#pragma warning disable S2094
namespace Defra.TradeImportsProcessor.Processor
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Program;
}
#pragma warning restore S2094
