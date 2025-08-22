using API.Extensions;
using API.Helpers;
using API.Middleware;
using API.SignalR;
using Core.Entities.Identity;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Services;
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
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDbContext<AppDbContext>(options =>
//            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDbContext<AppIdentityDbContext>(options =>
//            options.UseSqlite(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(c =>
{
    var conString = builder.Configuration.GetConnectionString("Redis") ?? throw new Exception("Can't get redis connection string");
    var config = ConfigurationOptions.Parse(conString, true);

    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddSingleton<ICartService, CartService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddApplicationServices();
//builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddSwaggerDocumentation();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithOrigins("https://eshop-project-2025.azurewebsites.net");
        //.WithOrigins("http://localhost:5000", "https://localhost:5001");
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
    app.MapOpenApi();
}

app.UseStatusCodePagesWithReExecute("/errors/{0}"); // Ex. If the path doesn't match any route call the error method inside ErrorController 
app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles(); // To load images from wwwroot

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGroup("api").MapIdentityApi<AppUser>(); // api/login
app.MapHub<NotificationHub>("/hub/notifications");
app.MapFallbackToController("Index", "Fallback");

#region MigrateAsync / Seeding Data

using var serviceScope = app.Services.CreateScope();

var services = serviceScope.ServiceProvider;
var loggerFactory = services.GetRequiredService<ILoggerFactory>();

try
{
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await AppIdentityDbContextSeed.SeedUsersAsync(userManager);
    await AppDbContextSeed.SeedAsync(context, loggerFactory);

    //var userManager = services.GetRequiredService<UserManager<AppUser>>();
    //var identityContext = services.GetRequiredService<AppIdentityDbContext>();
    //await identityContext.Database.MigrateAsync();
    //await AppIdentityDbContextSeed.SeedUsersAsync(userManager);
}
catch (Exception ex)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(ex, "An error occured during migration/seeding data");
}

#endregion MigrateAsync / Seeding Data


app.Run();
