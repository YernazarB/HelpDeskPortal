using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HelpDesk.Infrastructure.Services
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly Lazy<int?> _userId;

        public UserAccessor(IHttpContextAccessor accessor)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _userId = new Lazy<int?>(() => GetUserId());
        }

        private int? GetUserId()
        {
            if (_accessor.HttpContext.User is null)
            {
                return null;
            }

            if (!int.TryParse(GetUserClaim(ClaimTypes.NameIdentifier), out var userId))
            {
                return null;
            }

            return userId;
        }

        public int? UserId { get { return _userId.Value; } }
        public bool IsInRole(UserRole role)
        {
            return _accessor.HttpContext.User != null && _accessor.HttpContext.User.IsInRole(role.ToString());
        }

        private string GetUserClaim(string claimType) => _accessor.HttpContext.User?.Claims
            .FirstOrDefault(x => x.Type == claimType)?.Value;

    }
}
