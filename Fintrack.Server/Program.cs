using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using Fintrack.Server.Models;
using Fintrack.Server.Data;
using Fintrack.Server.Infrastructure;
using Fintrack.Server.Infrastructure.Authorization;
using Fintrack.Server.Infrastructure.Email;
using Fintrack.Server.Api.Middleware;
using Microsoft.AspNetCore.Identity.UI.Services;
using Fintrack.Server.Infrastructure.Data.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Add Custom Infrastructure & Authorization
builder.Services.AddInfrastructure();
builder.Services.AddPermissionAuthorization();

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Override Identity's built-in no-op email sender with our SMTP implementation.
// These MUST be registered after AddIdentityApiEndpoints to take effect.
builder.Services.AddTransient<IEmailSender, SmtpMailSender>();
builder.Services.AddTransient<IEmailSender<ApplicationUser>, IdentityEmailSender>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// In Development, expose real exception details for easier debugging
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Seed Database if needed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await DefaultCategorySeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.UseCustomExceptionHandler();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapIdentityApi<ApplicationUser>();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();

public partial class Program { }
