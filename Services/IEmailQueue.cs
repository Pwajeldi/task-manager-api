using Microsoft.AspNetCore.Identity.Data;
using Task_Management_App.Models;

namespace Task_Management_App.Services
{
    public interface IEmailQueue
    {
        void EnqueueEmail(EmailRequestMod email);
        Task<EmailRequestMod> Dequeue(CancellationToken cancellationToken);
    }
}
