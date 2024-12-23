using AppData.Infrastructures.Models;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;


namespace AppData.Migrations
{
    public class AppDataSqlServerContextFactory : IDesignTimeDbContextFactory<AppDataSqlServerContext>
    {
        public AppDataSqlServerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDataSqlServerContext>();
            optionsBuilder.UseSqlServer("Your_Connection_String");

            return new AppDataSqlServerContext(optionsBuilder.Options);
        }
    }

}
