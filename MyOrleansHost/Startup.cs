﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;
using Orleans.Hosting;

namespace MyOrleansHost
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureOrleansDefaultOptions(Configuration.GetSection("DefaultOptions"));

            // can use default options (similar to how it works in the published release with SystemStore).
            services.AddAzureTableMembership();

            // these are other alternate overloads so that end users can integrate more complex configuration scenarios
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
                storageBuilder.AddAzureBlob("AzureBlob1", Configuration.GetSection("StorageProviders:AzureBlob1"));
                storageBuilder.AddAzureBlob("AzureBlob2", new AzureBlobStorageOptions { ConnectionString = Configuration.GetConnectionString("AzureStorage"), ContainerName = "myapp", IndentJson = true });

                // alt version
                storageBuilder.AddAzureBlob2("AzureBlob3")
                    .Configure(Configuration.GetSection("StorageProviders:AzureBlob1"))
                    .Configure(options => options.ContainerName = "overriden");

                // alt version
                storageBuilder.AddAzureBlob3("AzureBlob4", optionsBuilder =>
                    optionsBuilder.Configure(Configuration.GetSection("StorageProviders:AzureBlob1"))
                    .Configure(options => options.ContainerName = "overriden AzureBlob4"));

                // can use default options (similar to how it works in the published release with SystemStore).
                storageBuilder.AddAzureBlob3("AzureBlob5");
            });

            app.ConfigureStreamProviders(storageBuilder =>
            {
                //storageBuilder.AddSms("Sms", new SimpleMessageOptions { FireAndForget = true });
                storageBuilder.AddEventHub("EventHub", new EventHubStreamOptions
                {
                    CacheSizeMb = 100,
                    CheckpointerOptions = new CheckpointerOptions
                    {
                        DataConnectionString = Configuration.GetConnectionString("EventHub"),
                        TableName = "mycheckpoints"
                    }
                });
            });
        }
    }
}
