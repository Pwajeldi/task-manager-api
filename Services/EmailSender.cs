using MimeKit;
using Task_Management_App.Models;
using MailKit;
using MailKit.Net.Smtp;

namespace Task_Management_App.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string senderName, string senderEmail, EmailRequestMod emailrequest)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress(emailrequest.receiverName, emailrequest.receiverEmail));
            message.Subject = emailrequest.subject;
            message.Body = new TextPart("plain")
            {
                Text = emailrequest.content
            };

            using (var client = new SmtpClient())
            { 
                var primaryport = int.Parse(_configuration["SmtpSettings:Port1"] ?? throw new Exception("Port number missing"));
                var Fallbackport = int.Parse(_configuration["SmtpSettings:Port2"] ?? throw new Exception("Port number missing"));
                var server = _configuration["SmtpSettings:Server"];
                var username = _configuration["SmtpSettings:Login"];
                var password = _configuration["SmtpSettings:Password"];
                try
                {
                    client.Connect(server, primaryport, false);
                    _logger.LogInformation("Connected to SMTP server via port {primaryport}", primaryport);
                }
                catch (Exception ex) { 
                    _logger.LogWarning(ex,"Error connecting via port {primaryport}, Connecting via port {Fallbackport}...", primaryport, Fallbackport);


                    client.Connect(server, Fallbackport, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    _logger.LogInformation("Connected to SMTP server via port {Fallbackport}", Fallbackport);
                }

                // Establishes a secure connection to the SMTP server


                // Authenticate with the SMTP server(!! only needed if the server requires authentication)
               
                try
                {
                    _logger.LogInformation("Authenticating..");
                    client.Authenticate(username, password);
                    _logger.LogInformation("Authentication succesfull");
                }
                catch (Exception ex) { _logger.LogError(ex,"Error authenticating with SMTP server"); }


                // Send the email message
                try
                {
                    client.Send(message);
                    _logger.LogInformation("Mail sent to inbox");
                }
                catch (Exception ex) { _logger.LogError(ex, "Error sending email"); }


                // Disconnect from the SMTP server
                try
                {
                    client.Disconnect(true);
                    _logger.LogInformation("Disconnected");
                }
                catch (Exception ex) { _logger.LogError(ex, "Error disconnecting from SMTP server"); }
            }
        }
    }
}
