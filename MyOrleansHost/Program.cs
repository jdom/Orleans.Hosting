using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Orleans.Hosting;

namespace MyOrleansHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // in the future we might configure the TCP server directly here, but since it's not properly abstracted away,
            // I'm not even attempting it
            var host = new SiloHostBuilder()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }

        //public static void Main(string[] args)
        //{
        //    var host = new SiloHostBuilder()
        //        .UseLegacyConfiguration(new ClusterConfiguration())
        //        .Build();

        //    host.Run();
        //}
    }
}
