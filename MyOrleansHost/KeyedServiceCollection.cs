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

    public interface IKeyedServiceCollectionBuilder<TKey, TService> : IEnumerable<KeyValuePair<TKey, Func<TService>>>
    {
        IServiceProvider ApplicationServices { get; }
        void AddService(TKey key, Func<TService> serviceFactory);
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
            foreach (var factory in this.builder)
            {
                this.keyedServices.Add(factory.Key, factory.Value());
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

            public IEnumerator<KeyValuePair<string, Func<THostedService>>> GetEnumerator() => keyedServiceFactories.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}