using HelpDesk.Application.Requests.Queries;
using HelpDesk.Application.Responses;
using HelpDesk.Infrastructure.Helpers;
using HelpDesk.Infrastructure.Services;
using HelpDesk.Infrastructure.Services.CacheService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Infrastructure.Handlers
{
    public class GetOrganizationQueryHandler : BaseHandler<GetOrganizationQuery, OrganizationViewModel>
    {
        public GetOrganizationQueryHandler(AppDbContext db, ILogger<GetOrganizationQueryHandler> logger, IUserAccessor userAccessor,
            ICacheService cache)
            : base(db, logger, userAccessor, cache)
        {
        }

        protected override async Task<BaseResponse<OrganizationViewModel>> HandleRequest(GetOrganizationQuery request, CancellationToken cancellationToken)
        {
            var organizationKey = $"{CacheHelper.OrganizationsKey}:{request.Id}";
            var model = await Cache.GetData<OrganizationViewModel>(organizationKey, cancellationToken);

            if (model != null)
            {
                return new BaseResponse<OrganizationViewModel>(model);
            }

            var organization = await DbContext.Organizations.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (organization == null)
            {
                return NotFound(nameof(organization));
            }

            model = new OrganizationViewModel { Id = organization.Id, Name = organization.Name };
            await Cache.SetData(organizationKey, model, cancellationToken);

            return new BaseResponse<OrganizationViewModel>(model);
        }
    }
}
