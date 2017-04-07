using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Orleans.Hosting;
using System.Net;

namespace MyOrleansHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new SiloHostBuilder()
                .UseSetting("environment", "Development")
                //.UseNetworkOptions(options => 
                //{
                //    // there will be extension methods for common scenarios such as localhost silo, azure cloud services config, etc
                //    options.Endpoint = new IPEndPoint(NetworkOptions.ResolveIPAddress(), 22222);
                //    options.ProxyGatewayEndpoint = new IPEndPoint(options.Endpoint.Address, 44444);
                //})
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }

        //public static void Main(string[] args)
        //{
        //    var clusterConfiguration = new ClusterConfiguration();
        //    // call clusterConfiguration.Load("OrleansConfiguration.xml") or configure programmatically
        //    var host = new SiloHostBuilder()
        //        .UseLegacyConfiguration(clusterConfiguration)
        //        .Build();

        //    host.Run();
        //}
    }
}
