using System.Net.Http.Json;
using Acerto.Auth.SDK;
using Acerto.Products.SDK;
using Microsoft.Extensions.Options;
using NetDevPack.Identity.Jwt.Model;
using NetDevPack.Identity.Model;

namespace Acerto.Shared.Utilities
{
    public sealed class TokenRefreshHandler : DelegatingHandler
    {
        private readonly IAuthApiClient _authApiClient;
        private readonly IOptions<AuthApiClientOptions> _options;
        private readonly IAuthTokenStore _authTokenStore;

        public TokenRefreshHandler(
            IAuthApiClient authApiClient,
            IOptions<AuthApiClientOptions> options,
            IAuthTokenStore authTokenStore)
        {
            _authApiClient = authApiClient;
            _options = options;
            _authTokenStore = authTokenStore ?? throw new ArgumentNullException(nameof(authTokenStore));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken)
                    .ConfigureAwait(false);

                return response;
            }
            catch (Exception e) when (e is TokenExpiredException)
            {
                var response = await _authApiClient.SendLoginAsync(new LoginUser { Email = _options.Value.User, Password = _options.Value.Password });
                if (response.IsSuccessStatusCode)
                {
                    var token = response.Content.ReadFromJsonAsync<UserResponse>(cancellationToken);
                    await _authTokenStore.SetTokenAsync(token);

                    // reenvia request original
                    return await base.SendAsync(request, cancellationToken)
                        .ConfigureAwait(false);
                }

                throw;
            }
        }
    }
}
