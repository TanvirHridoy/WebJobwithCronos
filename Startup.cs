using Serilog.Events;
using Serilog;
using ServiceWorkerCronJobDemo.Services;
using System.Globalization;
using Serilog.Core;
using Microsoft.EntityFrameworkCore;
using ServiceWorkerCronJobDemo.DTO;

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

            var levelSwitch = new LoggingLevelSwitch();
            levelSwitch.MinimumLevel = LogEventLevel.Verbose;

            var filePath = $"{AppDomain.CurrentDomain.BaseDirectory}\\runlogs\\WebCronJob_log.txt";
            var logConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("OriginSystem", typeof(Program).Namespace)
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.File(filePath, LogEventLevel.Verbose, "", CultureInfo.InvariantCulture);
            Log.Logger = logConfiguration.CreateLogger();
            services.AddLogging(x => x.AddSerilog());

            //sql server
            services.AddDbContext<EmployeeDbContext>(o => o.UseSqlServer(Configuration.GetConnectionString("EmployeeDb")));

            services.AddScoped<IMyScopedService, MyScopedService>();

            services.AddCronJob<MyCronJob1>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = "*/1 * * * *";
            });
            // MyCronJob2 calls the scoped service MyScopedService
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
