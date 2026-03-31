using Microsoft.AspNetCore.Mvc;
using Task_Management_App.DTOs;
using Task_Management_App.Models;

namespace Task_Management_App.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task<string> GenerateAccessToken(AppUserModel User);
        RefreshTokenMod GenerateRefreshToken(string userId);
    }
}
