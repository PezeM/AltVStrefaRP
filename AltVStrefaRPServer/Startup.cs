using System;
using System.IO;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Handlers;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Server;
using AltVStrefaRPServer.Modules.Admin;
using AltVStrefaRPServer.Modules.Businesses;
using AltVStrefaRPServer.Modules.CharacterModule.Customization;
using AltVStrefaRPServer.Modules.Chat;
using AltVStrefaRPServer.Modules.Environment;
using AltVStrefaRPServer.Modules.Fractions;
using AltVStrefaRPServer.Modules.Inventory;
using AltVStrefaRPServer.Modules.Money;
using AltVStrefaRPServer.Modules.Networking;
using AltVStrefaRPServer.Modules.Vehicle;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Businesses;
using AltVStrefaRPServer.Services.Characters;
using AltVStrefaRPServer.Services.Characters.Accounts;
using AltVStrefaRPServer.Services.Characters.Customization;
using AltVStrefaRPServer.Services.Fractions;
using AltVStrefaRPServer.Services.Inventory;
using AltVStrefaRPServer.Services.Money;
using AltVStrefaRPServer.Services.Money.Bank;
using AltVStrefaRPServer.Services.Vehicles;
using AltVStrefaRPServer.Services.Vehicles.VehicleShops;
using EFCore.DbContextFactory.Extensions;
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
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "resources", "AltVStrefaRPServer"))
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
            //services.AddDbContext<ServerContext>(options =>
            //    options.UseMySql(appSettings.ConnectionString,
            //        mysqlOptions =>
            //        {
            //            mysqlOptions.ServerVersion(new Version(10, 1, 37), ServerType.MariaDb);
            //        }));
            
            services.AddDbContextFactory<ServerContext>(options => 
                options.UseMySql(appSettings.ConnectionString, mysqlOptions =>
                {
                    mysqlOptions.ServerVersion(new Version(10, 1, 37), ServerType.MariaDb);
                }));

            // Add services
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<ILogin, Login>();
            services.AddTransient<IAccountDatabaseService, AccountDatabaseService>();
            services.AddTransient<ICharacterCreatorService, CharacterCreatorService>();
            services.AddTransient<ICharacterDatabaseService, CharacterDatabaseService>();
            services.AddTransient<IMoneyService, MoneyService>();
            services.AddTransient<IBankAccountDatabaseService, BankAccountDatabaseService>();
            services.AddTransient<ITaxService, TaxService>();
            services.AddTransient<IBusinessService, BusinessService>();
            services.AddTransient<IBusinessDatabaseService, BusinessDatabaseService>();
            services.AddTransient<IVehicleSpawnService, VehicleSpawnService>();
            services.AddTransient<IVehicleCreatorService, VehicleCreatorService>();
            services.AddTransient<IVehicleDatabaseService, VehicleDatabaseService>();
            services.AddTransient<IVehicleShopDatabaseService, VehicleShopDatabaseService>();
            services.AddTransient<IVehicleShopsFactory, VehicleShopsFactory>();
            services.AddTransient<IFractionDatabaseService, FractionDatabaseService>();
            services.AddTransient<IFractionFactoryService, FractionFactoryService>();
            services.AddTransient<IInventoryDatabaseService, InventoryDatabaseService>();

            services.AddTransient<PlayerConnect>();
            services.AddTransient<PlayerDisconnect>();
            services.AddTransient<VehicleHandler>();
            services.AddTransient<ObjectSync>();

            services.AddSingleton<HashingService>();
            services.AddSingleton<IVehiclesManager, VehiclesManager>();
            services.AddSingleton<BankHandler>();
            services.AddSingleton<BankAccountManager>();
            services.AddSingleton<BusinessesManager>();
            services.AddSingleton<BusinessHandler>();
            services.AddSingleton<TemporaryChatHandler>();
            services.AddSingleton<TimeController>();
            services.AddSingleton<VehicleShopsManager>();
            services.AddSingleton<IFractionsManager, FractionsManager>();
            services.AddSingleton<FractionHandler>();
            services.AddSingleton<InventoriesManager>();
            services.AddSingleton<InventoryHandler>();

            services.AddTransient<AdminCommands>();
            services.AddTransient<CharacterCreator>();
            services.AddTransient<SittingHandler>();
            services.AddTransient<TrashbinsController>();
            services.AddTransient<VehicleShopsHandler>();
            services.AddTransient<TownHallFractionHandler>();
            services.AddTransient<ItemFactory>();

            services.AddSingleton<SerializatorTest>();
            services.AddSingleton<NetworkingManager>();

            // Build provider
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
