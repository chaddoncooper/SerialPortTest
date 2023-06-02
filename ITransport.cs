namespace Arcta.Lims.Machines.Protocols.Transport
{
    public interface ITransport
    {
        public event EventHandler<string>? NewInboundMessageEvent;
        public Task StartReceivingInboundMessagesAsync(CancellationToken cancellationToken);
        public Task SendOutboundMessageAsync(string message, CancellationToken cancellationToken);
    }
}
