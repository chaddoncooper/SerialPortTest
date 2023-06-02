namespace SerialPortTest.TCP;
public class TcpOptions
{
    public const string Tcp = "Tcp";
    public required string IPAddress { get; set; }
    public required int Port { get; set; }
    public TcpMode TcpMode { get; set; } = TcpMode.Server;
}
public enum TcpMode
{
    Server = 0,
    Client = 1
}