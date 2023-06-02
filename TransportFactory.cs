using Arcta.Lims.Machines.Protocols.Transport.Options;
using Microsoft.Extensions.Logging;

namespace Arcta.Lims.Machines.Protocols.Transport;

public class TransportFactory : ITransportFactory
{
    private readonly ILoggerFactory _loggerFactory;

    public TransportFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public ITransport GetTransport(ProtocolOptions options)
    {
        return options.Transport.Type.ToUpper() switch
        {
            "TCP" => options.Transport.Tcp!.TcpMode == TcpMode.Listener ?
                new TcpListener(_loggerFactory.CreateLogger<TcpListener>(), options.Transport.Tcp!)
                : new TcpClient(_loggerFactory.CreateLogger<TcpClient>(), options.Transport.Tcp!),
            "RS232" => new RS232(_loggerFactory.CreateLogger<RS232>(), options.Transport.Rs232!),
            _ => throw new NotSupportedException($"The physical layer type: {options.Transport.Type}, is not supported."),
        };
    }
}
