﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;
using Orleans.Hosting;
using Orleans.Hosting.Membership;
using System.Net;
using System;
using MyOrleansApp;

namespace MyOrleansHost
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // optionally set up a configuration that can bind values from several sources. Supports a ton of different configuration providers
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container or configure Options.
        public void ConfigureServices(IServiceCollection services)
        {
            // NetworkOptions can either be configured here, or in the Main method by using UseNetworkOptions. Here it's useful if it comes from declarative config
            services.Configure<NetworkOptions>(options =>
            {
                options.Endpoint = new IPEndPoint(NetworkOptions.ResolveIPAddress(), 22223);
                options.ProxyGatewayEndpoint = new IPEndPoint(options.Endpoint.Address, 44445);
                options.OpenConnectionTimeout = TimeSpan.FromSeconds(10);
            });

            services.Configure<MessagingOptions>(options =>
            {
                options.ResponseTimeout = TimeSpan.FromSeconds(45);
                options.ResendOnTimeout = true;
            });

            services.Configure<MembershipOracleOptions>(options =>
            {
                options.MaxJoinAttemptTime = TimeSpan.FromMinutes(3);
                options.ProbeTimeout = TimeSpan.FromSeconds(15);
                options.NumMissedProbesLimit = 3;
            });

            services.Configure<ActivationCollectionOptions>(options =>
            {
                options.DefaultCollectionAgeLimit = TimeSpan.FromMinutes(30);
                options.AddOverride<PlayerGrain>(TimeSpan.FromHours(3));
            });

            // Optionally specify default options so that individual providers can just fall back to using these values.
            // For example, if DataConnectionString is defined in those options, they will be automatically used by
            // the membership provider, storage providers, etc (unless explicitly overwritten when configuring them).
            services.ConfigureOrleansDefaultOptions(Configuration.GetSection("DefaultOptions"));

            // can use default options (similar to how it works in the published release with SystemStore).
            services.AddAzureTableMembership();

            // These are alternate overloads so that end users can integrate more complex configuration scenarios:
            // services.AddAzureTableMembership(new AzureTableMembershipOptions { ConnectionString = "set directly without overrides" });
            // services.AddAzureTableMembership(Configuration.GetConnectionString("AzureStorage"));
            // services.AddAzureTableMembership(Configuration.GetSection("DefaultOptions"));
            // services.AddAzureTableMembership(options => options.Configure(x => x.ConnectionString = "xxx"));

            services.AddStorageProviders(); // this is opt-in!
            services.AddStreamProviders();
        }

        // optionally will try to match to the most specific environment if Configure{Environment]Services is defined
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            ConfigureServices(services);
            services.AddTestHooks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

#region Serilog configuration
            // Example hooking up Serilog
            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
              .Enrich.FromLogContext()
              .WriteTo.LiterateConsole()
              .CreateLogger();
            loggerFactory.AddSerilog();
            //appLifetime.ApplicationStopped.Register(Serilog.Log.CloseAndFlush);
#endregion Serilog configuration

            if (env.IsDevelopment())
            {
                // app.CrashOnUnobservedExceptions(true);
            }

            app.ConfigureStorageProviders(storageBuilder =>
            {
                storageBuilder.AddMemory("Default");

                // can use default options (similar to how it works in the published release with SystemStore).
                storageBuilder.AddAzureBlob("AzureBlobUsingDefaults");

                storageBuilder.AddAzureBlob("AzureBlobOverriding", optionsBuilder =>
                    optionsBuilder
                        .Configure(Configuration.GetSection("StorageProviders:AzureBlob1"))
                        .Configure(options => options.ContainerName = "overriden AzureBlob2")
                        .UseJson());

                storageBuilder.AddAzureBlob("AzureBlobUsingExternal", Configuration.GetSection("StorageProviders:AzureBlob1"));

                // Not sure about allowing pre-materialized options
                storageBuilder.AddAzureBlob("AzureBlobMaterializedOptions",
                    new AzureBlobStorageOptions { ConnectionString = "set directly when configuring, without fallbacks" });

                // Alternative way of configuring (not overloads) that I kept here for opinions but we shouldn't support all
                storageBuilder.AddAzureBlobAlternative("AzureBlobAlternative1", Configuration.GetSection("StorageProviders:AzureBlob1"));
                storageBuilder.AddAzureBlobAlternative("AzureBlobAlternative2", new AzureBlobStorageOptions { ConnectionString = Configuration.GetConnectionString("AzureStorage"), ContainerName = "myapp" });

                // Alternative way of configuring (not overloads) that I kept here for opinions but we shouldn't support all
                storageBuilder.AddAzureBlobFluent("AzureBlobUsingFluentConfig")
                    .Configure(Configuration.GetSection("StorageProviders:AzureBlob1"))
                    .Configure(options => options.ContainerName = "overriden");
            });

            app.ConfigureStreamProviders(streamBuilder =>
            {
                streamBuilder.AddEventHub("EventHub", optionsBuilder =>
                    optionsBuilder.Configure(options =>
                        {
                            options.CacheSizeMb = 100;
                            options.CheckpointerOptions.ConnectionString = Configuration.GetConnectionString("EventHub");
                            options.CheckpointerOptions.TableName = "mycheckpoints";
                        }));

                streamBuilder.AddEventHub("EventHubConfig1", optionsBuilder => optionsBuilder.Configure(Configuration.GetSection("StreamProviders:EventHubConfig1")));

                //streamBuilder.AddEventHub("EventHub", new EventHubStreamOptions
                //{
                //    CacheSizeMb = 100,
                //    CheckpointerOptions = new CheckpointerOptions
                //    {
                //        ConnectionString = Configuration.GetConnectionString("EventHub"),
                //        TableName = "mycheckpoints"
                //    }
                //});

                // storageBuilder.AddSms("Sms");
            });
        }
    }
}
