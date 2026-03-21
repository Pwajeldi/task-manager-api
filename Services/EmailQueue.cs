using System.Collections;
using System.Threading.Channels;
using Task_Management_App.Models;

namespace Task_Management_App.Services
{
    public class EmailQueue : IEmailQueue
    {
        private readonly Channel<EmailRequestMod> _queue;

        public EmailQueue()
        {
            _queue = Channel.CreateUnbounded<EmailRequestMod>();
        }

        public void EnqueueEmail(EmailRequestMod email)
        {
            _queue.Writer.TryWrite(email);
        }

        public async Task<EmailRequestMod> Dequeue(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
