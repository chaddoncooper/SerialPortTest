using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SerialPortTest.Extensions;
using System.Net.Sockets;
using System.Text;

namespace SerialPortTest.TCP
{
    public class TcpClientPhysicalLayer : IPhysicalLayer
    {
        private readonly ILogger<TcpClientPhysicalLayer> _logger;
        private readonly TcpOptions _options;
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _networkStream;

        public event EventHandler<string>? NewInboundMessageEvent;

        public TcpClientPhysicalLayer(ILogger<TcpClientPhysicalLayer> logger, IOptions<TcpOptions> options)
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

                    OnDataReceived(Encoding.ASCII.GetString(buffer, 0, bytesRead));
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

        private void OnDataReceived(string data)
        {
            LogInboundMessage(data);

            NewInboundMessageEvent?.Invoke(this, data);
        }

        private void LogInboundMessage(string inboundMessage)
        {
            LogInboundMessageAsString(inboundMessage);
            _logger.LogTrace("Inbound message character stream:\n[ {InboundMessageCharacterStream} ]", inboundMessage.AsAsciiCharacterCodes());
        }

        private void LogInboundMessageAsString(string inboundMessage)
        {
            var logMessage = inboundMessage[0] switch
            {
                (char)10 => "<LF>",
                (char)13 => "<CR>",
                _ => inboundMessage,
            };
            _logger.LogTrace("Inbound message received:\n[ {InboundMessage} ]", logMessage);
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
