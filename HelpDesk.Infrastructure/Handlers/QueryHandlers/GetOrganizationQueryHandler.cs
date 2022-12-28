using HelpDesk.Application.Requests.Queries;
using HelpDesk.Application.Responses;
using HelpDesk.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Infrastructure.Handlers
{
    public class GetOrganizationQueryHandler : BaseHandler<GetOrganizationQuery, OrganizationViewModel>
    {
        public GetOrganizationQueryHandler(AppDbContext db, ILogger<GetOrganizationQueryHandler> logger, IUserAccessor userAccessor)
            : base(db, logger, userAccessor)
        {
        }

        protected override async Task<BaseResponse<OrganizationViewModel>> HandleRequest(GetOrganizationQuery request, CancellationToken cancellationToken)
        {
            var organization = await DbContext.Organizations.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (organization == null)
            {
                return NotFound(nameof(organization));
            }

            return new BaseResponse<OrganizationViewModel>
                (new OrganizationViewModel { Id = organization.Id, Name = organization.Name });
        }
    }
}
