﻿namespace SerialPortTest.Options
{
    public class ProtocolOptions
    {
        public const string Protocol = "Protocol";
        public required PhysicalLayerOptions PhysicalLayer { get; set; }
    }
}