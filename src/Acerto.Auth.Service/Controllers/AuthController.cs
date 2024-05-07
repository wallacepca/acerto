using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Acerto.Shared.Contracts.Messages;
using Acerto.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Identity.Interfaces;
using NetDevPack.Identity.Jwt;
using NetDevPack.Identity.Jwt.Model;
using NetDevPack.Identity.Model;
using NetDevPack.Security.Jwt.Core.Interfaces;

namespace Acerto.Auth.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IOptions<AppJwtSettings> _jwtOptions;
        private readonly IJwtService _jwtService;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<AppJwtSettings> jwtOptions,
            IJwtService jwtService,
            IJwtBuilder jwtBuilder,
            ILogger<AuthController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions;
            _jwtService = jwtService;
            _jwtBuilder = jwtBuilder;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> RegisterAsync(RegisterUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identityUser = new IdentityUser
            {
                UserName = user.Email,
                Email = user.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, user.Password);

            if (result.Succeeded)
            {
                var response = await GenerateJwtAsync(user.Email);
                _logger.LogInformation("User {userLogin} created sucessyfully.", user.Email);
                return Ok(response);
            }
            else
            {
                var state = new ModelStateDictionary();

                foreach (var error in result.Errors)
                {
                    state.AddModelError(error.Code, error.Description);
                }

                return BadRequest(state);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserResponse>> LoginAsync(LoginUser loginUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if (result.Succeeded)
            {
                var loginResponse = await GenerateJwtAsync(loginUser.Email);

                return Ok(loginResponse);
            }

            if (result.IsLockedOut)
            {
                return BadRequest(ResultMessages.UserIsLocked);
            }
            else
            {
                return BadRequest(ResultMessages.InvalidUserOrPassword);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshTokenAsync([FromForm] string refreshToken)
        {
            var tokenValidation = await _jwtBuilder.ValidateRefreshToken(refreshToken);

            if (!tokenValidation)
            {
                return BadRequest(ResultMessages.RefreshTokenIsExpired);
            }

            var jwt = await GenerateJwtAsync(tokenValidation.UserId);

            return Ok(jwt);
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("It works!");
        }

        private static UserResponse GetTokenResponse(string accessToken, IdentityUser user, IEnumerable<Claim> claims)
        {
            return new UserResponse
            {
                AccessToken = accessToken,
                ExpiresIn = TimeSpan.FromHours(24).TotalSeconds,
                UserToken = new UserToken
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new UserClaim { Type = c.Type, Value = c.Value })
                }
            };
        }

        private async Task<UserResponse> GenerateJwtAsync(string email)
        {
            var user = (await _userManager.FindByEmailAsync(email))!;
            var claims = await _userManager.GetClaimsAsync(user);

            var identityClaims = await GetUserClaimsAsync(claims, user);

            var encodedToken = await GenerateAccessTokenAsync(identityClaims);

            return GetTokenResponse(encodedToken, user, claims);
        }

        private async Task<ClaimsIdentity> GetUserClaimsAsync(ICollection<Claim> claims, IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email!));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.UtcNow.ToUnixEpochDate().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToUnixEpochDate().ToString(), ClaimValueTypes.Integer64));

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            return identityClaims;
        }

        private async Task<string> GenerateAccessTokenAsync(ClaimsIdentity identityClaims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var currentIssuer = $"{ControllerContext.HttpContext.Request.Scheme}://{ControllerContext.HttpContext.Request.Host}";

            var key = await _jwtService.GetCurrentSigningCredentials();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = string.IsNullOrWhiteSpace(_jwtOptions.Value.Issuer) ? currentIssuer : _jwtOptions.Value.Issuer,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = key,
                IssuedAt = DateTime.UtcNow
            });

            return tokenHandler.WriteToken(token);
        }
    }
}
