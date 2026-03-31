using System.ComponentModel.DataAnnotations;

namespace Task_Management_App.DTOs
{
    public class refreshRequestDto
    {
        [Required]public required string accessToken {  get; set; }
        [Required]public required string refreshToken { get; set; }
    }
}
