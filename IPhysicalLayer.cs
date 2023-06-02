namespace SerialPortTest
{
    public interface IPhysicalLayer
    {
        public event EventHandler<string>? NewInboundMessageEvent;
        public Task StartReceivingInboundMessagesAsync(CancellationToken cancellationToken);
        public Task SendOutboundMessageAsync(string message, CancellationToken cancellationToken);
    }
}
