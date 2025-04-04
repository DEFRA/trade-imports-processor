using Microsoft.Extensions.Options;

namespace Defra.TradeImportsProcessor.Processor.Extensions;

public static class OptionsExtensions
{
    public static OptionsBuilder<TOptions> AddValidateOptions<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string section
    )
        where TOptions : class
    {
        var s = services.AddOptions<TOptions>().BindConfiguration(section).ValidateDataAnnotations();

        return s;
    }

    public static TOptions Get<TOptions>(this OptionsBuilder<TOptions> optionsBuilder)
        where TOptions : class
    {
        return optionsBuilder.Services.GetOptions<TOptions>();
    }

    private static TOptions GetOptions<TOptions>(this IServiceCollection services)
        where TOptions : class
    {
        return services.BuildServiceProvider().GetRequiredService<IOptions<TOptions>>().Value;
    }
}
