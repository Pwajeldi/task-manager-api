using Microsoft.EntityFrameworkCore;
using Task_Management_App.Models;

namespace Task_Management_App.DataBase
{
    public class TaskDatabase: DbContext
    {
        public DbSet<TaskModel> TaskTable { get; set; }
    }
}
