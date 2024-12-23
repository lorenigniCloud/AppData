using AppData.Infrastructures.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Infrastructures.Models;

public static class AppDataSqlServerContextExtensions
{
    public static IServiceCollection AddAppDataSqlServerContext(this IServiceCollection services, IConfiguration configuration)
    {
        string? connection = configuration["ConnectionStrings:DefaultConnection"];

        services.AddDbContext<AppDataSqlServerContext>(options =>
            options.UseSqlServer(connection, b => b.MigrationsAssembly("AppData.Migrations"))
        );
        return services;
    }
}


