using HelpDesk.Domain.Enums;

namespace HelpDesk.Infrastructure.Services
{
    public interface IUserAccessor
    {
        int? UserId { get; }
        bool IsInRole(UserRole role);
    }
}
