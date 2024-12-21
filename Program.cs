using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OfficeManagement.Data;
using Microsoft.AspNetCore.Identity;
using OfficeManagement.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// Add database context for Office management
builder.Services.AddDbContext<OfficeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OfficeContext")
        ?? throw new InvalidOperationException("Connection string 'OfficeContext' not found.")));

// Check that the OfficeIdentityContext is defined and registered correctly
builder.Services.AddDbContext<OfficeIdentityContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OfficeIdentityContext")
        ?? throw new InvalidOperationException("Connection string 'OfficeIdentityContext' not found.")));

builder.Services.AddDefaultIdentity<OfficeUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<OfficeIdentityContext>();

// Add Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Set the default page to welcome
app.MapGet("/", () => Results.Redirect("/Welcome"));

app.Run();

//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using OfficeManagement.Data;
//using Microsoft.AspNetCore.Identity;
//using OfficeManagement.Areas.Identity.Data;
//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddRazorPages();
//builder.Services.AddDbContext<OfficeContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("OfficeContext") ?? throw new InvalidOperationException("Connection string 'OfficeContext' not found.")));

//builder.Services.AddDefaultIdentity<OfficeUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<OfficeIdentityContext>();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();
//app.UseAuthentication();;

//app.UseAuthorization();

//app.MapRazorPages();

//app.Run();
