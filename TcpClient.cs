using Arcta.Lims.Machines.Protocols.Transport.Options;
using Microsoft.Extensions.Logging;

namespace Arcta.Lims.Machines.Protocols.Transport
{
    public sealed class TcpClient : StreamingTransport, ITransport
    {
        private readonly System.Net.Sockets.TcpClient _tcpClient;
        private readonly TcpOptions _options;

        public TcpClient(ILogger<TcpClient> logger, TcpOptions options) : base(logger)
        {
            _tcpClient = new System.Net.Sockets.TcpClient();
            _options = options;
            _tcpClient.Connect(_options.IPAddress, _options.Port);
            _logger.LogTrace("Connected to host {IPAddress}:{Port}", _options.IPAddress, _options.Port);
            Stream = _tcpClient.GetStream();
        }

        public void Dispose()
        {
            Stream?.Close();
            _tcpClient?.Close();
        }
    }
}
