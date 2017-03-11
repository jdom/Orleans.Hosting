using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Orleans.Hosting
{
    public static class BuilderStreamExtensions
    {
        public static IApplicationBuilder ConfigureStreamProviders(this IApplicationBuilder app, Action<IKeyedServiceCollectionBuilder<IStreamProvider>> configureProviders)
        {
            var streamBuilder = app.ApplicationServices.GetRequiredService<IKeyedServiceCollectionBuilder<IStreamProvider>>();
            configureProviders(streamBuilder);
            return app;
        }

        public static void AddStreamProviders(this IServiceCollection services)
        {
            services.AddSingleton<IKeyedServiceCollection<IStreamProvider>, KeyedServiceCollection<IStreamProvider>>();
            services.AddFromExisting<IHostedService, IKeyedServiceCollection<IStreamProvider>>();
            services.AddSingleton<IKeyedServiceCollectionBuilder<IStreamProvider>, KeyedServiceCollection<IStreamProvider>.KeyedServiceCollectionBuilder>();
        }
    }

    public static class StreamProvidersBuilderExtensions
    {
        public static IKeyedServiceCollectionBuilder<IStreamProvider> AddEventHub(this IKeyedServiceCollectionBuilder<IStreamProvider> streamBuilder, string name, EventHubStreamOptions options)
        {
            streamBuilder.AddService(name, () => CreateEventHubStreamProvider(streamBuilder.ApplicationServices, name, options));
            return streamBuilder;
        }

        public static IKeyedServiceCollectionBuilder<IStreamProvider> AddEventHub(this IKeyedServiceCollectionBuilder<IStreamProvider> streamBuilder, string name, IConfigurationSection configuration)
        {
            return AddEventHub(streamBuilder, name, configuration.Get<EventHubStreamOptions>());
        }

        private static EventHubStreamProvider CreateEventHubStreamProvider(IServiceProvider sp, string name, EventHubStreamOptions options)
        {
            return ActivatorUtilities.CreateInstance<EventHubStreamProvider>(sp, name, options);
        }
        public static IKeyedServiceCollectionBuilder<IStreamProvider> AddSms(this IKeyedServiceCollectionBuilder<IStreamProvider> streamBuilder, string name)
        {
            // throw new NotImplementedException();
            return streamBuilder;
        }
    }
}
