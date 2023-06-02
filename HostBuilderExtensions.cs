using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SerialPortTest;
internal static class HostBuilderExtensions
{
    public static IHostBuilder RegisterServices(this IHostBuilder builder)
    {
        return builder.ConfigureServices((hostBuilderContext, services) =>
            services

                .Configure<RS232Options>(hostBuilderContext.Configuration.GetSection(RS232Options.RS232))
                .AddScoped<RS232InstrumentPhysicalLayer>()
        );
    }
}
