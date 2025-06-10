using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Microsoft.Extensions.Options;

namespace Defra.TradeImportsProcessor.Processor.Utils.Http;

public static class Proxy
{
    private const string ProxyClient = "proxy";

    [ExcludeFromCodeCoverage]
    public static void AddHttpProxyClient(this IServiceCollection services)
    {
        // Some .net connections use this http client
        services.AddHttpClient(ProxyClient).ConfigurePrimaryHttpMessageHandler(ConfigurePrimaryHttpMessageHandler);

        // Others, including the Azure SDK, rely on this, falling back to HTTPS_PROXY.
        // HTTPS_PROXY if used must not include the protocol
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<CdpOptions>>();
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger(ProxyClient);

            return CreateProxy(options.Value.CdpHttpsProxy, logger);
        });
    }

    [ExcludeFromCodeCoverage]
    private static HttpClientHandler ConfigurePrimaryHttpMessageHandler(IServiceProvider sp)
    {
        var options = sp.GetRequiredService<IOptions<CdpOptions>>();
        var proxy = sp.GetRequiredService<IWebProxy>();
        return CreateHttpClientHandler(proxy, options.Value.CdpHttpsProxy!);
    }

    public static HttpClientHandler CreateHttpClientHandler(IWebProxy proxy, string? proxyUri)
    {
        return new HttpClientHandler { Proxy = proxy, UseProxy = proxyUri != null };
    }

    public static IWebProxy CreateProxy(string? proxyUri, ILogger logger)
    {
        var proxy = new WebProxy { BypassProxyOnLocal = true };
        if (proxyUri != null)
        {
            ConfigureProxy(proxy, proxyUri, logger);
        }
        else
        {
            logger.LogWarning("CDP_HTTPS_PROXY is NOT set, proxy client will be disabled");
        }

        return proxy;
    }

    public static void ConfigureProxy(WebProxy proxy, string proxyUri, ILogger logger)
    {
        logger.LogDebug("Creating proxy http client");
        var uri = new UriBuilder(proxyUri);

        var credentials = GetCredentialsFromUri(uri);
        if (credentials != null)
        {
            logger.LogDebug("Setting proxy credentials");
            proxy.Credentials = credentials;
        }

        // Remove credentials from URI to so they don't get logged.
        uri.UserName = "";
        uri.Password = "";
        proxy.Address = uri.Uri;
    }

    private static NetworkCredential? GetCredentialsFromUri(UriBuilder uri)
    {
        var username = uri.UserName;
        var password = uri.Password;
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return null;
        return new NetworkCredential(username, password);
    }
}
