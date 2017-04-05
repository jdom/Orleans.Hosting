using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using System;
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


        //The following options are taken from MessagingConfiguration, as they are network related


        /// <summary>
        /// The OpenConnectionTimeout attribute specifies the timeout before a connection open is assumed to have failed
        /// </summary>
        public TimeSpan OpenConnectionTimeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The MaxSocketAge attribute specifies how long to keep an open socket before it is closed.
        /// Default is TimeSpan.MaxValue (never close sockets automatically, unless they were broken).
        /// </summary>
        public TimeSpan MaxSocketAge { get; set; } = TimeSpan.MaxValue;

        /// <summary>
        /// The SiloSenderQueues attribute specifies the number of parallel queues and attendant threads used by the silo to send outbound
        /// messages (requests, responses, and notifications) to other silos.
        /// If this attribute is not specified, then System.Environment.ProcessorCount is used.
        /// </summary>
        public int SiloSenderQueues { get; set; } = Environment.ProcessorCount;
        /// <summary>
        /// The GatewaySenderQueues attribute specifies the number of parallel queues and attendant threads used by the silo Gateway to send outbound
        ///  messages (requests, responses, and notifications) to clients that are connected to it.
        ///  If this attribute is not specified, then System.Environment.ProcessorCount is used.
        /// </summary>
        public int GatewaySenderQueues { get; set; } = Environment.ProcessorCount;
        /// <summary>
        ///  The ClientSenderBuckets attribute specifies the total number of grain buckets used by the client in client-to-gateway communication
        ///  protocol. In this protocol, grains are mapped to buckets and buckets are mapped to gateway connections, in order to enable stickiness
        ///  of grain to gateway (messages to the same grain go to the same gateway, while evenly spreading grains across gateways).
        ///  This number should be about 10 to 100 times larger than the expected number of gateway connections.
        ///  If this attribute is not specified, then Math.Pow(2, 13) is used.
        /// </summary>
        public int ClientSenderBuckets { get; set; } = (int)Math.Pow(2, 13);
        /// <summary>
        ///  This is the period of time a gateway will wait before dropping a disconnected client.
        /// </summary>
        public TimeSpan ClientDropTimeout { get; set; }

        /// <summary>
        /// The size of a buffer in the messaging buffer pool.
        /// </summary>
        public int BufferPoolBufferSize { get; set; }
        /// <summary>
        /// The maximum size of the messaging buffer pool.
        /// </summary>
        public int BufferPoolMaxSize { get; set; }
        /// <summary>
        /// The initial size of the messaging buffer pool that is pre-allocated.
        /// </summary>
        public int BufferPoolPreallocationSize { get; set; }

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