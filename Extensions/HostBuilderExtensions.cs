using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Arcta.Lims.Machines.Protocols.Transport.Extensions;
internal static class HostBuilderExtensions
{
    public static IHostBuilder RegisterServices(this IHostBuilder builder)
    {
        return builder.ConfigureServices((hostBuilderContext, services) =>
            services
                .AddSingleton<ITransportFactory, TransportFactory>()
        );
    }
}
