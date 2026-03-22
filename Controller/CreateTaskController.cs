using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Management_App.DataBase;
using Task_Management_App.DTOs;
using Task_Management_App.Models;

namespace Task_Management_App.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateTaskController : ControllerBase
    {
        private readonly ILogger<CreateTaskController> _logger;
            public CreateTaskController(ILogger<CreateTaskController> logger)
            {
                _logger = logger;
            }
        [HttpPost]
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


            await context.TaskTable.AddAsync(newTask);
            await context.SaveChangesAsync();
            _logger.LogInformation("New task added to database");

            return Ok(newTask);
        }
    }
}
