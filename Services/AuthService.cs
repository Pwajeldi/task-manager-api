using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Task_Management_App.DTOs;
using Task_Management_App.Models;

namespace Task_Management_App.Services
{
    public class AuthService : IAuthService
    {
        private UserManager<AppUserModel> _userManager;
        private IConfiguration _configuration;
        public AuthService(UserManager<AppUserModel> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }    //Injects the UserManager and IConfiguration to get appsettings data
        public async Task<string> LoginAsync(LoginDto dto)
        {
            // Checks the validity of user credentials
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                throw new Exception("Wrong Password");
            }

            var userRoles = await _userManager.GetRolesAsync(user); // Gets the role(s) of the found user

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            }; //Gets the Claims of the found user

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            } // Adds the user's role(s) to the list of claims

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            //Gets the private security key from appsettings via IConfiguration

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: authClaims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            //Bundles everything to build the token

            return new JwtSecurityTokenHandler().WriteToken(token);
            // Uses the Jwt-security-token-handler to write and return the newly constructed token as a string
        }
    }
}
