using Arcta.Lims.Machines.Protocols.Transport.Extensions;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Arcta.Lims.Machines.Protocols.Transport
{
    public abstract partial class StreamingTransport : ITransport
    {
        protected readonly ILogger _logger;
        protected Stream? Stream;

        public event EventHandler<string>? NewInboundMessageEvent;

        public StreamingTransport(ILogger logger)
        {
            _logger = logger;
        }

        public async Task StartReceivingInboundMessagesAsync(CancellationToken cancellationToken)
        {
            if (Stream == null)
            {
                throw new NullReferenceException("Unable to receive as stream is null");
            }
            var buffer = new byte[4096];

            _logger.LogTrace("{MethodName} started", nameof(StartReceivingInboundMessagesAsync));

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    int bytesRead = await Stream.ReadAsync(buffer, cancellationToken);

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
            if (Stream == null)
            {
                throw new NullReferenceException("Unable to send as stream is null");
            }

            try
            {
                LogOutboundMessage(outboundMessage);
                await Stream.WriteAsync(Encoding.ASCII.GetBytes(outboundMessage), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sending data: {ExceptionMessage}", ex.Message);
                throw;
            }
        }

        private void LogOutboundMessage(string inboundMessage)
        {
            _logger.LogTrace("Outbound message received:\n[ {OutboundMessage} ]", inboundMessage.WithAsciiCodes());
            _logger.LogTrace("Outbound message character stream:\n[ {OutboundMessageCharacterStream} ]", inboundMessage.AsAsciiCharacterCodes());
        }
    }
}
