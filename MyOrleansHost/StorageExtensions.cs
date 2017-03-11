using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton<INamedServiceCollection<IStorageProvider>, NamedServiceCollection<IStorageProvider>>();
            services.AddFromExisting<IHostedService, INamedServiceCollection<IStorageProvider>>();
            services.AddSingleton<INamedServiceCollectionBuilder<IStorageProvider>, NamedServiceCollection<IStorageProvider>.Builder>();
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
    }
}
