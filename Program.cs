// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SerialPortTest;
using System.IO.Ports;

using IHost host = Host.CreateDefaultBuilder(args)
    .RegisterServices()
    .Build();

foreach (var portName in SerialPort.GetPortNames())
{
    Console.WriteLine(portName);
}


var rs232 = host.Services.GetRequiredService<RS232InstrumentPhysicalLayer>();
rs232.NewInboundMessageEvent += Rs232Receiver_DataReceived;
var cancellationTokenSource = new CancellationTokenSource();
Task task = Task.Run(() => rs232.StartReceivingInboundMessagesAsync(cancellationTokenSource.Token));
await rs232.SendOutboundMessageAsync("Hello from console", cancellationTokenSource.Token);
Console.ReadLine();

static void Rs232Receiver_DataReceived(object? sender, string e)
{

}

