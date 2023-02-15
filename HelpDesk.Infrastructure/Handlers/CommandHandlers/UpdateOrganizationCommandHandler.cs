﻿using HelpDesk.Application.Requests.Commands;
using HelpDesk.Application.Responses;
using HelpDesk.Infrastructure.Helpers;
using HelpDesk.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Infrastructure.Handlers
{
    public class UpdateOrganizationCommandHandler : BaseHandler<UpdateOrganizationCommand, OrganizationViewModel>
    {
        public UpdateOrganizationCommandHandler(AppDbContext db, ILogger<UpdateOrganizationCommandHandler> logger, IUserAccessor userAccessor, 
            IDistributedCache cache) : base(db, logger, userAccessor, cache)
        {
        }

        protected override async Task<BaseResponse<OrganizationViewModel>> HandleRequest(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await DbContext.Organizations.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (organization == null)
            {
                return NotFound(nameof(organization));
            }

            organization.Name = request.Name;
            await DbContext.SaveChangesAsync(UserAccessor.UserId);
            await Cache.RemoveAsync(CacheHelper.OrganizationsKey);
            await Cache.RemoveAsync($"{CacheHelper.OrganizationsKey}:{organization.Id}");

            return new BaseResponse<OrganizationViewModel>
                (new OrganizationViewModel { Id = organization.Id, Name = organization.Name });
        }
    }
}
