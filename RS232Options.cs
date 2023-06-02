using System.IO.Ports;

namespace SerialPortTest;

public class RS232Options
{
    public const string RS232 = "RS232";
    public required string PortName { get; set; }
    public int BaudRate { get; set; }
    public Parity Parity { get; set; }
    public int DataBits { get; set; }
    public StopBits StopBits { get; set; }
}


