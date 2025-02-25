using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzNetCronExpressions
{
    using Quartz;
    using Quartz.Impl;

    class Program
    {
        //public static StdSchedulerFactory SchedulerFactory;
        //public static IScheduler Scheduler;
        //public static ITrigger ImageTrigger;
        //public static ITrigger SecondTrigger;

        static void Main(string[] args)
        {

            //SchedulerFactory = new StdSchedulerFactory();
            //Scheduler = SchedulerFactory.GetScheduler();

            //Scheduler.Start();

            Test001();
        }

        private static void Test001()
        {
            //var cron = new Quartz.CronExpression("10 * * * * ?");
            var cron = new Quartz.CronExpression("* 5 18 ? * 1");
            var date = DateTime.Now;
            DateTimeOffset? nextFire = cron.GetNextValidTimeAfter(date);
            // Log the cron expression, current date, and next fire time
            Console.WriteLine($"Cron Expression: {cron}");
            Console.WriteLine($"Current Date: {date}");
            Console.WriteLine($"Next Fire: {nextFire}");
        }

        //private static void Test002(IScheduler Scheduler)
        //{
        //    IJobDetail job = JobBuilder.Create<SimpleJob>()
        //                               .WithIdentity("job1")
        //                               .Build();

        //    ITrigger trigger = TriggerBuilder.Create()
        //                                     .WithIdentity("trigger1")
        //                                     .StartNow()
        //                                     .WithCronSchedule("0 * 8-22 * * ?")
        //                                     .Build();

        //    Scheduler.ScheduleJob(job, trigger);

        //}
    }
}
