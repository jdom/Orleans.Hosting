// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Builder;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using System.Linq;
using System.Threading.Tasks;

namespace Orleans.Hosting.Internal
{
    public class SiloHost : IWebHost
    {
        private readonly IServiceCollection _applicationServiceCollection;
        private IStartup _startup;
        private ApplicationLifetime _applicationLifetime;
        // private HostedServiceExecutor _hostedServiceExecutor;

        private readonly IServiceProvider _hostingServiceProvider;
        private readonly WebHostOptions _options;
        private readonly IConfiguration _config;

        private IServiceProvider _applicationServices;
        private Silo _application;
        private ILogger<SiloHost> _logger;

        // Used for testing only
        internal WebHostOptions Options => _options;

        public SiloHost(
            IServiceCollection appServices,
            IServiceProvider hostingServiceProvider,
            WebHostOptions options,
            IConfiguration config)
        {
            if (appServices == null)
            {
                throw new ArgumentNullException(nameof(appServices));
            }

            if (hostingServiceProvider == null)
            {
                throw new ArgumentNullException(nameof(hostingServiceProvider));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _config = config;
            _options = options;
            _applicationServiceCollection = appServices;
            _hostingServiceProvider = hostingServiceProvider;
            _applicationServiceCollection.AddSingleton<IApplicationLifetime, ApplicationLifetime>();
            // _applicationServiceCollection.AddSingleton<HostedServiceExecutor>();
        }

        public IServiceProvider Services
        {
            get
            {
                EnsureApplicationServices();
                return _applicationServices;
            }
        }

        public IFeatureCollection ServerFeatures { get; } = new FeatureCollection();

        public void Initialize()
        {
            if (_application == null)
            {
                _application = BuildApplication();
            }
        }

        public virtual void Start()
        {
            _logger = _applicationServices.GetRequiredService<ILogger<SiloHost>>();

            Initialize();

            _applicationLifetime = _applicationServices.GetRequiredService<IApplicationLifetime>() as ApplicationLifetime;
            //_hostedServiceExecutor = _applicationServices.GetRequiredService<HostedServiceExecutor>();
            //var diagnosticSource = _applicationServices.GetRequiredService<DiagnosticSource>();
            //var httpContextFactory = _applicationServices.GetRequiredService<IHttpContextFactory>();
            //Server.Start(new HostingApplication(_application, _logger, diagnosticSource, httpContextFactory));

            // Fire IApplicationLifetime.Started
            _applicationLifetime?.NotifyStarted();

            // Fire IHostedService.Start
            //_hostedServiceExecutor.Start();

            _logger.Started();
        }

        private void EnsureApplicationServices()
        {
            if (_applicationServices == null)
            {
                EnsureStartup();
                _applicationServices = _startup.ConfigureServices(_applicationServiceCollection);
            }
        }

        private void EnsureStartup()
        {
            if (_startup != null)
            {
                return;
            }

            _startup = _hostingServiceProvider.GetRequiredService<IStartup>();
        }

        private Silo BuildApplication()
        {
            EnsureApplicationServices();
            var builderFactory = _applicationServices.GetRequiredService<IApplicationBuilderFactory>();
            var builder = builderFactory.CreateBuilder(ServerFeatures);
            builder.ApplicationServices = _applicationServices;

            var startupFilters = _applicationServices.GetService<IEnumerable<IStartupFilter>>();
            Action<IApplicationBuilder> configure = _startup.Configure;
            foreach (var filter in startupFilters.Reverse())
            {
                configure = filter.Configure(configure);
            }

            configure(builder);

            // TODO better lifecycle
            var hostedServices = builder.ApplicationServices.GetServices<IHostedService>();
            Task.WaitAll(hostedServices.Select(x => x.Start()).ToArray());

            // TODO
            return new Silo();
        }



        public void Dispose()
        {
            _logger?.Shutdown();

            // Fire IApplicationLifetime.Stopping
            _applicationLifetime?.StopApplication();

            // Fire the IHostedService.Stop
            //_hostedServiceExecutor?.Stop();

            (_hostingServiceProvider as IDisposable)?.Dispose();
            (_applicationServices as IDisposable)?.Dispose();

            // Fire IApplicationLifetime.Stopped
            _applicationLifetime?.NotifyStopped();

            // HostingEventSource.Log.HostStop();
        }
    }

    internal class Silo
    {
    }
}
