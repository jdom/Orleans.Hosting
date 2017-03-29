using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans.Hosting.Membership;
using System;

namespace Orleans.Hosting
{
    public static class MembershipExtensions
    {
        // this extension method would be defined in the Azure nuget package
        public static IServiceCollection AddAzureTableMembership(this IServiceCollection services, Action<IConfigureOptionsBuilder<AzureTableMembershipOptions>> configureOptions)
        {
            services.AddSingleton<IMembershipProvider, AzureTableMembershipProvider>();
            services.AddFromExisting<IHostedService, IMembershipProvider>();
            services.AddSingleton<IOptions<AzureTableMembershipOptions>>(sp => AzureTableMembershipOptionsFactory(sp, configureOptions));

            return services;
        }

        public static IServiceCollection AddAzureTableMembership(this IServiceCollection services)
        {
            return AddAzureTableMembership(services, (Action<IConfigureOptionsBuilder<AzureTableMembershipOptions>>)null);
        }

        public static IServiceCollection AddAzureTableMembership(this IServiceCollection services, AzureTableMembershipOptions options)
        {
            services.AddSingleton<IMembershipProvider, AzureTableMembershipProvider>();
            services.AddFromExisting<IHostedService, IMembershipProvider>();
            services.AddSingleton<IOptions<AzureTableMembershipOptions>>(new OptionsWrapper<AzureTableMembershipOptions>(options));

            return services;
        }

        private static IOptions<AzureTableMembershipOptions> AzureTableMembershipOptionsFactory(IServiceProvider sp, Action<IConfigureOptionsBuilder<AzureTableMembershipOptions>> configureOptions)
        {
            var configureOptionsBuilder = new ConfigureOptionsBuilder<AzureTableMembershipOptions>(sp.GetService<IConfigurationSection>());
            configureOptions?.Invoke(configureOptionsBuilder);
            return configureOptionsBuilder;
        }

        public static IServiceCollection AddAzureTableMembership(this IServiceCollection services, IConfigurationSection configuration)
        {
            return AddAzureTableMembership(services, builder => builder.Configure(configuration));
        }

        public static IServiceCollection AddAzureTableMembership(this IServiceCollection services, string connectionString)
        {
            return AddAzureTableMembership(services, builder => builder.Configure(x => x.ConnectionString = connectionString));
        }
    }
}
