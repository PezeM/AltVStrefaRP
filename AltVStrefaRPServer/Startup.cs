using System;
using System.IO;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Handlers;
using AltVStrefaRPServer.Services.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace AltVStrefaRPServer
{
    /// <summary>
    /// Class for defining DI
    /// </summary>
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public ServiceProvider ServiceProvider { get; set; }

        public Startup()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Configurations
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            // Add reading settings from json
            //services.AddOptions();
            //services.Configure<AppSettings>(Configuration);
            var appSettings = new AppSettings();
            Configuration.Bind(appSettings);
            services.AddSingleton(appSettings);

            // Add database
            services.AddDbContext<ServerContext>(options =>
                options.UseMySql(appSettings.ConnectionString,
                    mysqlOptions =>
                    {
                        mysqlOptions.ServerVersion(new Version(10, 1, 37), ServerType.MariaDb);
                    }));
            // Add services
            services.AddTransient<ILogin, Login>();
            services.AddSingleton<HashingService>();
            services.AddSingleton<PlayerConnect>();

            // Build provider
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
