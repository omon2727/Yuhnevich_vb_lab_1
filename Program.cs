using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Yuhnevich_vb_lab.Data;
using Yuhnevich_vb_lab.Services;
using Yuhnevich_vb_lab.Services.CategoryService;
using Yuhnevich_vb_lab.Services.ProductService;
using Yuhnevich_vb_lab.UI.Services.CategoryService;
using Yuhnevich_vb_lab.UI.Services.ProductService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("SqliteConnection") ?? throw new InvalidOperationException("Connection string 'SqliteConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Регистрация HttpClient для ApiProductService и ApiCategoryService
builder.Services.AddHttpClient<IProductService, ApiProductService>(opt =>
    opt.BaseAddress = new Uri("https://localhost:7002/api/dishes/"));
builder.Services.AddHttpClient<ICategoryService, ApiCategoryService>(opt =>
    opt.BaseAddress = new Uri("https://localhost:7002/api/categories/"));

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("admin", p =>
        p.RequireClaim(ClaimTypes.Role, "admin"));
});

builder.Services.AddTransient<IEmailSender, NoOpEmailSender>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

// Маршрут для Image
app.MapControllerRoute(
    name: "image",
    pattern: "Image/{action=Index}/{id?}",
    defaults: new { controller = "Image" })
    .WithStaticAssets();

// Маршрут для Catalog с поддержкой category и pageNo
app.MapControllerRoute(
    name: "Catalog",
    pattern: "Catalog/{category?}/{pageNo:int?}",
    defaults: new { controller = "Product", action = "Index" })
    .WithStaticAssets();

// Маршрут по умолчанию
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

await DbInit.SeedData(app);

app.Run();