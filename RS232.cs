using Arcta.Lims.Machines.Protocols.Transport.Options;
using Microsoft.Extensions.Logging;
using System.IO.Ports;

namespace Arcta.Lims.Machines.Protocols.Transport
{
    public sealed class RS232 : StreamingTransport, IDisposable, ITransport
    {
        private readonly SerialPort _serialPort;

        public RS232(ILogger<RS232> logger, RS232Options options) : base(logger)
        {
            _serialPort = new SerialPort(options.PortName, options.BaudRate,
                options.Parity, options.DataBits, options.StopBits);

            _serialPort.Open();
            Stream = _serialPort.BaseStream;
            _logger.LogTrace("Serial port {ComPort} opened", _serialPort.PortName);
        }

        public void Dispose()
        {
            Stream?.Dispose();
            _serialPort.Dispose();
            _logger.LogTrace("Serial port {ComPort} closed", _serialPort.PortName);
        }
    }
}
