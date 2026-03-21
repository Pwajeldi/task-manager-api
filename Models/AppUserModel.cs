using Microsoft.AspNetCore.Identity;

namespace Task_Management_App.Models
{
    public class AppUserModel: IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<TaskModel>? Tasks { get; set; }

    }
}
