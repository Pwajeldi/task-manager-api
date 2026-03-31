using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Management_App.DataBase;
using Task_Management_App.DTOs;
using Task_Management_App.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Tls;

namespace Task_Management_App.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ILogger<TaskController> _logger;
            public TaskController(ILogger<TaskController> logger)
            {
                _logger = logger;
            }
        //Endpoint for creating tasks
        [HttpPost("CreateTask")]
        [Authorize]
        public async Task<IActionResult> CreateTask(CreateTaskdto dto, TaskDatabase context)
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Fetches the user's id from his claims within the jwt token

            var newTask = new TaskModel
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                UserId = userid,
            };
           


            await context.TaskTable.AddAsync(newTask); //Adds the new task to the database context without saving.
            await context.SaveChangesAsync(); // Saves the changes.
            _logger.LogInformation("New task added to database");

            return Ok(newTask);
        }

        // Endpoint to retrieve all tasks of the user
        [HttpGet("myTasks")]
        [Authorize]
        public async Task<IActionResult> GetUserTasks(TaskDatabase context)
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userTasks = await context.TaskTable.Where(task => task.UserId == userid).Select(task => new TaskResponsedto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CreatedDate = task.CreatedDate,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted
            }).ToListAsync();
            // Fetches the user,s tasks via the userId foregn key, and uses the dto to avoid displaying the Id.
            if (userTasks == null || userTasks.Count == 0)
            {
                return NotFound("You have no tasks saved.\n Create tasks and view them here!");
            }
            return Ok(userTasks);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask(TaskDatabase context, int id)
        {
            // find userid from jwt claims
            var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userid == null)
            {
                return Unauthorized("Cannot perform this action");
            }

            // find the ICollection Task where it matches with the userid.
            var task = await context.TaskTable.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userid);
            if (task == null)
            {
                return NotFound("Task not found or you do not have permission to delete this task.");
            }
            
            context.TaskTable.Remove(task);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }   
}
