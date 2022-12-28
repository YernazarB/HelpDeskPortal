using Microsoft.AspNetCore.Authorization;
using HelpDesk.Domain.Enums;

namespace HelpDeskPortal
{
    class AuthorizeByRoleAttribute : AuthorizeAttribute
    {
        public AuthorizeByRoleAttribute(UserRole role)
        {
            Roles = role.ToString();
        }

        public AuthorizeByRoleAttribute(params UserRole[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}
