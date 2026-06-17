using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using WebApi.Models;

namespace WebApi.Services
{
    public class JobService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;

        public JobService()
        {
            _schedulerFactory = new StdSchedulerFactory();
        }

        public async Task<IScheduler> GetSchedulerAsync()
        {
            if (_scheduler == null || _scheduler.IsShutdown)
            {
                _scheduler = await _schedulerFactory.GetScheduler();
                await _scheduler.Start();
            }
            return _scheduler;
        }

        public async Task<List<JobInfo>> GetAllJobsAsync()
        {
            var scheduler = await GetSchedulerAsync();
            var result = new List<JobInfo>();

            try
            {
                var executingJobs = await scheduler.GetCurrentlyExecutingJobs();
                foreach (var ctx in executingJobs)
                {
                    result.Add(new JobInfo
                    {
                        JobName = ctx.JobDetail.Key.Name,
                        Group = ctx.JobDetail.Key.Group,
                        Description = ctx.JobDetail.Description,
                        IsRunning = true,
                        NextFireTime = ctx.Trigger.GetNextFireTimeUtc()?.DateTime.ToLocalTime(),
                        PreviousFireTime = ctx.Trigger.GetPreviousFireTimeUtc()?.DateTime.ToLocalTime()
                    });
                }
            }
            catch { }

            return result;
        }

        public async Task ScheduleSimpleJobAsync(string jobName, int intervalSeconds, string description = null)
        {
            var scheduler = await GetSchedulerAsync();
            var job = JobBuilder.Create<SimpleJob>()
                .WithIdentity(jobName, "default")
                .WithDescription(description)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{jobName}_trigger", "default")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(intervalSeconds).RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public async Task PauseJobAsync(string jobName, string group = "default")
        {
            var scheduler = await GetSchedulerAsync();
            var jobKey = new JobKey(jobName, group);
            await scheduler.PauseJob(jobKey);
        }

        public async Task ResumeJobAsync(string jobName, string group = "default")
        {
            var scheduler = await GetSchedulerAsync();
            var jobKey = new JobKey(jobName, group);
            await scheduler.ResumeJob(jobKey);
        }

        public async Task DeleteJobAsync(string jobName, string group = "default")
        {
            var scheduler = await GetSchedulerAsync();
            var jobKey = new JobKey(jobName, group);
            await scheduler.DeleteJob(jobKey);
        }

        public async Task StopAsync()
        {
            if (_scheduler != null && !_scheduler.IsShutdown)
            {
                await _scheduler.Shutdown();
            }
        }
    }

    public class SimpleJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine($"[Job] {context.JobDetail.Key.Name} executed at {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}
