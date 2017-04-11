using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.Text;

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
            EnsureServicesBuilt();

            return Task.WhenAll(keyedServices.Values.Select(x => x.Start()));
        }

        public Task Stop()
        {
            return Task.WhenAll(keyedServices.Values.Select(x => x.Stop()));
        }

        public override string ToString()
        {
            EnsureServicesBuilt();

            var sb = new StringBuilder();
            sb.AppendLine($"Named service collection of {typeof(THostedService).Name}:");
            foreach (var service in this.keyedServices)
            {
                sb.AppendLine($"  [{service.Key}]: {service.Value}");
            }
            return sb.ToString();
        }

        private void EnsureServicesBuilt()
        {
            var services = this.builder?.Build();
            if (services != null)
            {
                foreach (var service in services)
                {
                    this.keyedServices.Add(service.Key, service.Value);
                }
            }

            this.builder = null;
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