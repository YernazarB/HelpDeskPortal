using HelpDesk.Application.Responses;
using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Requests.Commands
{
    public class CreateOrganizationCommand : BaseRequest<OrganizationViewModel>
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; }
    }
}
