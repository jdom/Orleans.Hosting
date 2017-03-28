using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Orleans.Hosting.Membership
{
    public class AzureTableMembershipProvider : IMembershipProvider
    {
        public AzureTableMembershipProvider(IOptions<AzureTableMembershipOptions> options)
        {
            Console.WriteLine($"{this.GetType().Name}: {options.Value.ConnectionString}");
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

    public class AzureTableMembershipOptions
    {
        public string ConnectionString { get; set; }
    }
}