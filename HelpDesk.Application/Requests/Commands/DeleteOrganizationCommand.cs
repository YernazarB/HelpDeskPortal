namespace HelpDesk.Application.Requests.Commands
{
    public class DeleteOrganizationCommand : BaseRequest<object>
    {
        public int Id { get; set; }
    }
}
