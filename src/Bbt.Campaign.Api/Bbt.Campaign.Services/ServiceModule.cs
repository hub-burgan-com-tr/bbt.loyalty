﻿using Bbt.Campaign.EntityFrameworkCore;
using Bbt.Campaign.Shared.ServiceDependencies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bbt.Campaign.Services
{
    public static class ServiceModule
    {
        public static void Configure(IConfiguration configuration, IServiceCollection services)
        {
            IocLoader.UseIocLoader(services);
            EntityFrameworkCoreModule.Configure(configuration, services);
            services.AddAutoMapper(typeof(ServiceModule));
        }

        //public void ConfigureQuartz(IServiceCollection services)
        //{
        //    // base configuration from appsettings.json
        //    services.Configure<QuartzOptions>(Configuration.GetSection("Quartz"));

        //    // if you are using persistent job store, you might want to alter some options
        //    services.Configure<QuartzOptions>(options =>
        //    {
        //        options.Scheduling.IgnoreDuplicates = true; // default: false
        //        options.Scheduling.OverWriteExistingData = true; // default: true
        //    });

        //    services.AddQuartz(q =>
        //    {
        //        // handy when part of cluster or you want to otherwise identify multiple schedulers
        //        q.SchedulerId = "Scheduler-Core";

        //        // we take this from appsettings.json, just show it's possible
        //        // q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";

        //        // as of 3.3.2 this also injects scoped services (like EF DbContext) without problems
        //        q.UseMicrosoftDependencyInjectionJobFactory();

        //        // or for scoped service support like EF Core DbContext
        //        // q.UseMicrosoftDependencyInjectionScopedJobFactory();

        //        // these are the defaults
        //        q.UseSimpleTypeLoader();
        //        q.UseInMemoryStore();
        //        q.UseDefaultThreadPool(tp =>
        //        {
        //            tp.MaxConcurrency = 10;
        //        });

        //        // quickest way to create a job with single trigger is to use ScheduleJob
        //        // (requires version 3.2)
        //        q.ScheduleJob<ExampleJob>(trigger => trigger
        //            .WithIdentity("Combined Configuration Trigger")
        //            .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(7)))
        //            .WithDailyTimeIntervalSchedule(x => x.WithInterval(10, IntervalUnit.Second))
        //            .WithDescription("my awesome trigger configured for a job with single call")
        //        );

        //        // you can also configure individual jobs and triggers with code
        //        // this allows you to associated multiple triggers with same job
        //        // (if you want to have different job data map per trigger for example)
        //        q.AddJob<ExampleJob>(j => j
        //            .StoreDurably() // we need to store durably if no trigger is associated
        //            .WithDescription("my awesome job")
        //        );

        //        // here's a known job for triggers
        //        var jobKey = new JobKey("awesome job", "awesome group");
        //        q.AddJob<ExampleJob>(jobKey, j => j
        //            .WithDescription("my awesome job")
        //        );

        //        q.AddTrigger(t => t
        //            .WithIdentity("Simple Trigger")
        //            .ForJob(jobKey)
        //            .StartNow()
        //            .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(10)).RepeatForever())
        //            .WithDescription("my awesome simple trigger")
        //        );

        //        q.AddTrigger(t => t
        //            .WithIdentity("Cron Trigger")
        //            .ForJob(jobKey)
        //            .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(3)))
        //            .WithCronSchedule("0/3 * * * * ?")
        //            .WithDescription("my awesome cron trigger")
        //        );

        //        // you can add calendars too (requires version 3.2)
        //        const string calendarName = "myHolidayCalendar";
        //        q.AddCalendar<HolidayCalendar>(
        //            name: calendarName,
        //            replace: true,
        //            updateTriggers: true,
        //            x => x.AddExcludedDate(new DateTime(2020, 5, 15))
        //        );

        //        q.AddTrigger(t => t
        //            .WithIdentity("Daily Trigger")
        //            .ForJob(jobKey)
        //            .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(5)))
        //            .WithDailyTimeIntervalSchedule(x => x.WithInterval(10, IntervalUnit.Second))
        //            .WithDescription("my awesome daily time interval trigger")
        //            .ModifiedByCalendar(calendarName)
        //        );

        //        // also add XML configuration and poll it for changes
        //        q.UseXmlSchedulingConfiguration(x =>
        //        {
        //            x.Files = new[] { "~/quartz_jobs.config" };
        //            x.ScanInterval = TimeSpan.FromSeconds(2);
        //            x.FailOnFileNotFound = true;
        //            x.FailOnSchedulingError = true;
        //        });

        //        // convert time zones using converter that can handle Windows/Linux differences
        //        q.UseTimeZoneConverter();

        //        // auto-interrupt long-running job
        //        q.UseJobAutoInterrupt(options =>
        //        {
        //            // this is the default
        //            options.DefaultMaxRunTime = TimeSpan.FromMinutes(5);
        //        });
        //        q.ScheduleJob<SlowJob>(
        //            triggerConfigurator => triggerConfigurator
        //                .WithIdentity("slowJobTrigger")
        //                .StartNow()
        //                .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever()),
        //            jobConfigurator => jobConfigurator
        //                .WithIdentity("slowJob")
        //                .UsingJobData(JobInterruptMonitorPlugin.JobDataMapKeyAutoInterruptable, true)
        //                // allow only five seconds for this job, overriding default configuration
        //                .UsingJobData(JobInterruptMonitorPlugin.JobDataMapKeyMaxRunTime, TimeSpan.FromSeconds(5).TotalMilliseconds.ToString(CultureInfo.InvariantCulture)));

        //        // add some listeners
        //        q.AddSchedulerListener<SampleSchedulerListener>();
        //        q.AddJobListener<SampleJobListener>(GroupMatcher<JobKey>.GroupEquals(jobKey.Group));
        //        q.AddTriggerListener<SampleTriggerListener>();

        //        // example of persistent job store using JSON serializer as an example
        //        /*
        //        q.UsePersistentStore(s =>
        //        {
        //            s.UseProperties = true;
        //            s.RetryInterval = TimeSpan.FromSeconds(15);
        //            s.UseSqlServer(sqlServer =>
        //            {
        //                sqlServer.ConnectionString = "some connection string";
        //                // this is the default
        //                sqlServer.TablePrefix = "QRTZ_";
        //            });
        //            s.UseJsonSerializer();
        //            s.UseClustering(c =>
        //            {
        //                c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
        //                c.CheckinInterval = TimeSpan.FromSeconds(10);
        //            });
        //        });
        //        */
        //    });

        //    // we can use options pattern to support hooking your own configuration
        //    // because we don't use service registration api, 
        //    // we need to manually ensure the job is present in DI
        //    services.AddTransient<ExampleJob>();

        //    services.Configure<SampleOptions>(Configuration.GetSection("Sample"));
        //    services.AddOptions<QuartzOptions>()
        //        .Configure<IOptions<SampleOptions>>((options, dep) =>
        //        {
        //            if (!string.IsNullOrWhiteSpace(dep.Value.CronSchedule))
        //            {
        //                var jobKey = new JobKey("options-custom-job", "custom");
        //                options.AddJob<ExampleJob>(j => j.WithIdentity(jobKey));
        //                options.AddTrigger(trigger => trigger
        //                    .WithIdentity("options-custom-trigger", "custom")
        //                    .ForJob(jobKey)
        //                    .WithCronSchedule(dep.Value.CronSchedule));
        //            }
        //        });

        //    // Quartz.Extensions.Hosting allows you to fire background service that handles scheduler lifecycle
        //    services.AddQuartzHostedService(options =>
        //    {
        //        // when shutting down we want jobs to complete gracefully
        //        options.WaitForJobsToComplete = true;
        //    });
        //}
    }
}
