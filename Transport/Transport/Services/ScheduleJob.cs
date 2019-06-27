using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Transport.Services
{
    public class ScheduleJob : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential("carryontransport@gmail.com", "carryon1")
            };

            using (var message = new MailMessage("carryontransport@gmail.com", "lora.zulicek@fer.hr")
            {
                Subject = "Uspješna registracija",
                Body = "Uspješno ste se registrirali."
            })
            {
                 smtpClient.SendMailAsync(message);
            }
        }

        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<ScheduleJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartNow()
             .WithCronSchedule("0 0/1 * 1/1 * ? *")
            .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }  
}
