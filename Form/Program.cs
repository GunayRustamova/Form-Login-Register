using Form.DAL;
using Form.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddIdentity<AppUser, IdentityRole>(IdentityOptions =>
{
    IdentityOptions.Password.RequiredLength = 8;
    IdentityOptions.Password.RequireNonAlphanumeric = false;
    IdentityOptions.Password.RequireUppercase = true;
    IdentityOptions.Password.RequireLowercase = true;
    IdentityOptions.Lockout.MaxFailedAccessAttempts = 3;
    IdentityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    IdentityOptions.Lockout.AllowedForNewUsers = true;
    IdentityOptions.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._";
    IdentityOptions.User.RequireUniqueEmail = true;
}).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>(); ;
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
