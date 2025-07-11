using API.Extensions;
using API.Helpers;
using API.Middleware;
using Core.Entities.Identity;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddAutoMapper(typeof(MappingProfiles));

builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(c =>
{

    var config = ConfigurationOptions.Parse(
        builder.Configuration.GetConnectionString("Redis"), true
    );

    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddSwaggerDocumentation();
builder.Services.AddApplicationServices();
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200", "https://localhost:4200");
    });
});

var app = builder.Build();

#region MigrateAsync / Seeding Data

using var serviceScope = app.Services.CreateScope();

var services = serviceScope.ServiceProvider;
var loggerFactory = services.GetRequiredService<ILoggerFactory>();

try
{
    var context = services.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await AppDbContextSeed.SeedAsync(context, loggerFactory);

    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var identityContext = services.GetRequiredService<AppIdentityDbContext>();
    await identityContext.Database.MigrateAsync();
    await AppIdentityDbContextSeed.SeedUsersAsync(userManager);
}
catch (Exception ex)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(ex, "An error occured during migration/seeding data");
}

#endregion MigrateAsync / Seeding Data

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
    app.MapOpenApi();
}

app.UseStatusCodePagesWithReExecute("/errors/{0}"); // Ex. If the path doesn't match any route call the error method inside ErrorController 

app.UseHttpsRedirection();

app.UseStaticFiles(); // To load images from wwwroot

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
