using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Orleans.Hosting
{
    public interface IStorageProvider : IHostedService
    {
    }

    public class AzureBlobStorageProvider : IStorageProvider
    {
        public AzureBlobStorageProvider(string name, AzureBlobStorageOptions options, IApplicationLifetime appLifetime /* can inject services via DI */)
        {
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
    }

    public class AzureBlobStorageOptions
    {
        public string ConnectionString { get; set; }
        public bool IndentJson { get; set; }
        public string ContainerName { get; set; }
    }
}