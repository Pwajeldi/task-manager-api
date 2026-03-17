using System.ComponentModel.DataAnnotations;
using Task_Management_App.Validations;

namespace Task_Management_App.DTOs
{
    public class CreateTaskdto
    {
        [Required][StringLength(40)]public string? Title { get; set; }
        [Required][StringLength(500)]public string? Description { get; set; }
        [FutureDate]public DateTime DueDate { get; set; }
    }
}
