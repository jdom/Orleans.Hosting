using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;

namespace Orleans.Hosting
{
    public interface IStreamProvider : IHostedService
    {
    }

    public class EventHubStreamProvider : IStreamProvider
    {
        private EventHubStreamOptions options;
        private string name;

        public EventHubStreamProvider(string name, EventHubStreamOptions options, IApplicationLifetime appLifetime /* can inject services via DI */)
        {
            // BTW, Jason is working to create a fine-grained application lifecycle abstraction. This is just the one from AspNetCore right now.
            //appLifetime.ApplicationStopped.Register(this.Stop);

            this.name = name;
            this.options = options;
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
            return $"{this.GetType().Name}: {this.name} - {this.options.CheckpointerOptions.DataConnectionString}";
        }
    }

    public class EventHubStreamOptions
    {
        /// <summary>
        /// Checkpoint settings type.  Type must conform to ICheckpointerSettings interface.
        /// </summary>
        public CheckpointerOptions CheckpointerOptions { get; set; } = new CheckpointerOptions();

        /// <summary>
        /// Cache size in megabytes.
        /// </summary>
        public int CacheSizeMb { get; set; }

        /// <summary>
        /// Minimum time message will stay in cache before it is available for time based purge.
        /// </summary>
        public TimeSpan DataMinTimeInCache { get; set; }
    }

    public class CheckpointerOptions
    {
        /// <summary>
        /// Azure table storage data connections string
        /// </summary>
        public string DataConnectionString { get; set; }

        /// <summary>
        /// Azure storage table name where the checkpoints will be stored
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// How often to persist the checkpoints, if they've changed.
        /// </summary>
        public TimeSpan PersistInterval { get; set; }

        /// <summary>
        /// This name partitions a service's checkpoint information from other services.
        /// </summary>
        public string CheckpointNamespace { get; set; }
    }
}