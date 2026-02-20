using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OfficeManagement.Data;
using Microsoft.AspNetCore.Identity;
using OfficeManagement.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OfficeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OfficeContext")
        ?? throw new InvalidOperationException("Connection string 'OfficeContext' not found.")));

// Configure identity services using OfficeContext for authentication
builder.Services.AddIdentity<OfficeUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<OfficeContext>()
.AddDefaultTokenProviders();

// Enable session support
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout duration
    options.Cookie.HttpOnly = true; // Ensure the session cookie is only accessible via HTTP requests
    options.Cookie.IsEssential = true; // Mark session cookie as essential
});

builder.Services.AddTransient<IEmailSender, OfficeManagement.Services.EmailSender>();

builder.Services.AddRazorPages();

builder.Host.UseSerilog((ctx, lc) =>
    lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();


// Configuring the HTTP request pipeline
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

app.MapGet("/", () => Results.Redirect("/Welcome"));

app.Run();

