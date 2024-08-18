using Serilog;
using Serilog.Core;
using Serilog.Events;
using ServiceWorkerCronJobDemo.Services;
using System.Globalization;

namespace ServiceWorkerCronJobDemo
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //sql server
            //services.AddDbContext<EmployeeDbContext>(o => o.UseSqlServer(Configuration.GetConnectionString("EmployeeDb")));

            var levelSwitch = new LoggingLevelSwitch();
            levelSwitch.MinimumLevel = LogEventLevel.Verbose;

            var filePath = $"{AppDomain.CurrentDomain.BaseDirectory}\\runlogs\\WebCronJob_log.txt";
            var logConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("OriginSystem", typeof(Program).Namespace)
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.File(filePath, LogEventLevel.Verbose,
                    rollingInterval: RollingInterval.Day, // Optional: create a new file daily
                    fileSizeLimitBytes: 10_000_000, // Optional: limit file size (10 MB)
                    retainedFileCountLimit: 10, // Optional: keep 10 files
                    formatProvider: CultureInfo.InvariantCulture
                );
            Log.Logger = logConfiguration.CreateLogger();
            services.AddLogging(x => x.AddSerilog());


            services.AddScoped<IMyScopedService, MyScopedService>();

            services.AddCronJob<MyJobWithSerilog>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = "*/1 * * * *";
            });
            //MyCronJob2 calls the scoped service MyScopedService
            //services.AddCronJob<MyCronJob2>(c =>
            //{
            //    c.TimeZoneInfo = TimeZoneInfo.Local;
            //    c.CronExpression = "* * * * *";
            //});
            //services.AddCronJob<MyCronJob3>(c =>
            //{
            //    c.TimeZoneInfo = TimeZoneInfo.Local;
            //    c.CronExpression = "50 12 * * *";
            //});
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
