using Arcta.Lims.Machines.Protocols.Transport;
using Arcta.Lims.Machines.Protocols.Transport.Extensions;
using Arcta.Lims.Machines.Protocols.Transport.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .RegisterServices()
    .Build();

//foreach (var portName in SerialPort.GetPortNames())
//{
//    Console.WriteLine(portName);
//}

var configuration = host.Services.GetRequiredService<IConfiguration>();
var machinesOptions = new List<MachineOptions>(); // This will go into IOptions on the machine factory
configuration.Bind("Machines", machinesOptions);

var protocolFactory = host.Services.GetRequiredService<ITransportFactory>();
var cancellationTokenSource = new CancellationTokenSource();

//var rs232 = protocolFactory.GetTransport(machinesOptions.First().Protocol);
//rs232.NewInboundMessageEvent += YouHazMail;
//Task rs232ReceiveTask = Task.Run(() => rs232.StartReceivingInboundMessagesAsync(cancellationTokenSource.Token));
//await rs232.SendOutboundMessageAsync("Hello from console", cancellationTokenSource.Token);

var tcpListener = protocolFactory.GetTransport(machinesOptions.ElementAt(1).Protocol);
Task tcpListenerRecieveTask = Task.Run(() => tcpListener.StartReceivingInboundMessagesAsync(cancellationTokenSource.Token));
var tcpClient = protocolFactory.GetTransport(machinesOptions.ElementAt(2).Protocol);
Task tcpClientRecieveTask = Task.Run(() => tcpClient.StartReceivingInboundMessagesAsync(cancellationTokenSource.Token));
await tcpClient.SendOutboundMessageAsync("this is message 1", cancellationTokenSource.Token);
await tcpClient.SendOutboundMessageAsync("this is message 2", cancellationTokenSource.Token);
await tcpClient.SendOutboundMessageAsync("this is message 3", cancellationTokenSource.Token);
await tcpClient.SendOutboundMessageAsync("this is message 4", cancellationTokenSource.Token);
await tcpClient.SendOutboundMessageAsync("this is message 5", cancellationTokenSource.Token);


Console.ReadLine();

static void YouHazMail(object? sender, string e)
{
    Console.WriteLine(e);
}

