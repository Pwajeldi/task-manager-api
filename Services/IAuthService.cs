using Task_Management_App.DTOs;

namespace Task_Management_App.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginDto dto);
    }
}
