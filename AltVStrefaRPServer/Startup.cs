using System;
using System.IO;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Handlers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Modules.Admin;
using AltVStrefaRPServer.Modules.Businesses;
using AltVStrefaRPServer.Modules.CharacterModule.Customization;
using AltVStrefaRPServer.Modules.Chat;
using AltVStrefaRPServer.Modules.Environment;
using AltVStrefaRPServer.Modules.Money;
using AltVStrefaRPServer.Modules.Vehicle;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Businesses;
using AltVStrefaRPServer.Services.Characters;
using AltVStrefaRPServer.Services.Characters.Customization;
using AltVStrefaRPServer.Services.Money;
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
                    }), ServiceLifetime.Scoped);

            // Add services
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<ILogin, Login>();
            services.AddTransient<ICharacterCreatorService, CharacterCreatorService>();
            services.AddTransient<ICharacterDatabaseService, CharacterDatabaseService>();
            services.AddTransient<IVehicleCreatorService, VehicleCreatorService>();
            services.AddTransient<IMoneyService, MoneyService>();
            services.AddTransient<IBusinessService, BusinessService>();
            services.AddTransient<IVehicleDatabaseService, VehicleDatabaseService>();


            services.AddTransient<PlayerConnect>();
            services.AddTransient<PlayerDisconnect>();
            services.AddTransient<VehicleHandler>();
            services.AddTransient<ObjectSync>();

            services.AddSingleton<HashingService>();
            services.AddSingleton<VehicleManager>();
            services.AddSingleton<BankHandler>();
            services.AddSingleton<BankAccountManager>();
            services.AddSingleton<BusinessManager>();
            services.AddSingleton<BusinessHandler>();
            services.AddSingleton<TemporaryChatHandler>();
            services.AddSingleton<TimeManager>();

            services.AddTransient<AdminCommands>();
            services.AddTransient<CharacterCreator>();
            services.AddTransient<SittingHandler>();
            services.AddTransient<TrashBinsHandler>();

            // Build provider
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
