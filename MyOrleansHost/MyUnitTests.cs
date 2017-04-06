using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;
using System;
using System.Collections.Generic;

namespace MyOrleansHost
{
    // Example of how would unit testing with TestCluster look like. Basically instead of passing a ClusterConfiguration, you define the Startup class (not the host).
    // You can pass extended dynamic configuration to the silos when they are starting up, as a serialized property bag, in case something cannot be defined statically
    // on a per-silo basis and needs ti align in the entire cluster.

    public class MyUnitTests
    {
        // There could even be a pre-built base class that configures stuff that comes from TestCluster automatically
        public class MyTestStartup
        {
            public IConfigurationRoot Configuration { get; }

            public MyTestStartup(IHostingTestClusterEnvironment env)
            {
                var builder = new ConfigurationBuilder()
                    .AddInMemoryCollection(env.Configuration)
                    .AddEnvironmentVariables();
                Configuration = builder.Build();
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
            {
                app.ConfigureStorageProviders(storageBuilder =>
                {
                    storageBuilder.AddMemory("Default");
                    storageBuilder.AddAzureBlob("AzureBlobUsingDefaults");
                });
            }
        }

        public void MyTestMethod()
        {
            Guid someValueToSendToRemoteSilos = Guid.NewGuid();
            var options = new TestClusterOptions(typeof(MyTestStartup));
            options.Configuration["ServiceId"] = someValueToSendToRemoteSilos.ToString();
            var cluster = new TestCluster(options);
        }
    }

    public interface IHostingTestClusterEnvironment : IHostingEnvironment
    {
        Dictionary<string, string> Configuration { get; }
    }

    internal class TestCluster
    {
        private TestClusterOptions options;

        public TestCluster(TestClusterOptions options)
        {
            this.options = options;
        }
    }

    internal class TestClusterOptions
    {
        private Type startupType;

        public TestClusterOptions(Type startupType)
        {
            this.startupType = startupType;
        }

        public Dictionary<string, string> Configuration { get; } = new Dictionary<string, string>();
    }
}
