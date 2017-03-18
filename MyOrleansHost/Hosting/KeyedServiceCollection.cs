using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace Orleans.Hosting
{
#region generic version of the Keyed service collection (IMO adds unnecessary cognitive load)
    public interface IKeyedServiceCollection<TKey, TService> : IReadOnlyCollection<KeyValuePair<TKey, TService>>
    {
        TService GetService(TKey key);
    }

    public interface IKeyedServiceCollectionBuilder<TKey, TService>
    {
        IServiceProvider ApplicationServices { get; }
        void AddService(TKey key, Func<TService> serviceFactory);
        IEnumerable<KeyValuePair<TKey, TService>> Build();
    }
#endregion

    public interface INamedServiceCollection<THostedService> : IKeyedServiceCollection<string, THostedService>, IHostedService
        where THostedService : IHostedService
    {
    }

    public interface INamedServiceCollectionBuilder<THostedService> : IKeyedServiceCollectionBuilder<string, THostedService>
        where THostedService : IHostedService
    {
    }

    public class NamedServiceCollection<THostedService> : INamedServiceCollection<THostedService>
        where THostedService : IHostedService
    {
        private readonly Dictionary<string, THostedService> keyedServices = new Dictionary<string, THostedService>();
        private INamedServiceCollectionBuilder<THostedService> builder;

        public NamedServiceCollection(INamedServiceCollectionBuilder<THostedService> builder)
        {
            this.builder = builder;
        }
        public int Count => keyedServices.Count;
        public IEnumerator<KeyValuePair<string, THostedService>> GetEnumerator() => keyedServices.GetEnumerator();
        public THostedService GetService(string key) => keyedServices[key];

        public Task Start()
        {
            foreach (var service in this.builder.Build())
            {
                this.keyedServices.Add(service.Key, service.Value);
            }

            return Task.WhenAll(keyedServices.Values.Select(x => x.Start()));
        }

        public Task Stop()
        {
            return Task.WhenAll(keyedServices.Values.Select(x => x.Stop()));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public class Builder : INamedServiceCollectionBuilder<THostedService>
        {
            private readonly Dictionary<string, Func<THostedService>> keyedServiceFactories = new Dictionary<string, Func<THostedService>>();

            public Builder(IServiceProvider serviceProvider)
            {
                this.ApplicationServices = serviceProvider;
            }

            public IServiceProvider ApplicationServices { get; }

            public void AddService(string key, Func<THostedService> serviceFactory)
            {
                this.keyedServiceFactories.Add(key, serviceFactory);
            }

            public IEnumerable<KeyValuePair<string, THostedService>> Build()
            {
                foreach(var factory in this.keyedServiceFactories)
                {
                    yield return new KeyValuePair<string, THostedService>(factory.Key, factory.Value());
                }
            }
        }
    }
}