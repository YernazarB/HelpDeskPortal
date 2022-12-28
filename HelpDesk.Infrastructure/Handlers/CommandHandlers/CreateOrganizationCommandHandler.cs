using HelpDesk.Application.Requests.Commands;
using HelpDesk.Application.Responses;
using HelpDesk.Domain.Entities;
using HelpDesk.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Infrastructure.Handlers
{
    public class CreateOrganizationCommandHandler : BaseHandler<CreateOrganizationCommand, OrganizationViewModel>
    {
        public CreateOrganizationCommandHandler(AppDbContext db, ILogger<CreateOrganizationCommandHandler> logger, IUserAccessor userAccessor) 
            : base(db, logger, userAccessor)
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

            return new BaseResponse<OrganizationViewModel>
                (new OrganizationViewModel { Id = organization.Id, Name = organization.Name });
        }
    }
}
