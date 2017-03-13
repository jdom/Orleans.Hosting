using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Orleans.Hosting
{
    public static class BuilderStorageExtensions
    {
        public static IApplicationBuilder ConfigureStorageProviders(this IApplicationBuilder app, Action<INamedServiceCollectionBuilder<IStorageProvider>> configureProviders)
        {
            var storageBuilder = app.ApplicationServices.GetRequiredService<INamedServiceCollectionBuilder<IStorageProvider>>();
            configureProviders(storageBuilder);
            return app;
        }

        public static void AddStorageProviders(this IServiceCollection services)
        {
            services.AddNamedHostedServiceCollection<IStorageProvider>();
        }
    }

    public static class StorageProvidersBuilderExtensions
    {
        public static INamedServiceCollectionBuilder<IStorageProvider> AddAzureBlob(this INamedServiceCollectionBuilder<IStorageProvider> storageBuilder, string name, AzureBlobStorageOptions options)
        {
            storageBuilder.AddService(name, () => CreateAzureBlobStorageProvider(storageBuilder.ApplicationServices, name, options));
            return storageBuilder;
        }

        public static INamedServiceCollectionBuilder<IStorageProvider> AddAzureBlob(this INamedServiceCollectionBuilder<IStorageProvider> storageBuilder, string name, IConfigurationSection configuration)
        {
            return AddAzureBlob(storageBuilder, name, configuration.Get<AzureBlobStorageOptions>());
        }

        private static AzureBlobStorageProvider CreateAzureBlobStorageProvider(IServiceProvider sp, string name, AzureBlobStorageOptions options)
        {
            return ActivatorUtilities.CreateInstance<AzureBlobStorageProvider>(sp, name, options);
        }
        public static INamedServiceCollectionBuilder<IStorageProvider> AddMemory(this INamedServiceCollectionBuilder<IStorageProvider> storageBuilder, string name)
        {
            // throw new NotImplementedException();
            return storageBuilder;
        }

        public static IConfigureOptionsBuilder<AzureBlobStorageOptions> AddAzureBlob2(this INamedServiceCollectionBuilder<IStorageProvider> storageBuilder, string name)
        {
            // Alternative implementation that allows chaining configuration
            var configureOptionsBuilder = new ConfigureOptionsBuilder<AzureBlobStorageOptions>();
            storageBuilder.AddService(name, () => CreateAzureBlobStorageProvider2(storageBuilder.ApplicationServices, name, configureOptionsBuilder));
            return configureOptionsBuilder;
        }

        private static AzureBlobStorageProvider2 CreateAzureBlobStorageProvider2(IServiceProvider sp, string name, IOptions<AzureBlobStorageOptions> options)
        {
            return ActivatorUtilities.CreateInstance<AzureBlobStorageProvider2>(sp, name, options);
        }

        public static INamedServiceCollectionBuilder<IStorageProvider> AddAzureBlob3(this INamedServiceCollectionBuilder<IStorageProvider> storageBuilder, string name, Action<IConfigureOptionsBuilder<AzureBlobStorageOptions>> configureOptions)
        {
            // Alternative implementation that allows chaining configuration
            var configureOptionsBuilder = new ConfigureOptionsBuilder<AzureBlobStorageOptions>();
            configureOptions(configureOptionsBuilder);
            storageBuilder.AddService(name, () => CreateAzureBlobStorageProvider2(storageBuilder.ApplicationServices, name, configureOptionsBuilder));
            return storageBuilder;
        }
    }
}
