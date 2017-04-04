using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;

namespace Orleans.Hosting
{
    public interface IStorageProvider : IHostedService
    {
    }

    public class AzureBlobStorageProvider : IStorageProvider
    {
        private string name;
        private string connectionString;

        public AzureBlobStorageProvider(string name, IOptions<AzureBlobStorageOptions> options, IApplicationLifetime appLifetime /* can inject services via DI */)
        {
            this.name = name;
            this.connectionString = options.Value.ConnectionString;

            // BTW, Jason is working to create a fine-grained application lifecycle abstraction. This is just the one from AspNetCore right now.
        }

        public Task Start()
        {
            return Task.FromResult(true);
        }

        public Task Stop()
        {
            return Task.FromResult(true);
        }

        public override string ToString()
        {
            return $"{this.GetType().Name}: {this.name} - {this.connectionString}";
        }
    }

    public class AzureBlobStorageOptions : IConfigureSerializer
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public IExternalSerializer Serializer { get; set; }
    }

#region alternative (deprecated)
    public class AzureBlobStorageProviderAlternative : IStorageProvider
    {
        private string name;
        private string connectionString;

        public AzureBlobStorageProviderAlternative(string name, AzureBlobStorageOptions options, IApplicationLifetime appLifetime /* can inject services via DI */)
        {
            this.name = name;
            this.connectionString = options.ConnectionString;

            // BTW, Jason is working to create a fine-grained application lifecycle abstraction. This is just the one from AspNetCore right now.
        }

        public Task Start()
        {
            return Task.FromResult(true);
        }

        public Task Stop()
        {
            return Task.FromResult(true);
        }

        public override string ToString()
        {
            return $"{this.GetType().Name}: {this.name} - {this.connectionString}";
        }
    }
#endregion alternative (deprecated)
}