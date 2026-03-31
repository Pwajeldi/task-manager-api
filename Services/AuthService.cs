using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Task_Management_App.DataBase;
using Task_Management_App.DTOs;
using Task_Management_App.Models;

namespace Task_Management_App.Services
{
    public class AuthService : IAuthService
    {
        private UserManager<AppUserModel> _userManager;
        private IConfiguration _configuration;
        private readonly TaskDatabase _context;
        public AuthService(UserManager<AppUserModel> userManager, IConfiguration configuration, TaskDatabase context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }    //Injects the UserManager and IConfiguration to get appsettings data
        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
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
            var userID = await _userManager.GetUserIdAsync(user);

            // GENERATE TOKENS
            var accessToken = await GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken(userID);

            await _context.RefreshTokenTab.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new LoginResponseDto{
                AccessToken = accessToken,
                RefreshToken = refreshToken.RefreshToken 
            };
        }

        public RefreshTokenMod GenerateRefreshToken(string userId)
        {
            var randomnum = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomnum);
            var refreshtoken = Convert.ToBase64String(randomnum);
            return new RefreshTokenMod
            {
                UserId = userId,
                RefreshToken = refreshtoken,
                Expires = DateTime.UtcNow.AddMinutes(30),
                IsRevoked = false
            };
        }

        public async Task<string> GenerateAccessToken(AppUserModel User)
        {

            var userRoles = await _userManager.GetRolesAsync(User); // Gets the role(s) of the found user

            // Used roles to generate the access token
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, User.Id),
                new Claim(ClaimTypes.Email, User.Email ),
                new Claim(ClaimTypes.Name, User.UserName)
            }; //Gets the Claims of the found user

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            } // Adds the user's role(s) to the list of claims

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new Exception("Jwt key not found")));
            //Gets the private security key from appsettings via IConfiguration
            var lifetime = double.Parse(_configuration["Jwt:LifeTime"]);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: authClaims,
                expires: DateTime.Now.AddMinutes(lifetime),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            // Uses the Jwt-security-token-handler to write and return the newly constructed token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
