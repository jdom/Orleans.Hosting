using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using System;
using System.Linq;

namespace Orleans.Hosting
{
    //
    // Summary:
    //     Extension methods for setting up MVC services in an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    public static class SiloServiceCollectionExtensions
    {
        // this extension method would be defined in the Azure nuget package
        public static IServiceCollection UseAzureTableMembership(this IServiceCollection services, Action<IConfigureOptionsBuilder<AzureBlobStorageOptions>> configureOptions)
        {
            // services.AddSingleton<IMembershipProvider, AzureTableMembershipProvider>();
            return services;
        }

        public static IServiceCollection AddTestHooks(this IServiceCollection services) { return services; }
    }
}