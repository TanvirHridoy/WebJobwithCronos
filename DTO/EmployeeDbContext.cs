using Microsoft.EntityFrameworkCore;

namespace ServiceWorkerCronJobDemo.DTO;

public class EmployeeDbContext : DbContext
{

    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
               : base(options)
    {
    }

    public DbSet<WebJobLog> WebJobLogs { get; set; }
}