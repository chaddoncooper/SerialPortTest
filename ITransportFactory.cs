using Arcta.Lims.Machines.Protocols.Transport.Options;

namespace Arcta.Lims.Machines.Protocols.Transport
{
    public interface ITransportFactory
    {
        ITransport GetTransport(ProtocolOptions options);
    }
}