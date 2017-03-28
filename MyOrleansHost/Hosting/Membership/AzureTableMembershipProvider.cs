using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Orleans.Hosting.Membership
{
    public class AzureTableMembershipProvider : IMembershipProvider
    {
        private string connectionString;

        public AzureTableMembershipProvider(IOptions<AzureTableMembershipOptions> options)
        {
            this.connectionString = options.Value.ConnectionString;
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
            return $"{this.GetType().Name}: {this.connectionString}";
        }
    }

    public class AzureTableMembershipOptions
    {
        public string ConnectionString { get; set; }
    }
}