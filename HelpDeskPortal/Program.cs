using HelpDesk.Infrastructure.Options;
using HelpDesk.Infrastructure;
using Microsoft.EntityFrameworkCore;
using HelpDeskPortal.Extensions;
using MassTransit;

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
var messageBrokerSection = builder.Configuration.GetSection("MessageBroker");
builder.Services.Configure<AuthOptions>(authSection);
builder.Services.Configure<MessageBrokerOptions>(messageBrokerSection);

var authOptions = authSection.Get<AuthOptions>();
var messageBrokerOptions = messageBrokerSection.Get<MessageBrokerOptions>();

builder.Services.AddAuthenticationAndAuthorization(authOptions);
//builder.Services.AddMessageBroker(messageBrokerOptions);

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
