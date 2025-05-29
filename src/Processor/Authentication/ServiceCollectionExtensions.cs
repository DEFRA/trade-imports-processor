using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Microsoft.AspNetCore.Authentication;

namespace Defra.TradeImportsProcessor.Processor.Authentication;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationAuthorization(this IServiceCollection services)
    {
        services.AddOptions<AclOptions>().BindConfiguration("Acl");

        services
            .AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
                BasicAuthenticationHandler.SchemeName,
                _ => { }
            );

        services.AddAuthorizationBuilder();

        return services;
    }
}
