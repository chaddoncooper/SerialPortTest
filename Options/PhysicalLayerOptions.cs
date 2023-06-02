using SerialPortTest.RS232;
using SerialPortTest.TCP;

namespace SerialPortTest.Options
{
    public class PhysicalLayerOptions
    {
        public const string PhysicalLayer = "PhysicalLayer";
        public required string Type { get; set; }
        public TcpOptions? Tcp { get; set; }
        public RS232Options? Rs232 { get; set; }
    }
}
