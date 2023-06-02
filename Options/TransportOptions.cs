namespace Arcta.Lims.Machines.Protocols.Transport.Options
{
    public class TransportOptions
    {
        public const string Transport = "Transport";
        public required string Type { get; set; }
        public TcpOptions? Tcp { get; set; }
        public RS232Options? Rs232 { get; set; }
    }
}
