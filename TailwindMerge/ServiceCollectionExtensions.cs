using Microsoft.Extensions.DependencyInjection;

namespace TailwindMerge;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a singleton instance of the <see cref="TwMerge"/> service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    public static void AddTwMerge(this IServiceCollection services)
    {
        services.AddSingleton<TwMerge>();
    }

    /// <summary>
    /// Registers a singleton instance of the <see cref="TwMerge"/> service with the given configuration options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="options">The delegate to configure the <see cref="TwConfig"/> options.</param>
    public static void AddTwMerge(this IServiceCollection services, Action<TwConfig> options)
    {
        var twConfig = new TwConfig();

        options?.Invoke(twConfig);

        services.AddSingleton(x => new TwMerge(twConfig));
    }
}
