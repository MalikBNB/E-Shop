using API.Extensions;
using API.Helpers;
using API.Middleware;
using Infrastructure.Data;
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

builder.Services.AddSingleton<IConnectionMultiplexer>(c => {

    var config = ConfigurationOptions.Parse(
        builder.Configuration.GetConnectionString("Redis"), true
    );

    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddSwaggerDocumentation();
builder.Services.AddApplicationServices();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200");
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
}
catch(Exception ex)
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

app.UseAuthorization();

app.MapControllers();

app.Run();
