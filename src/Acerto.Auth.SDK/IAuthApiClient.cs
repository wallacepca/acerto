using Microsoft.AspNetCore.Mvc;
using NetDevPack.Identity.Jwt.Model;
using NetDevPack.Identity.Model;
using Refit;

namespace Acerto.Auth.SDK
{
    public interface IAuthApiClient
    {
        [Post("/auth/register")]
        Task<UserResponse> RegisterUserAsync(RegisterUser registerUser, CancellationToken cancellationToken = default);

        [Post("/auth/register")]
        Task<HttpResponseMessage> SendRegisterUserAsync(RegisterUser registerUser, CancellationToken cancellationToken = default);

        [Post("/auth/login")]
        Task<UserResponse> LoginAsync(LoginUser login, CancellationToken cancellationToken = default);

        [Post("/auth/login")]
        Task<HttpResponseMessage> SendLoginAsync(LoginUser login, CancellationToken cancellationToken = default);

        [Post("/auth/refresh-token")]
        Task<string> RefreshTokenAsync([FromForm] string refreshToken, CancellationToken cancellationToken = default);
    }
}
