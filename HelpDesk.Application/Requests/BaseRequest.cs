using HelpDesk.Application.Responses;
using MediatR;

namespace HelpDesk.Application.Requests
{
    public class BaseRequest<T> : IRequest<BaseResponse<T>> where T : class
    {
    }
}
