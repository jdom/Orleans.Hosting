using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Orleans.Hosting
{
    public static class ServiceProviderExtensions
    {
        public static TService GetServiceByKey<TKey, TService>(this IServiceProvider services, TKey key)
        {
            IKeyedServiceCollection<TKey, TService> collection = services.GetRequiredService<IKeyedServiceCollection<TKey, TService>>();
            return collection.GetService(key);
        }

        public static TService GetServiceByName<TService>(this IServiceProvider services, string key)
        {
            IKeyedServiceCollection<string, TService> collection = services.GetRequiredService<IKeyedServiceCollection<string, TService>>();
            return collection.GetService(key);
        }
    }

    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        public static void AddNamedHostedServiceCollection<THostedService>(this IServiceCollection services)
            where THostedService : IHostedService
        {
            services.AddSingleton<INamedServiceCollection<THostedService>, NamedServiceCollection<THostedService>>();
            services.AddFromExisting<IKeyedServiceCollection<string, THostedService>, INamedServiceCollection<THostedService>>();
            services.AddFromExisting<IHostedService, INamedServiceCollection<THostedService>>();
            services.AddSingleton<INamedServiceCollectionBuilder<THostedService>, NamedServiceCollection<THostedService>.Builder>();
            services.AddFromExisting<IKeyedServiceCollectionBuilder<string, THostedService>, INamedServiceCollectionBuilder<THostedService>>();
        }

        public static void AddNamedHostedServiceCollections(this IServiceCollection services)
        {
            Type openGeneric = typeof(IKeyedServiceCollection<,>).GetGenericArguments()[1];
            services.AddSingleton(typeof(INamedServiceCollection<>), typeof(NamedServiceCollection<>));
            services.AddFromExisting(typeof(IKeyedServiceCollection<,>).MakeGenericType(typeof(string), openGeneric), typeof(INamedServiceCollection<>));
            services.AddFromExisting(typeof(IHostedService), typeof(INamedServiceCollection<>));
            services.AddSingleton(typeof(INamedServiceCollectionBuilder<>), typeof(NamedServiceCollection<>.Builder));
            services.AddFromExisting(typeof(IKeyedServiceCollectionBuilder<,>).MakeGenericType(typeof(string), openGeneric), typeof(INamedServiceCollectionBuilder<>));
        }

        /// <summary>
        /// Attempts to use an existing registration of <typeparamref name="TImplementation"/> to satisfy the service type <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The service type being provided.</typeparam>
        /// <typeparam name="TImplementation">The implementation of <typeparamref name="TService"/>.</typeparam>
        /// <param name="services">The service collection.</param>
        public static void AddFromExisting<TService, TImplementation>(this IServiceCollection services) where TImplementation : TService
        {
            var registration = services.FirstOrDefault(service => service.ServiceType == typeof(TImplementation));
            if (registration != null)
            {
                var newRegistration = new ServiceDescriptor(
                    typeof(TService),
                    sp => sp.GetRequiredService<TImplementation>(),
                    registration.Lifetime);
                services.Add(newRegistration);
            }
        }

        public static void AddFromExisting(this IServiceCollection services, Type serviceType, Type implementationType)
        {
            // TODO: validate hierarchy
            var registration = services.FirstOrDefault(service => service.ServiceType == implementationType);
            if (registration != null)
            {
                var typesToResolve = new[] { implementationType.MakeGenericType(typeof(IStorageProvider)), implementationType.MakeGenericType(typeof(IStreamProvider)) };
                Func<IServiceProvider, object> resolver = sp => new[] { sp.GetRequiredService(typesToResolve[0]), sp.GetRequiredService(typesToResolve[1]) };
                var newRegistration = new ServiceDescriptor(
                    serviceType, 
                    resolver,
                    //sp => sp.GetRequiredService(implementationType),
                    registration.Lifetime);
                services.Add(newRegistration);
            }
        }

        public static IServiceCollection Clone(this IServiceCollection serviceCollection)
        {
            IServiceCollection clone = new ServiceCollection();
            foreach (var service in serviceCollection)
            {
                clone.Add(service);
            }
            return clone;
        }
    }
}
