using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Orleans.Hosting
{
    //
    // Summary:
    //     Extension methods for setting up MVC services in an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    public static class SiloServiceCollectionExtensions
    {
        /// <summary>Configures default options that will be passed to every named service (provider) when configuring them. These options can be overritten.</summary>
        public static IServiceCollection ConfigureOrleansDefaultOptions(this IServiceCollection services, IConfigurationSection defaultOptions)
        {
            // see if we want to use a different wrapper interface to avoid collisions (ie: DefaultConfiguration)
            services.AddSingleton<IConfigurationSection>(defaultOptions);
            return services;
        }

        public static IServiceCollection AddTestHooks(this IServiceCollection services) { return services; }
    }
}