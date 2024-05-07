using Acerto.Products.SDK;
using Acerto.Shared.Utilities;
using Microsoft.Extensions.Logging;
using Polly;
using Refit;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddSDK<TSdk, TOptions>(this IServiceCollection services, Action<TOptions> configureOptions)
            where TSdk : class
            where TOptions : ApiClientOptions, new()
        {
            services.AddOptions();
            services.Configure(configureOptions);

            var options = new TOptions();
            configureOptions.Invoke(options);

            services.AddRefitClient<TSdk>()
                .ConfigureHttpClient(x =>
                {
                    x.BaseAddress = new Uri(options.BaseAddress);
                })
                .AddHttpMessageHandler<PollytContextHandler<TSdk>>()
                .AddHttpMessageHandler<AuthenticationHandler>()
                .AddTransientHttpErrorPolicy(
                    builder => builder.RetryAsync((result, retryCount, context) =>
                    {
                        var logger = context["Logger"] as ILogger;
                        logger!.LogInformation("Error calling api - Error:{error} RetryCount: {retryCount}", result.Exception?.Message, retryCount);
                    }));

            services.AddAuthenticationServices<TSdk>();

            return services;
        }

        public static IServiceCollection AddAuthenticationServices<TSdk>(this IServiceCollection services)
        {
            services.AddTransient<IAuthTokenStore, AuthTokenStore>();
            services.AddScoped<AuthenticationHandler>();
            services.AddScoped<PollytContextHandler<TSdk>>();
            services.AddMemoryCache();

            return services;
        }
    }
}
