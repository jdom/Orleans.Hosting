using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;

namespace Orleans.Hosting
{
    public interface IKeyedServiceCollection<THostedService> : IReadOnlyCollection<KeyValuePair<string, THostedService>>, IHostedService
        where THostedService : IHostedService
    {
        THostedService GetService(string name);
    }

    public interface IKeyedServiceCollectionBuilder<THostedService> : IEnumerable<KeyValuePair<string, Func<THostedService>>>
        where THostedService : IHostedService
    {
        IServiceProvider ApplicationServices { get; }
        void AddService(string name, Func<THostedService> serviceFactory);
    }

    public class KeyedServiceCollection<THostedService> : IKeyedServiceCollection<THostedService>
        where THostedService : IHostedService
    {
        private readonly Dictionary<string, THostedService> keyedServices = new Dictionary<string, THostedService>();
        private IKeyedServiceCollectionBuilder<THostedService> builder;

        public KeyedServiceCollection(IKeyedServiceCollectionBuilder<THostedService> builder)
        {
            this.builder = builder;
        }
        public int Count => keyedServices.Count;
        public IEnumerator<KeyValuePair<string, THostedService>> GetEnumerator() => keyedServices.GetEnumerator();
        public THostedService GetService(string name) => keyedServices[name];

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

        public class KeyedServiceCollectionBuilder : IKeyedServiceCollectionBuilder<THostedService>
        {
            private readonly Dictionary<string, Func<THostedService>> keyedServiceFactories = new Dictionary<string, Func<THostedService>>();

            public KeyedServiceCollectionBuilder(IServiceProvider serviceProvider)
            {
                this.ApplicationServices = serviceProvider;
            }

            public IServiceProvider ApplicationServices { get; }

            public void AddService(string name, Func<THostedService> serviceFactory)
            {
                this.keyedServiceFactories.Add(name, serviceFactory);
            }

            public IEnumerator<KeyValuePair<string, Func<THostedService>>> GetEnumerator() => keyedServiceFactories.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
   
}