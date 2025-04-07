using Defra.TradeImportsProcessor.Processor.Utils.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace Defra.TradeImportsProcessor.Processor.Tests.Utils;

public class ProxyTest
{
    private const string ProxyUri = "http://user:password@localhost:8080";
    private const string LocalProxy = "http://localhost:8080/";
    private const string Localhost = "http://localhost/";

    [Fact]
    public void ExtractProxyCredentials()
    {
        var proxy = new System.Net.WebProxy { BypassProxyOnLocal = true };

        Proxy.ConfigureProxy(proxy, ProxyUri, NullLogger.Instance);

        var credentials = proxy.Credentials?.GetCredential(new Uri(ProxyUri), "Basic");

        credentials?.UserName.Should().Be("user");
        credentials?.Password.Should().Be("password");
    }

    [Fact]
    public void ExtractProxyEmptyCredentials()
    {
        const string noPasswordUri = "http://user@localhost:8080";

        var proxy = new System.Net.WebProxy { BypassProxyOnLocal = true };

        Proxy.ConfigureProxy(proxy, noPasswordUri, NullLogger.Instance);

        proxy.Credentials.Should().BeNull();
    }

    [Fact]
    public void ExtractProxyUri()
    {
        var proxy = new System.Net.WebProxy { BypassProxyOnLocal = true };

        Proxy.ConfigureProxy(proxy, ProxyUri, NullLogger.Instance);
        proxy.Address.Should().NotBeNull();
        proxy.Address?.AbsoluteUri.Should().Be(LocalProxy);
    }

    [Fact]
    public void CreateProxyFromUri()
    {
        var proxy = Proxy.CreateProxy(ProxyUri, NullLogger.Instance) as System.Net.WebProxy;
        proxy?.Address.Should().NotBeNull();
        proxy?.Address?.AbsoluteUri.Should().Be(LocalProxy);
    }

    [Fact]
    public void CreateNoProxyFromEmptyUri()
    {
        var proxy = Proxy.CreateProxy(null, NullLogger.Instance) as System.Net.WebProxy;
        proxy?.Address.Should().BeNull();
    }

    [Fact]
    public void ProxyShouldBypassLocal()
    {
        var proxy = Proxy.CreateProxy(ProxyUri, NullLogger.Instance);

        proxy.IsBypassed(new Uri(Localhost)).Should().BeTrue();
        proxy.IsBypassed(new Uri("https://defra.gov.uk")).Should().BeFalse();
    }

    [Fact]
    public void HandlerShouldHaveProxy()
    {
        var proxy = Proxy.CreateProxy(ProxyUri, NullLogger.Instance);
        var handler = Proxy.CreateHttpClientHandler(proxy, ProxyUri);

        handler.Proxy.Should().NotBeNull();
        handler.UseProxy.Should().BeTrue();
        handler.Proxy?.Credentials.Should().NotBeNull();
        handler.Proxy?.GetProxy(new Uri(Localhost)).Should().NotBeNull();
        handler.Proxy?.GetProxy(new Uri("http://google.com")).Should().NotBeNull();
        handler.Proxy?.GetProxy(new Uri(Localhost))?.AbsoluteUri.Should().Be(Localhost);
        handler.Proxy?.GetProxy(new Uri("http://google.com"))?.AbsoluteUri.Should().Be(LocalProxy);
    }
}
