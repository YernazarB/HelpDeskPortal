using HelpDesk.Application.Requests;
using HelpDesk.Application.Responses;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Infrastructure.Handlers
{
    public abstract class BaseHandler<T, E> : IRequestHandler<T, BaseResponse<E>> where E : class where T : BaseRequest<E>
    {
        protected readonly ILogger Logger;
        protected readonly AppDbContext DbContext;
        protected readonly IUserAccessor UserAccessor;
        public BaseHandler(AppDbContext db, ILogger logger, IUserAccessor userAccessor)
        {
            DbContext = db;
            Logger = logger;
            UserAccessor = userAccessor;
        }

        public async Task<BaseResponse<E>> Handle(T request, CancellationToken cancellationToken)
        {
            try
            {
                return await HandleRequest(request, cancellationToken);
            }
            catch (Exception e)
            {
                var message = e.InnerException?.Message ?? e.Message;
                Logger.LogError(message);
                return BadRequest(message);
            }
        }

        protected BaseResponse<E> Ok(E data)
        {
            return new BaseResponse<E>(data);
        }

        protected BaseResponse<E> Unauthorize()
        {
            return new BaseResponse<E>(code: ResponseCode.Unauthorize);
        }

        protected BaseResponse<E> Forbid()
        {
            return new BaseResponse<E>(code: ResponseCode.Forbidden);
        }

        protected BaseResponse<E> NotFound(string objName)
        {
            return new BaseResponse<E>(code: ResponseCode.NotFound, errorMessage: $"Object not found: {objName}");
        }

        protected BaseResponse<E> BadRequest(string message)
        {
            return new BaseResponse<E>(code: ResponseCode.BadRequest, errorMessage: message);
        }

        protected abstract Task<BaseResponse<E>> HandleRequest(T request, CancellationToken cancellationToken);
    }
}
