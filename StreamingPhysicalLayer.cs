using Microsoft.Extensions.Logging;
using SerialPortTest.Extensions;
using System.Text;

namespace SerialPortTest
{
    internal partial class StreamingPhysicalLayer : IPhysicalLayer
    {
        private readonly ILogger _logger;
        private readonly Stream _stream;

        public event EventHandler<string>? NewInboundMessageEvent;

        public StreamingPhysicalLayer(ILogger logger, Stream stream)
        {
            _logger = logger;
            _stream = stream;
        }
        public async Task StartReceivingInboundMessagesAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];

            _logger.LogTrace("{MethodName} started", nameof(StartReceivingInboundMessagesAsync));

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, cancellationToken);

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

        public async Task SendOutboundMessageAsync(string outboundMessage, CancellationToken cancellationToken)
        {
            try
            {
                LogOutboundMessage(outboundMessage);
                await _stream.WriteAsync(Encoding.ASCII.GetBytes(outboundMessage), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogTrace("{MethodName} cancelled", nameof(SendOutboundMessageAsync));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sending data: {ExceptionMessage}", ex.Message);
            }
        }

        private void LogOutboundMessage(string inboundMessage)
        {
            _logger.LogTrace("Outbound message received:\n[ {OutboundMessage} ]", inboundMessage.WithAsciiCodes());
            _logger.LogTrace("Outbound message character stream:\n[ {OutboundMessageCharacterStream} ]", inboundMessage.AsAsciiCharacterCodes());
        }
    }
}
