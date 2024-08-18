namespace ServiceWorkerCronJobDemo.Services;

public class MyJobWithSerilog : CronJobService
{
    private readonly ILogger<MyJobWithSerilog> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MyJobWithSerilog(IScheduleConfig<MyJobWithSerilog> config, ILogger<MyJobWithSerilog> logger, IServiceScopeFactory serviceScopeFactory)
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

        try
        {
            _logger.LogInformation($"{DateTime.Now:T} CronJob 1 is working.");

            for (int i = 1; i <= 120; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("CronJob 1 is being cancelled.");
                    break;
                }

                _logger.LogInformation("Elapsed Time: {seconds} seconds.", i);
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }

            _logger.LogInformation("{now} CronJob 1 finished work.", DateTime.Now.ToString("T"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while executing CronJob 1.");
            throw; // Optionally rethrow if you need to propagate the error
        }
        //}
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("CronJob 1 is stopping.");
        return base.StopAsync(cancellationToken);
    }
}