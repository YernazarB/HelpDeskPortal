using HelpDesk.Application.Requests.Queries;
using HelpDesk.Application.Responses;
using HelpDesk.Infrastructure.Helpers;
using HelpDesk.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HelpDesk.Infrastructure.Handlers.QueryHandlers
{
    public class GetOrganizationsQueryHandler : BaseHandler<GetOrganizationsQuery, OrganizationViewModel[]>
    {
        private readonly IDistributedCache _cache;
        public GetOrganizationsQueryHandler(AppDbContext db, ILogger<GetOrganizationsQueryHandler> logger, IUserAccessor userAccessor, 
            IDistributedCache cache) : base(db, logger, userAccessor)
        {
            _cache = cache;
        }

        protected override async Task<BaseResponse<OrganizationViewModel[]>> HandleRequest(GetOrganizationsQuery request, CancellationToken cancellationToken)
        {
            var organizationsString = await _cache.GetStringAsync(CacheHelper.OrganizationsKey, cancellationToken);
            OrganizationViewModel[] organizations;

            if (organizationsString != null)
            {
                organizations = JsonConvert.DeserializeObject<OrganizationViewModel[]>(organizationsString);
            }
            else
            {
                organizations = await DbContext.Organizations
                    .Select(x => new OrganizationViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                    }).ToArrayAsync(cancellationToken);
                organizationsString = JsonConvert.SerializeObject(organizations);

                var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };
                await _cache.SetStringAsync(CacheHelper.OrganizationsKey, organizationsString, options, cancellationToken);
            }

            return new BaseResponse<OrganizationViewModel[]>(organizations);
        }
    }
}
