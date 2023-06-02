namespace Arcta.Lims.Machines.Protocols.Transport.Options;
public class TcpOptions
{
    public const string Tcp = "Tcp";
    public required string IPAddress { get; set; }
    public required int Port { get; set; }
    public TcpMode TcpMode { get; set; } = TcpMode.Listener;
}
public enum TcpMode
{
    Listener = 0,
    Client = 1
}