namespace HelpDesk.Domain.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public int? CreatedById { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public int? ModifiedById { get; set; }
    }
}
