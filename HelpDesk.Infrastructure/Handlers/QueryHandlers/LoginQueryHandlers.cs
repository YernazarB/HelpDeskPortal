using HelpDesk.Application.Requests.Queries;
using HelpDesk.Application.Responses;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Options;
using HelpDesk.Infrastructure.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HelpDesk.Infrastructure.Handlers
{
    public class LoginQueryHandlers : IRequestHandler<LoginQuery, BaseResponse<LoginViewModel>>
    {
        private readonly AppDbContext _db;
        private readonly ILogger<LoginQueryHandlers> _logger;
        private readonly IOptions<AuthOptions> _options;
        public LoginQueryHandlers(AppDbContext db, ILogger<LoginQueryHandlers> logger, IOptions<AuthOptions> options)
        {
            _db = db;
            _logger = logger;
            _options = options;
        }

        public async Task<BaseResponse<LoginViewModel>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var hash = UsersHelper.ComputeSha256Hash(request.Password);
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Username == request.Username && x.PasswordHash == hash);
                if (user == null)
                {
                    _logger.LogWarning($"Failed to authenticate user: {request.Username}.");

                    return new BaseResponse<LoginViewModel>
                    {
                        ErrorMessage = "Incorrect email or password",
                        Code = ResponseCode.Unauthorize
                    };
                }

                var token = GenerateJWT(user.Id, user.UserRole);
                _logger.LogInformation($"Generated a new token for user: {request.Username}.");

                return new BaseResponse<LoginViewModel>(new LoginViewModel
                {
                    AccessToken = token
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new BaseResponse<LoginViewModel>(errorMessage: e.Message, code: ResponseCode.BadRequest);
            }
        }

        private string GenerateJWT(int userId, UserRole userRole)
        {
            var authOptions = _options.Value;
            ArgumentNullException.ThrowIfNull(authOptions, nameof(authOptions));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
            };

            claims.Add(new Claim(ClaimTypes.Role, userRole.ToString()));

            var token = new JwtSecurityToken(
                authOptions.Issuer,
                authOptions.Audience,
                claims,
                expires: DateTime.Now.AddSeconds(authOptions.TokenLifeTime),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
