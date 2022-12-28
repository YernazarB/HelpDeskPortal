using HelpDesk.Application.Requests.Commands;
using HelpDesk.Application.Responses;
using HelpDesk.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Infrastructure.Handlers
{
    public class DeleteOrganizationCommandHandler : BaseHandler<DeleteOrganizationCommand, object>
    {
        public DeleteOrganizationCommandHandler(AppDbContext db, ILogger<DeleteOrganizationCommandHandler> logger, IUserAccessor userAccessor) : base(db, logger, userAccessor)
        {
        }

        protected override async Task<BaseResponse<object>> HandleRequest(DeleteOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await DbContext.Organizations.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (organization == null)
            {
                return NotFound(nameof(organization));
            }

            organization.IsDeleted = true;
            await DbContext.SaveChangesAsync(UserAccessor.UserId);

            return new BaseResponse<object>();
        }
    }
}
