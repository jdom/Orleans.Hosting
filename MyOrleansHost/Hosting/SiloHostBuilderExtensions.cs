using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Orleans.Hosting
{
    public static class SiloHostBuilderExtensions
    {
        public static IWebHostBuilder UseNetworkOptions(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureServices(services =>
            {
                AddOrleansSocketServer(services);
            });

            return webHostBuilder;
        }

        public static IWebHostBuilder UseNetworkOptions(this IWebHostBuilder webHostBuilder, Action<NetworkOptions> configureOptions)
        {
            webHostBuilder.UseNetworkOptions().ConfigureServices(services =>
            {
                services.Configure<NetworkOptions>(configureOptions);
            });

            return webHostBuilder;
        }

        internal static void AddOrleansSocketServer(IServiceCollection services)
        {
            services.AddSingleton<IServer, OrleansSocketServer>();
        }
    }
}