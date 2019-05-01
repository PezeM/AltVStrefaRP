using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace AltVStrefaRPServer.Database
{
    //public class DesignTimeServerContextFactory : IDesignTimeDbContextFactory<ServerContext>
    //{
    //    public ServerContext CreateDbContext(string[] args)
    //    {
    //        // Configurations
    //        IConfiguration configuration = new ConfigurationBuilder()
    //            .SetBasePath(Directory.GetCurrentDirectory())
    //            .AddJsonFile("appsettings.json", false)
    //            .Build();

    //        var builder = new DbContextOptionsBuilder<ServerContext>();

    //        builder.UseMySql(configuration["ConnectionString"],
    //            mysqlOptions =>
    //            {
    //                mysqlOptions.ServerVersion(new Version(10, 1, 37), ServerType.MariaDb);
    //            });

    //        return new ServerContext(builder.Options);
    //    }
    //}
}
