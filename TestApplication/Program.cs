using Microsoft.EntityFrameworkCore;
using TestData;
using TestData.Repos;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var connectionString = configuration["Database:ConnectionString"] ?? throw new ArgumentNullException("Database:ConnectionString");

builder.Services.AddDbContext<TestDbContext>(options =>
{
    options.UseNpgsql(connectionString, o => o.UseNetTopologySuite());
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

builder.Services.AddScoped<IRepository, RepositoryADO>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseDeveloperExceptionPage();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
