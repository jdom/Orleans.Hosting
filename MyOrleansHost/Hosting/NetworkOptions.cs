using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;

namespace Orleans.Hosting
{
    public class NetworkOptions
    {
        /// <summary>
        /// The IPEndPoint this silo uses for silo-to-silo communication.
        /// </summary>
        public IPEndPoint Endpoint { get; set; }

        /// <summary>
        /// The IPEndPoint this silo uses for (gateway) silo-to-client communication.
        /// </summary>
        public IPEndPoint ProxyGatewayEndpoint { get; set; }

        public static IPAddress ResolveIPAddress(string addrOrHost = null, byte[] subnet = null, AddressFamily family = AddressFamily.InterNetwork)
        {
            // copy implementation from Orleans
            return new IPAddress(new byte[] { 127, 0, 0, 1 });
        }
    }

    // Details of how this IServer implementation will work need to be ironed out. An IServer implementation that is closest to
    // our needs is in https://github.com/aspnet/HttpSysServer
    public class OrleansSocketServer : IServer
    {
        private NetworkOptions options;

        public OrleansSocketServer(IOptions<NetworkOptions> options)
        {
            this.options = options.Value;
        }
        public IFeatureCollection Features { get; } = new FeatureCollection();

        public void Dispose()
        {
        }

        public void Start<TContext>(IHttpApplication<TContext> application)
        {
        }
    }
}