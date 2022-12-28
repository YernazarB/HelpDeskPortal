using HelpDesk.Application.Requests.Commands;
using HelpDesk.Application.Requests.Queries;
using HelpDesk.Application.Responses;
using HelpDesk.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HelpDeskPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : BaseController
    {
        public OrganizationsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(BaseResponse<OrganizationViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(int id)
        {
            return await HandleRequest(new GetOrganizationQuery { Id = id });
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<OrganizationViewModel[]>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            return await HandleRequest(new GetOrganizationsQuery());
        }

        [HttpPost]
        [AuthorizeByRole(UserRole.GlobalAdmin)]
        [ProducesResponseType(typeof(BaseResponse<OrganizationViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Create(CreateOrganizationCommand command)
        {
            return await HandleRequest(command);
        }

        [HttpPut]
        [AuthorizeByRole(UserRole.GlobalAdmin)]
        [ProducesResponseType(typeof(BaseResponse<OrganizationViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Update(UpdateOrganizationCommand command)
        {
            return await HandleRequest(command);
        }

        [HttpDelete]
        [AuthorizeByRole(UserRole.GlobalAdmin)]
        [Route("{id}")]
        [ProducesResponseType(typeof(BaseResponse<object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(int id)
        {
            return await HandleRequest(new DeleteOrganizationCommand { Id = id });
        }
    }
}
