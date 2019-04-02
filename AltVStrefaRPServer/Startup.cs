using System;
using System.IO;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Handlers;
using AltVStrefaRPServer.Modules.Character.Customization;
using AltVStrefaRPServer.Modules.Vehicle;
using AltVStrefaRPServer.Services.Character;
using AltVStrefaRPServer.Services.Character.Customization;
using AltVStrefaRPServer.Services.Vehicles;
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
            appSettings.Initialize();

            // Add database
            services.AddDbContext<ServerContext>(options =>
                options.UseMySql(appSettings.ConnectionString,
                    mysqlOptions =>
                    {
                        mysqlOptions.ServerVersion(new Version(10, 1, 37), ServerType.MariaDb);
                    }));

            // Add services
            services.AddTransient<ILogin, Login>();
            services.AddTransient<ICharacterCreatorService, CharacterCreatorService>();

            services.AddSingleton<IVehicleManagerService, VehicleManagerService>();
            services.AddSingleton<HashingService>();
            services.AddSingleton<PlayerConnect>();
            services.AddSingleton<PlayerDisconnect>();
            services.AddSingleton<VehicleManager>();

            services.AddTransient<CharacterCreator>();

            // Build provider
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
