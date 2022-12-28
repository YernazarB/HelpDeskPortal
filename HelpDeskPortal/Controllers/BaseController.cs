using HelpDesk.Application.Requests;
using HelpDesk.Application.Responses;
using HelpDesk.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BaseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected async Task<IActionResult> HandleRequest<T>(BaseRequest<T> request) where T : class
        {
            var response = await _mediator.Send(request);
            return CreateResponse(response);
        }

        private IActionResult CreateResponse<T>(BaseResponse<T> response, bool totalCountAdded = false) where T : class
        {
            switch (response.Code)
            {
                case ResponseCode.Ok:
                    return Ok(response);
                case ResponseCode.BadRequest:
                    return BadRequest(response);
                case ResponseCode.Unauthorize:
                    return Unauthorized(response);
                case ResponseCode.Forbidden:
                    return Forbid();
                case ResponseCode.NotFound:
                    return NotFound();
                default:
                    return BadRequest($"Unhandled status code: {response.Code}");
            }
        }
    }
}
