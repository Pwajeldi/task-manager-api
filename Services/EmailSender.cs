using MimeKit;
using Task_Management_App.Models;
using MailKit;
using MailKit.Net.Smtp;

namespace Task_Management_App.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
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
                var primaryport = int.Parse(_configuration["SmtpSettings:Port1"]);
                var Fallbackport = int.Parse(_configuration["SmtpSettings:Port2"]);
                var server = _configuration["SmtpSettings:Server"];
                var username = _configuration["SmtpSettings:Login"];
                var password = _configuration["SmtpSettings:Password"];
                try
                {
                    Console.WriteLine("connecting...");
                    client.Connect(server, primaryport, false);
                    Console.WriteLine($"Connected to SMTP server via port {primaryport}");
                }
                catch (Exception ex) { 
                    Console.WriteLine($"Error connecting via port {primaryport} : {ex.Message}" +
                    $" \n Connecting via port {Fallbackport}...");
                    client.Connect(server, Fallbackport, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    Console.WriteLine($"Connected to SMTP server via port {Fallbackport}");
                }

                // Establishes a secure connection to the SMTP server


                // Authenticate with the SMTP server(!! only needed if the server requires authentication)
               
                try
                {
                    Console.WriteLine("Authenticating..");
                    client.Authenticate(username, password);
                    Console.WriteLine("Authentication succesfull");
                }
                catch (Exception ex) { Console.WriteLine($"Error authenticating with SMTP server: {ex.Message}"); }


                // Send the email message
                try
                {
                    Console.WriteLine("Sending email...");
                    client.Send(message);
                    Console.WriteLine("Mail sent to inbox");
                }
                catch (Exception ex) { Console.WriteLine($"Error sending email: {ex.Message}"); }


                // Disconnect from the SMTP server
                try
                {
                    Console.WriteLine("Disconnecting...");
                    client.Disconnect(true);
                    Console.WriteLine("Disconnected");
                }
                catch (Exception ex) { Console.WriteLine($"Error disconnecting from SMTP server: {ex.Message}"); }
            }
        }
    }
}
