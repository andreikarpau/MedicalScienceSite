using System;
using Quartz;
using Quartz.Impl;

namespace BTTechnologies.MedScience.MVC.Infrastructure.Sheduler
{
    public class QuartzScheduler
    {
        private readonly IScheduler scheduler;
        private bool started;
        private static QuartzScheduler shedulerObject;
        private static readonly object LockObj = new object();

        public static QuartzScheduler Current
        {
            get
            {
                lock (LockObj)
                {
                    return shedulerObject ?? (shedulerObject = new QuartzScheduler());
                }
            }
        }

        private QuartzScheduler()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler();
        }

        public void Run()
        {
            if (started)
                return;

            IJobDetail job = JobBuilder.Create<SchedulerJob>().WithIdentity("WeeklyJob", "group1").Build();
            ITrigger trigger = TriggerBuilder.Create().WithIdentity("WeeklyTrigger", "group1").StartNow().WithSimpleSchedule(x => x.WithInterval(new TimeSpan(1, 1, 0, 0)).RepeatForever()).Build();
            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();
            started = true;
        }

        public void Stop()
        {
            scheduler.Shutdown(true);
            started = false;
        }
    }
}