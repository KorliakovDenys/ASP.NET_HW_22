using ASP.NET_HW_22.Data;
using ASP.NET_HW_22.Models;
using ASP.NET_HW_22.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IDbService, DbService>();

builder.Services.AddSingleton<IRepository<City>>(serviceProvider =>
    new Repository<City>(serviceProvider.GetRequiredService<IDbService>(), "Cities"));
builder.Services.AddSingleton<IRepository<Country>>(serviceProvider =>
    new Repository<Country>(serviceProvider.GetRequiredService<IDbService>(), "Countries"));
builder.Services.AddSingleton<IRepository<Continent>>(serviceProvider =>
    new Repository<Continent>(serviceProvider.GetRequiredService<IDbService>(), "Continents"));

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();