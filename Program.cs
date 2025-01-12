using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Synapse.Services;

namespace Synapse.OrdersExample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection(); 
            ConfigureServices(serviceCollection); 
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var alertService = serviceProvider.GetRequiredService<AlertService>();
            var orderService = serviceProvider.GetRequiredService<OrderService>();
            var logger = serviceProvider.GetRequiredService<ILogger<SynapseCode>>(); 
            
            logger.LogInformation("Application started");

            SynapseCode synapseCode = new SynapseCode(logger, alertService, orderService);
            await synapseCode.ProcessOrders();

            logger.LogInformation("Application ended");
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure
            .AddConsole()
            .AddDebug())
            .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);

            services.AddHttpClient<AlertService>();
            services.AddHttpClient<OrderService>();
        }
    }
}