using System.IO.Ports;

namespace SerialPortTest.RS232;

public class RS232Options
{
    public const string Rs232 = "Rs232";
    public required string PortName { get; set; }
    public int BaudRate { get; set; }
    public Parity Parity { get; set; }
    public int DataBits { get; set; }
    public StopBits StopBits { get; set; }
}
