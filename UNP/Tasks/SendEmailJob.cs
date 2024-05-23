using Quartz;
using UNP.Services;

namespace UNP.Tasks
{
    public class SendEmailJob : IJob    
    {
        private readonly IEmailService _emailService;

        public SendEmailJob(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _emailService.SendEmails();
        }
    }
}
