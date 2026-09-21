using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Google.SearchConsole.Abstract;
using Soenneker.Google.Credentials.Registrars;

namespace Soenneker.Google.SearchConsole.Registrars;

/// <summary>
/// A utility library for Google Search Console.
/// </summary>
public static class GoogleSearchConsoleUtilRegistrar
{
    /// <summary>
    /// Adds <see cref="IGoogleSearchConsoleUtil"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddGoogleSearchConsoleUtilAsSingleton(this IServiceCollection services)
    {
        services.AddGoogleCredentialsUtilAsSingleton();
        services.TryAddSingleton<IGoogleSearchConsoleUtil, GoogleSearchConsoleUtil>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="IGoogleSearchConsoleUtil"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddGoogleSearchConsoleUtilAsScoped(this IServiceCollection services)
    {
        services.AddGoogleCredentialsUtilAsScoped();
        services.TryAddScoped<IGoogleSearchConsoleUtil, GoogleSearchConsoleUtil>();

        return services;
    }
}
