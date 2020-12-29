using Microsoft.EntityFrameworkCore;

namespace ACL_Web_Proxy.Model
{
    public class EmployeeDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Log> Logs { get; set; }

        public EmployeeDbContext()
        {
            try
            {
                this.Database.EnsureCreated();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($"Server={DefaultValues.DbServer}; Database={DefaultValues.DbName}; Trusted_Connection=True;");
        }
    }
}
