using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Text;

namespace SerialPortTest.TCP
{
    public class TcpPhysicalLayer : IPhysicalLayer
    {
        private readonly ILogger<TcpPhysicalLayer> _logger;
        private readonly TcpOptions _options;
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _networkStream;

        public event EventHandler<string>? NewInboundMessageEvent;

        public TcpPhysicalLayer(ILogger<TcpPhysicalLayer> logger, IOptions<TcpOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _tcpClient = new TcpClient(_options.IPAddress, _options.Port);
            _networkStream = _tcpClient.GetStream();
        }
        public async Task StartReceivingInboundMessagesAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];

            _logger.LogTrace("{MethodName} started", nameof(StartReceivingInboundMessagesAsync));

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    int bytesRead = await _networkStream.ReadAsync(buffer, cancellationToken);

                    if (bytesRead == 0)
                    {
                        break;
                    }

                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received data: " + receivedData);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogTrace("{MethodName} stopped", nameof(StartReceivingInboundMessagesAsync));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error reading data: {ExceptionMessage}", ex.Message);
            }
        }

        public async Task SendOutboundMessageAsync(string message, CancellationToken cancellationToken)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);

            try
            {
                await _networkStream.WriteAsync(data, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sending data: {ExceptionMessage}", ex.Message);
            }
        }
    }
}
