using Org.BouncyCastle.Tls;
using Task_Management_App.Models;

namespace Task_Management_App.Services
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IEmailQueue _emailQueue;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<EmailBackgroundService> _logger;

        public EmailBackgroundService(IEmailQueue emailQueue, IEmailSender emailSender, ILogger<EmailBackgroundService> logger)
        {
            _emailQueue = emailQueue;
            _emailSender = emailSender;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var email = await _emailQueue.Dequeue(stoppingToken);

                try { 
                    await _emailSender.SendEmailAsync("CorporaJ", "pwajeldi700@gmail.com", email);
                    _logger.LogInformation("Email sent to {receiverEmail}", email.receiverEmail);
                }
                catch (Exception ex)
                {
                    // Log the exception
                    _logger.LogError(ex,"Failed to send email");
                }
            }
        }
    }
}
