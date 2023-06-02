using Arcta.Lims.Machines.Protocols.Transport.Options;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Arcta.Lims.Machines.Protocols.Transport
{
    public sealed class TcpListener : StreamingTransport, ITransport, IDisposable // TODO think about moving the disposable interface to the base interface
    {
        private System.Net.Sockets.TcpClient? _tcpClient;
        private readonly System.Net.Sockets.TcpListener _tcpListener;

        public TcpListener(ILogger<TcpListener> logger, TcpOptions options) : base(logger)
        {
            IPAddress ipAddress = IPAddress.Parse(options.IPAddress);
            _tcpListener = new System.Net.Sockets.TcpListener(ipAddress, options.Port);
            _tcpListener.Start();
            _logger.LogTrace("TCP Listener started on {IPAddress}:{Port}", ipAddress, options.Port);
        }

        public new async Task StartReceivingInboundMessagesAsync(CancellationToken cancellationToken)
        {
            _tcpClient = await _tcpListener.AcceptTcpClientAsync(cancellationToken);
            Stream = _tcpClient.GetStream();
            await base.StartReceivingInboundMessagesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Stream?.Close();
            _tcpClient?.Close();
            _tcpListener?.Stop();
        }
    }
}
