using HelpDesk.Application.Requests.Queries;
using HelpDesk.Application.Responses;
using HelpDesk.Infrastructure.Helpers;
using HelpDesk.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HelpDesk.Infrastructure.Handlers
{
    public class GetOrganizationQueryHandler : BaseHandler<GetOrganizationQuery, OrganizationViewModel>
    {
        public GetOrganizationQueryHandler(AppDbContext db, ILogger<GetOrganizationQueryHandler> logger, IUserAccessor userAccessor,
            IDistributedCache cache)
            : base(db, logger, userAccessor, cache)
        {
        }

        protected override async Task<BaseResponse<OrganizationViewModel>> HandleRequest(GetOrganizationQuery request, CancellationToken cancellationToken)
        {
            OrganizationViewModel model;
            var organizationKey = $"{CacheHelper.OrganizationsKey}:{request.Id}";
            var organizationJson = await Cache.GetStringAsync(organizationKey);

            if (organizationJson != null)
            {
                model = JsonConvert.DeserializeObject<OrganizationViewModel>(organizationJson);
            }

            var organization = await DbContext.Organizations.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (organization == null)
            {
                return NotFound(nameof(organization));
            }
            model = new OrganizationViewModel { Id = organization.Id, Name = organization.Name };
            organizationJson = JsonConvert.SerializeObject(model);
            await Cache.SetStringAsync(organizationKey, organizationJson);

            return new BaseResponse<OrganizationViewModel>(model);
        }
    }
}
