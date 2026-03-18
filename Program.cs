using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;
using Task_Management_App.DataBase;
using Task_Management_App.Models;
using Task_Management_App.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// REGISTER DATABASE SERVICE
builder.Services.AddDbContext<TaskDatabase>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// REGISTER IDENTITY SERVICE FOR USER ROLES
builder.Services.AddIdentity<AppUserModel, IdentityRole>()
    .AddEntityFrameworkStores<TaskDatabase>();

ArgumentNullException.ThrowIfNull(builder.Configuration["Jwt:Key"]);
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

// REGISTER AUTHENTICATION SERVICE WITH JWT BEARER
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>   //JWT configurations
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RoleClaimType = ClaimTypes.Role
    };
});
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// ROLE SEEDING via scoped instance
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User" };
    foreach(var role in roles) 
    {
        if(!await roleManager.RoleExistsAsync(role))  // Checks if the role already exists to avoid duplicates.
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// USER SEEDING via scoped instance
using(var scope = app.Services.CreateScope())
{
    string email = "pwajeldi900@gmail.com";
    string password = "Visored77@";
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUserModel>>();
    if (await userManager.FindByEmailAsync(email) == null) // Checks that a user with the specified email does not already exist.
    {
        var user = new AppUserModel(); // if successful, create a new user
        user.UserName = email;
        user.Email = email;
        user.LastName = "Joshua";
        user.FirstName = "Joshua";
        user.CreatedDate = DateTime.UtcNow;
        
        await userManager.CreateAsync(user, password);  // register the user
        await userManager.AddToRoleAsync(user, "Admin"); // Makes the new created user to have the admin role
    }
}

app.UseAuthentication();

app.MapControllers();

app.Run();
