using Arcta.Lims.Machines.Protocols.Transport.Options;
using Microsoft.Extensions.Logging;

namespace Arcta.Lims.Machines.Protocols.Transport
{
    public sealed class TcpClient : StreamingTransport, ITransport
    {
        public TcpClient(ILogger<TcpClient> logger, TcpOptions options) : base(logger)
        {
            var tcpClient = new System.Net.Sockets.TcpClient(options.IPAddress, options.Port);
            Stream = tcpClient.GetStream();
        }

        // TODO:
        // use client as the method for receive and sending
    }
}
