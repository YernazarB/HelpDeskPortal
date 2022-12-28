using HelpDesk.Domain.Enums;

namespace HelpDesk.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole UserRole { get; set; }
        public string PasswordHash { get; set; }
        public int? OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
