using System.Net;
using System.Net.Http.Headers;
using Acerto.Products.SDK;
using NetDevPack.Identity.Jwt.Model;

namespace Acerto.Shared.Utilities
{
    public sealed class AuthenticationHandler : DelegatingHandler
    {
        private readonly IAuthTokenStore _authTokenStore;

        public AuthenticationHandler(IAuthTokenStore authTokenStore)
        {
            _authTokenStore = authTokenStore ?? throw new ArgumentNullException(nameof(authTokenStore));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _authTokenStore.GetTokenAsync<UserResponse>();

            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            }

            var response = await base.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            if (token is not null && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!string.IsNullOrWhiteSpace(content))
                {
                    throw new TokenExpiredException();
                }
            }

            return response;
        }
    }
}
