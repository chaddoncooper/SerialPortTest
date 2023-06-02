using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO.Ports;
using System.Text;

namespace SerialPortTest
{
    public sealed class RS232InstrumentPhysicalLayer : IDisposable, IProtocolLayer
    {
        private readonly SerialPort _serialPort;
        private readonly ILogger<RS232InstrumentPhysicalLayer> _logger;
        private readonly RS232Options _options;
        public event EventHandler<string>? NewInboundMessageEvent;

        public RS232InstrumentPhysicalLayer(ILogger<RS232InstrumentPhysicalLayer> logger, IOptions<RS232Options> options)
        {
            _options = options.Value;
            _logger = logger;
            _serialPort = new SerialPort(_options.PortName, _options.BaudRate,
                _options.Parity, _options.DataBits, _options.StopBits);

            _serialPort.Open();
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
                    int bytesRead = await _serialPort.BaseStream.ReadAsync(buffer, cancellationToken);

                    if (bytesRead > 0)
                    {
                        var receivedData = new byte[bytesRead];
                        Array.Copy(buffer, 0, receivedData, 0, bytesRead);

                        OnDataReceived(receivedData);
                    }
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

        private void OnDataReceived(byte[] data)
        {
            var message = Encoding.ASCII.GetString(data);

            LogInboundMessage(message);

            NewInboundMessageEvent?.Invoke(this, message);
        }

        private void LogInboundMessage(string inboundMessage)
        {
            LogInboundMessageAsString(inboundMessage);
            LogInboundMessageAsCharacterStream(inboundMessage);
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

        private void LogInboundMessageAsCharacterStream(string message)
        {
            _logger.LogTrace("Inbound message character stream:\n[ {InboundMessageCharacterStream} ]",
                string.Join(" ", message.Select(ch => (int)ch)));
        }

        public async Task SendOutboundMessageAsync(string outboundMessage, CancellationToken cancellationToken)
        {
            try
            {
                LogOutboundMessage(outboundMessage);
                await _serialPort.BaseStream.WriteAsync(Encoding.ASCII.GetBytes(outboundMessage), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogTrace("{MethodName} cancelled", nameof(SendOutboundMessageAsync));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error reading data: {ExceptionMessage}", ex.Message);
            }
        }

        private void LogOutboundMessage(string inboundMessage)
        {
            LogOutboundMessageAsString(inboundMessage);
            LogOutboundMessageAsCharacterStream(inboundMessage);
        }

        private void LogOutboundMessageAsString(string outboundMessage)
        {
            var ch = outboundMessage.ToCharArray();

            var logMessage = "";
            for (int i = 0; i < ch.Length; i++)
            {
                char c = ch[i];

                logMessage += c.AsciiCode();

                logMessage += (i != ch.Length - 1) && c.IsAsciiControlOrSpaceChar() ? ", " : "";
            }

            _logger.LogTrace("Outbound message received:\n[ {OutboundMessage} ]", logMessage);
        }

        private void LogOutboundMessageAsCharacterStream(string message)
        {
            _logger.LogTrace("Outbound message character stream:\n[ {OutboundMessageCharacterStream} ]",
                string.Join(" ", message.Select(ch => (int)ch)));
        }

        public void Dispose()
        {
            _serialPort.Dispose();
        }
    }
}