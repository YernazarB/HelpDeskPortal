using HelpDesk.Application.Requests.Queries;
using HelpDesk.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HelpDeskPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        public AuthController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(BaseResponse<LoginViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login(LoginQuery query)
        {
            return await HandleRequest(query);
        }
    }
}
