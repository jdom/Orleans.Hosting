using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Orleans.Hosting
{
    public static class BuilderStorageExtensions
    {
        public static IApplicationBuilder ConfigureStorageProviders(this IApplicationBuilder app, Action<IKeyedServiceCollectionBuilder<IStorageProvider>> configureProviders)
        {
            var storageBuilder = app.ApplicationServices.GetRequiredService<IKeyedServiceCollectionBuilder<IStorageProvider>>();
            configureProviders(storageBuilder);
            return app;
        }

        public static void AddStorageProviders(this IServiceCollection services)
        {
            services.AddSingleton<IKeyedServiceCollection<IStorageProvider>, KeyedServiceCollection<IStorageProvider>>();
            services.AddFromExisting<IHostedService, IKeyedServiceCollection<IStorageProvider>>();
            services.AddSingleton<IKeyedServiceCollectionBuilder<IStorageProvider>, KeyedServiceCollection<IStorageProvider>.KeyedServiceCollectionBuilder>();
        }
    }

    public static class StorageProvidersBuilderExtensions
    {
        public static IKeyedServiceCollectionBuilder<IStorageProvider> AddAzureBlob(this IKeyedServiceCollectionBuilder<IStorageProvider> storageBuilder, string name, AzureBlobStorageOptions options)
        {
            storageBuilder.AddService(name, () => CreateAzureBlobStorageProvider(storageBuilder.ApplicationServices, name, options));
            return storageBuilder;
        }

        public static IKeyedServiceCollectionBuilder<IStorageProvider> AddAzureBlob(this IKeyedServiceCollectionBuilder<IStorageProvider> storageBuilder, string name, IConfigurationSection configuration)
        {
            return AddAzureBlob(storageBuilder, name, configuration.Get<AzureBlobStorageOptions>());
        }

        private static AzureBlobStorageProvider CreateAzureBlobStorageProvider(IServiceProvider sp, string name, AzureBlobStorageOptions options)
        {
            return ActivatorUtilities.CreateInstance<AzureBlobStorageProvider>(sp, name, options);
        }
        public static IKeyedServiceCollectionBuilder<IStorageProvider> AddMemory(this IKeyedServiceCollectionBuilder<IStorageProvider> storageBuilder, string name)
        {
            // throw new NotImplementedException();
            return storageBuilder;
        }
    }
}
