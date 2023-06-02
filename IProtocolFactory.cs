using SerialPortTest.Options;

namespace SerialPortTest
{
    public interface IProtocolFactory
    {
        IPhysicalLayer GetPhysicalLayer(ProtocolOptions options);
    }
}