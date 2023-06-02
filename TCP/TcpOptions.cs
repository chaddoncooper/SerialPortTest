namespace SerialPortTest.TCP;
public class TcpOptions
{
    public const string Tcp = "Tcp";
    public required string IPAddress { get; set; }
    public required int Port { get; set; }
}
