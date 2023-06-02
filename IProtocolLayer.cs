﻿namespace SerialPortTest
{
    internal interface IProtocolLayer
    {
        public event EventHandler<string>? NewInboundMessageEvent;
        public Task StartReceivingInboundMessagesAsync(CancellationToken cancellationToken);
        public Task SendOutboundMessageAsync(string message, CancellationToken cancellationToken);
    }
}