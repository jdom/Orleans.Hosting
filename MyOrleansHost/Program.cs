using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Orleans.Hosting;

namespace MyOrleansHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new SiloHostBuilder()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
