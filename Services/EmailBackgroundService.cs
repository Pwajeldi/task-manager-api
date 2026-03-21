using Org.BouncyCastle.Tls;
using Task_Management_App.Models;

namespace Task_Management_App.Services
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IEmailQueue _emailQueue;
        private readonly IEmailSender _emailSender;

        public EmailBackgroundService(IEmailQueue emailQueue, IEmailSender emailSender)
        {
            _emailQueue = emailQueue;
            _emailSender = emailSender;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var email = await _emailQueue.Dequeue(stoppingToken);

                try { 
                    await _emailSender.SendEmailAsync("CorporaJ", "pwajeldi700@gmail.com", email);
                }
                catch (Exception ex)
                {
                    // Log the exception (not implemented here)
                    Console.WriteLine($"Failed to send email: {ex.Message}");
                }
            }
        }
    }
}
