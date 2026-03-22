using System.ComponentModel.DataAnnotations;
using Task_Management_App.Validations;

namespace Task_Management_App.DTOs
{
    public class CreateTaskdto
    {
        [Required][StringLength(40)]public required string Title { get; set; }
        [Required][StringLength(500)]public required string Description { get; set; }
        [Required][FutureDate]public required DateTime DueDate { get; set; }
    }
}
