using Task_Management_App.Models;

namespace Task_Management_App.Services
{
    public interface IEmailSender
    {
       Task SendEmailAsync(string senderName, string senderEmail, EmailRequestMod emailrequest);
    }
}
