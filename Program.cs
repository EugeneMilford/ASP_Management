using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OfficeManagement.Data;
using Microsoft.AspNetCore.Identity;
using OfficeManagement.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add database context for Office management
builder.Services.AddDbContext<OfficeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OfficeContext")
        ?? throw new InvalidOperationException("Connection string 'OfficeContext' not found.")));

// Configure identity services using OfficeContext for authentication
builder.Services.AddIdentity<OfficeUser, IdentityRole>(options =>
{
    // Configure identity options here as needed
    options.SignIn.RequireConfirmedAccount = true; 
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<OfficeContext>()
.AddDefaultTokenProviders();


builder.Services.AddTransient<IEmailSender, OfficeManagement.Services.EmailSender>();

// Add Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline
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

