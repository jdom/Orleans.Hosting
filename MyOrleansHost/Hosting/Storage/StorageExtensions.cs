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
        public static INamedServiceCollectionBuilder<IStorageProvider> AddMemory(this INamedServiceCollectionBuilder<IStorageProvider> storageBuilder, string name)
        {
            // throw new NotImplementedException();
            return storageBuilder;
        }

        private static AzureBlobStorageProvider CreateAzureBlobStorageProvider(IServiceProvider sp, string name, IOptions<AzureBlobStorageOptions> options)
        {
            return ActivatorUtilities.CreateInstance<AzureBlobStorageProvider>(sp, name, options);
        }

        public static INamedServiceCollectionBuilder<IStorageProvider> AddAzureBlob(this INamedServiceCollectionBuilder<IStorageProvider> storageBuilder, string name, Action<IConfigureOptionsBuilder<AzureBlobStorageOptions>> configureOptions = null)
        {
            var configureOptionsBuilder = new ConfigureOptionsBuilder<AzureBlobStorageOptions>(storageBuilder.ApplicationServices.GetService<IConfigurationSection>());
            configureOptions?.Invoke(configureOptionsBuilder);
            storageBuilder.AddService(name, () => CreateAzureBlobStorageProvider(storageBuilder.ApplicationServices, name, configureOptionsBuilder));
            return storageBuilder;
        }

        public static INamedServiceCollectionBuilder<IStorageProvider> AddAzureBlob(this INamedServiceCollectionBuilder<IStorageProvider> storageBuilder, string name, AzureBlobStorageOptions options)
        {
            storageBuilder.AddService(name, () => CreateAzureBlobStorageProvider(storageBuilder.ApplicationServices, name, new OptionsWrapper<AzureBlobStorageOptions>(options)));
            return storageBuilder;
        }

        #region alternative (deprecated)

        public static IConfigureOptionsBuilder<AzureBlobStorageOptions> AddAzureBlobFluent(this INamedServiceCollectionBuilder<IStorageProvider> storageBuilder, string name)
        {
            // Alternative implementation that allows chaining configuration
            var configureOptionsBuilder = new ConfigureOptionsBuilder<AzureBlobStorageOptions>(storageBuilder.ApplicationServices.GetService<IConfigurationSection>());
            storageBuilder.AddService(name, () => CreateAzureBlobStorageProvider(storageBuilder.ApplicationServices, name, configureOptionsBuilder));
            return configureOptionsBuilder;
        }

        public static INamedServiceCollectionBuilder<IStorageProvider> AddAzureBlobAlternative(this INamedServiceCollectionBuilder<IStorageProvider> storageBuilder, string name, AzureBlobStorageOptions options)
        {
            storageBuilder.AddService(name, () => CreateAzureBlobStorageProviderAlternative(storageBuilder.ApplicationServices, name, options));
            return storageBuilder;
        }

        public static INamedServiceCollectionBuilder<IStorageProvider> AddAzureBlobAlternative(this INamedServiceCollectionBuilder<IStorageProvider> storageBuilder, string name, IConfigurationSection configuration)
        {
            return AddAzureBlobAlternative(storageBuilder, name, configuration.Get<AzureBlobStorageOptions>());
        }

        private static AzureBlobStorageProviderAlternative CreateAzureBlobStorageProviderAlternative(IServiceProvider sp, string name, AzureBlobStorageOptions options)
        {
            return ActivatorUtilities.CreateInstance<AzureBlobStorageProviderAlternative>(sp, name, options);
        }
        #endregion alternative (deprecated)
    }
}
