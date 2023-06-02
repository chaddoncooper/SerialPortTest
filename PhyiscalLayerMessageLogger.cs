using Microsoft.Extensions.Logging;

namespace SerialPortTest
{
    internal class PhyiscalLayerMessageLogger
    {
        private readonly ILogger _logger;

        public PhyiscalLayerMessageLogger(ILogger logger)
        {
            this._logger = logger;
        }

        void LogInboundMessage(string inboundMessage)
        {

        }
    }
}
