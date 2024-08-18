using Microsoft.EntityFrameworkCore;
using ServiceWorkerCronJobDemo.DTO;
using System.ComponentModel.DataAnnotations;

namespace ServiceWorkerCronJobDemo.Services
{
    public class MyCronJob1 : CronJobService
    {
        private readonly ILogger<MyCronJob1> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MyCronJob1(IScheduleConfig<MyCronJob1> config, ILogger<MyCronJob1> logger, IServiceScopeFactory serviceScopeFactory)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob 1 starts.");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{now} CronJob 1 is working.", DateTime.Now.ToString("T"));

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();

                try
                {
                    await _db.WebJobLogs.AddAsync(new WebJobLog { Id = 0, Log = $"{DateTime.Now:T} CronJob 1 is working.", TimeStamp = DateTime.Now });
                    await _db.SaveChangesAsync();

                    for (int i = 1; i <= 120; i++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            _logger.LogInformation("CronJob 1 is being cancelled.");
                            await _db.WebJobLogs.AddAsync(new WebJobLog { Id = 0, Log = "CronJob 1 is being cancelled.", TimeStamp = DateTime.Now });
                            await _db.SaveChangesAsync();
                            break;
                        }

                        _logger.LogInformation("Elapsed Time: {seconds} seconds.", i);
                        await _db.WebJobLogs.AddAsync(new WebJobLog { Id = 0, Log = $"Elapsed Time: {i} seconds.", TimeStamp = DateTime.Now });
                        await _db.SaveChangesAsync();
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    }

                    _logger.LogInformation("{now} CronJob 1 finished work.", DateTime.Now.ToString("T"));
                    await _db.WebJobLogs.AddAsync(new WebJobLog { Id = 0, Log = $"{DateTime.Now:T} CronJob 1 finished work.", TimeStamp = DateTime.Now });
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while executing CronJob 1.");
                    throw; // Optionally rethrow if you need to propagate the error
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob 1 is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }


}
