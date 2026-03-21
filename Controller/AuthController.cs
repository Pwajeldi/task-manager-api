using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Task_Management_App.DTOs;
using Task_Management_App.Migrations;
using Task_Management_App.Models;
using Task_Management_App.Services;

namespace Task_Management_App.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<AppUserModel> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailQueue _emailQueue;

        public AuthController(IAuthService authService, UserManager<AppUserModel> userManager, IEmailSender emailSender, IEmailQueue emailQueue)
        {
            _authService = authService;
            _userManager = userManager;
            _emailSender = emailSender;
            _emailQueue = emailQueue;
        } //Dependency Injection of the IAuthService via the constructor


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            // Runs the implementes method and returns the generated token after login
            return Ok(new { token });
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user != null)
            {
                return BadRequest("User with this email already exists.");
            }

            var appUser = new AppUserModel
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CreatedDate = DateTime.Now
            };

            var RegUser = await _userManager.CreateAsync(appUser, dto.Password);
            await _userManager.AddToRoleAsync(appUser, "User");
            return Ok(new { RegUser });

        }

        [HttpPost("send-confirmation-email")]
        public async Task<IActionResult> SendConfirmationEmail(ConfirmPasswordDto dto)
        { // Confirming the user
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) { return BadRequest("Invalid user"); }

            var userid = user.Id;   //Getting the Id of the confirmed user
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);  // Generate token for the user's email
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));  //Web-Encode the token
            var link = $"http://localhost:5107/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";


            try
            {
                _emailQueue.EnqueueEmail(new EmailRequestMod
                {
                    receiverEmail = user.Email,
                    receiverName = user.FirstName + " " + user.LastName,
                    subject = "New Sign-in Confirmation",
                    content = $"Go to this link to verify your email \n {link}"
                });

                return Ok($"Email with confirmation link sent to {user.Email}");
            }
            catch (Exception ex) { return BadRequest("Error occured while sending email, please try again later"); }
        }


        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) { return BadRequest("Invalid user");}

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
            {
                return BadRequest("Invalid or expired token");
            }
            return Ok($"Your email {user.Email} has been confirmed succcessfully");
        }
    }

}