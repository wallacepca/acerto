using Acerto.Auth.SDK;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthApiClientExtensions
    {
        public static IServiceCollection AddAuthSDK(this IServiceCollection services, Action<AuthApiClientOptions> configureOptions)
            => services.AddSDK<IAuthApiClient, AuthApiClientOptions>(configureOptions);
    }
}
