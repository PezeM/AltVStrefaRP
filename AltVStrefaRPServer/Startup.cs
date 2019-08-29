using AltVStrefaRPServer.Data;
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
using AltVStrefaRPServer.Services.Inventories;
using AltVStrefaRPServer.Services.Money;
using AltVStrefaRPServer.Services.Money.Bank;
using AltVStrefaRPServer.Services.Vehicles;
using AltVStrefaRPServer.Services.Vehicles.VehicleShops;
using EFCore.DbContextFactory.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;
using System.IO;
using AltVStrefaRPServer.Modules.Core;
using AltVStrefaRPServer.Modules.HousingModule;
using AltVStrefaRPServer.Services.Housing;
using AltVStrefaRPServer.Services.Housing.Factories;

namespace AltVStrefaRPServer
{
    /// <summary>
    /// Class for defining DI
    /// </summary>
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public ServiceProvider ServiceProvider { get; private set; }

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

            services.AddDbContextFactory<ServerContext>(options =>
                options.UseMySql(appSettings.ConnectionString, mysqlOptions =>
                {
                    mysqlOptions.ServerVersion(new Version(10, 1, 37), ServerType.MariaDb);
                }));

            AddLogging(services, appSettings.ElasticsearchOptions);

            // Add services
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<ILogin, Login>();
            services.AddTransient<IAccountFactoryService, AccountFactoryService>();
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
            services.AddTransient<IInventoryTransferService, InventoryTransferService>();
            services.AddTransient<IInventoryEquipService, InventoryEquipService>();
            services.AddTransient<IInteriorDatabaseService, InteriorDatabaseService>();
            services.AddTransient<IInteriorsFactoryService, InteriorsFactoryService>();
            services.AddTransient<IHouseDatabaseService, HouseDatabaseService>();
            services.AddTransient<IHouseFactoryService, HouseFactoryService>();
            services.AddTransient<IBuyHouseService, BuyHouseService>();
            
            services.AddTransient<PlayerConnect>();
            services.AddTransient<PlayerDisconnect>();
            services.AddTransient<VehicleHandler>();
            services.AddTransient<ObjectSync>();

            services.AddSingleton<HashingService>();
            services.AddSingleton<IVehiclesManager, VehiclesManager>();
            services.AddSingleton<BankHandler>();
            services.AddSingleton<IBankAccountManager, BankAccountManager>();
            services.AddSingleton<IBusinessesManager, BusinessesManager>();
            services.AddSingleton<BusinessHandler>();
            services.AddSingleton<TemporaryChatHandler>();
            services.AddSingleton<TimeController>();
            services.AddSingleton<VehicleShopsManager>();
            services.AddSingleton<IFractionsManager, FractionsManager>();
            services.AddSingleton<FractionHandler>();
            services.AddSingleton<IInventoriesManager, InventoriesManager>();
            services.AddSingleton<InventoryHandler>();
            services.AddSingleton<INetworkingManager, NetworkingManager>();
            services.AddSingleton<VehiclesData>();
            services.AddSingleton<SoundManager>();
            services.AddSingleton<IInteriorsManager, InteriorsManager>();
            services.AddSingleton<IHousesManager, HousesManager>();
            services.AddSingleton<HouseHandler>();
            
            services.AddTransient<AdminCommands>();
            services.AddTransient<CharacterCreator>();
            services.AddTransient<SittingHandler>();
            services.AddTransient<TrashbinsController>();
            services.AddTransient<VehicleShopsHandler>();
            services.AddTransient<TownHallFractionHandler>();
            services.AddTransient<ItemFactory>();

            services.AddSingleton<SerializatorTest>();

            // Build provider
            ServiceProvider = services.BuildServiceProvider();
        }

        private static void AddLogging(IServiceCollection services, ElasticsearchOptions options)
        {
            services.AddLogging(builder =>
            {
                builder.AddSerilog();
            });

            var logsPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs/");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] <{ThreadId}><{ThreadName}> {Message:lj} {NewLine}{Exception}")
                .WriteTo.File($"{logsPath}.log",
                    LogEventLevel.Verbose,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] <{ThreadId}><{ThreadName}> {Message:lj} {NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 100,
                    rollOnFileSizeLimit: true)
                //.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(options.Uri))
                //{
                //    AutoRegisterTemplate = options.AutoRegisterTemplate,
                //    ModifyConnectionSettings = x => x.BasicAuthentication(options.Username, options.Password)
                //})
                .CreateLogger();
        }
    }
}
