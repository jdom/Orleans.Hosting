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


    public interface IConfigureOptionsBuilder<TOptions> : IOptions<TOptions>
        where TOptions : class, new()
    {
        IConfigureOptionsBuilder<TOptions> Configure(IConfigureOptions<TOptions> configureOptions);
    }

    public static class ConfigureOptionsBuilderExtensions
    {
        /// <summary>
        /// Registers an action used to configure a particular type of options.
        /// </summary>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="builder">The <see cref="IConfigureOptionsBuilder<TOptions>"/> to add the services to.</param>
        /// <param name="configureOptions">The action used to configure the options.</param>
        /// <returns>The <see cref="IConfigureOptionsBuilder<TOptions>"/> so that additional calls can be chained.</returns>
        public static IConfigureOptionsBuilder<TOptions> Configure<TOptions>(this IConfigureOptionsBuilder<TOptions> builder, Action<TOptions> configureOptions)
            where TOptions : class, new()
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            builder.Configure(new ConfigureOptions<TOptions>(configureOptions));
            return builder;
        }

        /// <summary>
        /// Registers a configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="builder">The builder to add the services to.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <returns>The <see cref="IConfigureOptionsBuilder<TOptions>"/> so that additional calls can be chained.</returns>
        public static IConfigureOptionsBuilder<TOptions> Configure<TOptions>(this IConfigureOptionsBuilder<TOptions> builder, IConfiguration config)
            where TOptions : class, new()
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return builder.Configure(new ConfigureFromConfigurationOptions<TOptions>(config));
        }
    }

    public class ConfigureOptionsBuilder<TOptions> : IConfigureOptionsBuilder<TOptions>
        where TOptions : class, new()
    {
        private List<IConfigureOptions<TOptions>> list = new List<IConfigureOptions<TOptions>>();

        //TODO: optimize
        public TOptions Value => new OptionsManager<TOptions>(list).Value;

        public IConfigureOptionsBuilder<TOptions> Configure(IConfigureOptions<TOptions> configureOptions)
        {
            this.list.Add(configureOptions);
            return this;
        }
    }
}