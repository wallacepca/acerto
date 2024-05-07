using Microsoft.Extensions.DependencyInjection;

namespace Acerto.Testing.App
{
    internal class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddAuthSDK(x => x.BaseAddress = "https://localhost:50011");
            services.AddProductsSDK(x => x.BaseAddress = "https://localhost:50021");
            services.AddOrdersSDK(x => x.BaseAddress = "https://localhost:50031");

            services.AddTransient<TestConsole>();

            var serviceProvider = services.BuildServiceProvider();
            var testConsole = serviceProvider.GetRequiredService<TestConsole>();
            await testConsole.RunAsync();
        }
    }
}
