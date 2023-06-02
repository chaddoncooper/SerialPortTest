using Microsoft.Extensions.Logging;
using SerialPortTest.Options;
using SerialPortTest.RS232;

namespace SerialPortTest;

public class ProtocolFactory : IProtocolFactory
{
    private readonly ILoggerFactory _loggerFactory;

    public ProtocolFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public IPhysicalLayer GetPhysicalLayer(ProtocolOptions options)
    {
        return options.PhysicalLayer.Type.ToUpper() switch
        {
            "TCP" => new RS232PhysicalLayer(_loggerFactory.CreateLogger<RS232PhysicalLayer>(), Microsoft.Extensions.Options.Options.Create(options.PhysicalLayer.Rs232!)),
            "RS232" => new RS232PhysicalLayer(_loggerFactory.CreateLogger<RS232PhysicalLayer>(), Microsoft.Extensions.Options.Options.Create(options.PhysicalLayer.Rs232!)),
            _ => throw new NotSupportedException($"The physical layer type: {options.PhysicalLayer.Type}, is not supported."),
        };
    }
}
