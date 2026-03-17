using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Task_Management_App.Models;

namespace Task_Management_App.DataBase
{
    public class TaskDatabase: IdentityDbContext
    {
        public DbSet<TaskModel> TaskTable { get; set; }
        public TaskDatabase(DbContextOptions<TaskDatabase> options) : base(options)
        {
        }
    }
}
