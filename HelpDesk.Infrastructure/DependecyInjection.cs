using HelpDesk.Infrastructure.Services;
using HelpDesk.Infrastructure.Services.CacheService;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HelpDesk.Infrastructure
{
    public static class DependecyInjection
    {
        public static void AddInfrastructureServices(this IServiceCollection collection)
        {
            collection.AddMediatR(Assembly.GetExecutingAssembly());
            collection.AddTransient<IUserAccessor, UserAccessor>();
            collection.AddScoped<ICacheService, CacheService>();
        }
    }
}
