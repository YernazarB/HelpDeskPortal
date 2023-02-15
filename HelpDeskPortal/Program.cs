using HelpDesk.Domain.Options;
using HelpDesk.Infrastructure;
using Microsoft.EntityFrameworkCore;
using HelpDeskPortal.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithAuth();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddInfrastructureServices();
builder.Services.AddRedisCache(builder.Configuration);

var authSection = builder.Configuration.GetSection("Auth");
builder.Services.Configure<AuthOptions>(authSection);
var authOptions = authSection.Get<AuthOptions>();
builder.Services.AddAuthenticationAndAuthorization(authOptions);

var app = builder.Build();

await app.MigrateDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
