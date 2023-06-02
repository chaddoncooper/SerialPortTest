using Arcta.Lims.Machines.Protocols.Transport;
using Arcta.Lims.Machines.Protocols.Transport.Extensions;
using Arcta.Lims.Machines.Protocols.Transport.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO.Ports;

using IHost host = Host.CreateDefaultBuilder(args)
    .RegisterServices()
    .Build();

foreach (var portName in SerialPort.GetPortNames())
{
    Console.WriteLine(portName);
}

var configuration = host.Services.GetRequiredService<IConfiguration>();
var machinesOptions = new List<MachineOptions>(); // This will go into IOptions on the machine factory
configuration.Bind("Machines", machinesOptions);

var protocolFactory = host.Services.GetRequiredService<ITransportFactory>();
var rs232 = protocolFactory.GetTransport(machinesOptions.First().Protocol);

rs232.NewInboundMessageEvent += YouHazMail;
var cancellationTokenSource = new CancellationTokenSource();
Task rs232ReceiveTask = Task.Run(() => rs232.StartReceivingInboundMessagesAsync(cancellationTokenSource.Token));
await rs232.SendOutboundMessageAsync("Hello from console", cancellationTokenSource.Token);

var tcp = protocolFactory.GetTransport(machinesOptions.ElementAt(1).Protocol);
Task tcpTask = Task.Run(() => tcp.StartReceivingInboundMessagesAsync(cancellationTokenSource.Token));
await tcp.SendOutboundMessageAsync("hello", cancellationTokenSource.Token);

Console.ReadLine();

static void YouHazMail(object? sender, string e)
{

}

