using HelpDesk.Application.Requests.Queries;
using HelpDesk.Application.Responses;
using HelpDesk.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Infrastructure.Handlers.QueryHandlers
{
    public class GetOrganizationsQueryHandler : BaseHandler<GetOrganizationsQuery, OrganizationViewModel[]>
    {
        public GetOrganizationsQueryHandler(AppDbContext db, ILogger<GetOrganizationsQueryHandler> logger, IUserAccessor userAccessor)
            : base(db, logger, userAccessor)
        {
        }

        protected override async Task<BaseResponse<OrganizationViewModel[]>> HandleRequest(GetOrganizationsQuery request, CancellationToken cancellationToken)
        {
            return new BaseResponse<OrganizationViewModel[]>(
                await DbContext.Organizations
                    .Select(x => new OrganizationViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                    }).ToArrayAsync(cancellationToken));
        }
    }
}
