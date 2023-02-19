using HelpDesk.Application.Requests.Queries;
using HelpDesk.Application.Responses;
using HelpDesk.Infrastructure.Helpers;
using HelpDesk.Infrastructure.Services;
using HelpDesk.Infrastructure.Services.CacheService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Infrastructure.Handlers.QueryHandlers
{
    public class GetOrganizationsQueryHandler : BaseHandler<GetOrganizationsQuery, OrganizationViewModel[]>
    {
        public GetOrganizationsQueryHandler(AppDbContext db, ILogger<GetOrganizationsQueryHandler> logger, IUserAccessor userAccessor, 
            ICacheService cache) : base(db, logger, userAccessor, cache)
        {
        }

        protected override async Task<BaseResponse<OrganizationViewModel[]>> HandleRequest(GetOrganizationsQuery request, CancellationToken cancellationToken)
        {
            var organizations = await Cache.GetData<OrganizationViewModel[]>(CacheHelper.OrganizationsKey, cancellationToken);

            if (organizations != null)
            {
                return new BaseResponse<OrganizationViewModel[]>(organizations);
            }

            organizations = await DbContext.Organizations
                    .Where(x => !x.IsDeleted)
                    .Select(x => new OrganizationViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                    }).ToArrayAsync(cancellationToken);

            await Cache.SetData(CacheHelper.OrganizationsKey, organizations, cancellationToken);

            return new BaseResponse<OrganizationViewModel[]>(organizations);
        }
    }
}
