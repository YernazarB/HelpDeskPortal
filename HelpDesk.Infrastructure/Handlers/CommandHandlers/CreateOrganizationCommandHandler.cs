using HelpDesk.Application.Requests.Commands;
using HelpDesk.Application.Responses;
using HelpDesk.Domain.Entities;
using HelpDesk.Infrastructure.Helpers;
using HelpDesk.Infrastructure.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Infrastructure.Handlers
{
    public class CreateOrganizationCommandHandler : BaseHandler<CreateOrganizationCommand, OrganizationViewModel>
    {
        public CreateOrganizationCommandHandler(AppDbContext db, ILogger<CreateOrganizationCommandHandler> logger, IUserAccessor userAccessor,
            IDistributedCache cache) 
            : base(db, logger, userAccessor, cache)
        {
        }

        protected override async Task<BaseResponse<OrganizationViewModel>> HandleRequest(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = new Organization
            {
                Name = request.Name
            };

            await DbContext.Organizations.AddAsync(organization);
            await DbContext.SaveChangesAsync(UserAccessor.UserId);
            await Cache.RemoveAsync(CacheHelper.OrganizationsKey, cancellationToken);

            return new BaseResponse<OrganizationViewModel>
                (new OrganizationViewModel { Id = organization.Id, Name = organization.Name });
        }
    }
}
