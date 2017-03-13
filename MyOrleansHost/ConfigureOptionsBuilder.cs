using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Orleans.Hosting
{

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
