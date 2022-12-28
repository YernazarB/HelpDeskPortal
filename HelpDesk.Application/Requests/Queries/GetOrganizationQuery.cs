using HelpDesk.Application.Responses;

namespace HelpDesk.Application.Requests.Queries
{
    public class GetOrganizationQuery : BaseRequest<OrganizationViewModel>
    {
        public int Id { get; set; }
    }
}
