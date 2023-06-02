using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SerialPortTest.Extensions;
using System.IO.Ports;
using System.Text;

namespace SerialPortTest.RS232
{
    public sealed class RS232PhysicalLayer : IDisposable, IPhysicalLayer
    {
        private readonly Stream _stream;
        private readonly ILogger<RS232PhysicalLayer> _logger;
        private readonly SerialPort _serialPort;
        private readonly RS232Options _options;
        public event EventHandler<string>? NewInboundMessageEvent;

        public RS232PhysicalLayer(ILogger<RS232PhysicalLayer> logger, IOptions<RS232Options> options)
        {
            _options = options.Value;
            _logger = logger;
            _serialPort = new SerialPort(_options.PortName, _options.BaudRate,
                _options.Parity, _options.DataBits, _options.StopBits);

            _serialPort.Open();
            _stream = _serialPort.BaseStream;
            _logger.LogTrace("{ComPort} opened", _options.PortName);
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

        public void Dispose()
        {
            _serialPort.Dispose();
        }
    }
}