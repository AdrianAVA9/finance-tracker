using Fintrack.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

using var app = builder.Build();
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

var count = await context.IncomeCategories.CountAsync();
Console.WriteLine($"Total Income Categories: {count}");

var categories = await context.IncomeCategories.ToListAsync();
foreach (var cat in categories)
{
    Console.WriteLine($"- {cat.Name} (Icon: {cat.Icon}, Color: {cat.Color})");
}
