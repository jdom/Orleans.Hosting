using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Orleans.Hosting
{
    public static class BuilderStreamExtensions
    {
        public static IApplicationBuilder ConfigureStreamProviders(this IApplicationBuilder app, Action<INamedServiceCollectionBuilder<IStreamProvider>> configureProviders)
        {
            var streamBuilder = app.ApplicationServices.GetRequiredService<INamedServiceCollectionBuilder<IStreamProvider>>();
            configureProviders(streamBuilder);
            return app;
        }

        public static void AddStreamProviders(this IServiceCollection services)
        {
            services.AddNamedHostedServiceCollection<IStreamProvider>();
        }
    }

    public static class StreamProvidersBuilderExtensions
    {
        public static INamedServiceCollectionBuilder<IStreamProvider> AddEventHub(this INamedServiceCollectionBuilder<IStreamProvider> builder, string name, Action<IConfigureOptionsBuilder<EventHubStreamOptions>> configureOptions = null)
        {
            var configureOptionsBuilder = new ConfigureOptionsBuilder<EventHubStreamOptions>(builder.ApplicationServices.GetService<IConfigurationSection>());
            configureOptions?.Invoke(configureOptionsBuilder);
            builder.AddService(name, () => CreateEventHubStreamProvider(builder.ApplicationServices, name, configureOptionsBuilder));
            return builder;
        }

        public static INamedServiceCollectionBuilder<IStreamProvider> AddEventHub(this INamedServiceCollectionBuilder<IStreamProvider> builder, string name, EventHubStreamOptions options)
        {
            builder.AddService(name, () => CreateEventHubStreamProvider(builder.ApplicationServices, name, new OptionsWrapper<EventHubStreamOptions>(options)));
            return builder;
        }

        private static EventHubStreamProvider CreateEventHubStreamProvider(IServiceProvider sp, string name, IOptions<EventHubStreamOptions> options)
        {
            return ActivatorUtilities.CreateInstance<EventHubStreamProvider>(sp, name, options);
        }

        public static INamedServiceCollectionBuilder<IStreamProvider> AddSms(this INamedServiceCollectionBuilder<IStreamProvider> builder, string name)
        {
            // builder.AddService(name, () => CreateEventHubStreamProvider(builder.ApplicationServices, name, new OptionsWrapper<EventHubStreamOptions>(options)));
            return builder;
        }
    }
}
