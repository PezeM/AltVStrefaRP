using System;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Server;
using AltVStrefaRPServer.Modules.Businesses;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Modules.Chat;
using AltVStrefaRPServer.Modules.Fractions;
using AltVStrefaRPServer.Modules.Inventory;
using AltVStrefaRPServer.Modules.Money;
using AltVStrefaRPServer.Modules.Vehicle;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Characters;
using AltVStrefaRPServer.Services.Fractions;
using AltVStrefaRPServer.Services.Inventories;
using AltVStrefaRPServer.Services.Money;
using AltVStrefaRPServer.Services.Vehicles;
using Microsoft.Extensions.Logging;
using net.vieapps.Components.Utility;
using Newtonsoft.Json;
using VehicleModel = AltV.Net.Enums.VehicleModel;

namespace AltVStrefaRPServer.Modules.Admin
{
    public class AdminCommands
    {
        private TemporaryChatHandler _chatHandler;
        private IVehiclesManager _vehiclesManager;
        private BankHandler _bankHandler;
        private IBusinessesManager _businessesManager;
        private BusinessHandler _businessHandler;
        private INotificationService _notificationService;
        private VehicleShopsManager _vehicleShopsManager;
        private IVehicleSpawnService _vehicleSpawnService;
        private readonly FractionHandler _fractionHandler;
        private readonly IFractionsManager _fractionsManager;
        private readonly IFractionDatabaseService _fractionDatabaseService;
        private readonly IMoneyService _moneyService;
        private readonly InventoryHandler _inventoryHandler;
        private readonly IInventoryDatabaseService _inventoryDatabaseService;
        private readonly ICharacterDatabaseService _characterDatabaseService;
        private readonly ItemFactory _itemFactory;
        private readonly ILogger<AdminCommands> _logger;

        public AdminCommands(TemporaryChatHandler chatHandler, IVehiclesManager vehiclesManager, BankHandler bankHandler,
            IBusinessesManager businessesManager, BusinessHandler businessHandler, INotificationService notificationService,
            VehicleShopsManager vehicleShopsManager, IVehicleSpawnService vehicleSpawnService, FractionHandler fractionHandler,
            IFractionsManager fractionsManager,
            IFractionDatabaseService fractionDatabaseService, IMoneyService moneyService,
            InventoryHandler inventoryHandler, IInventoryDatabaseService inventoryDatabaseService, ICharacterDatabaseService characterDatabaseService,
            ItemFactory itemFactory, ILogger<AdminCommands> logger)
        {
            _chatHandler = chatHandler;
            _vehiclesManager = vehiclesManager;
            _bankHandler = bankHandler;
            _businessesManager = businessesManager;
            _businessHandler = businessHandler;
            _notificationService = notificationService;
            _vehicleShopsManager = vehicleShopsManager;
            _vehicleSpawnService = vehicleSpawnService;
            _fractionHandler = fractionHandler;
            _fractionsManager = fractionsManager;
            _fractionDatabaseService = fractionDatabaseService;
            _moneyService = moneyService;
            _inventoryHandler = inventoryHandler;
            _inventoryDatabaseService = inventoryDatabaseService;
            _characterDatabaseService = characterDatabaseService;
            _itemFactory = itemFactory;
            _logger = logger;

            _logger.LogDebug("Admin commands initialized");
            AddCommands ();
        }

        private void AddCommands ()
        {
            _chatHandler.RegisterCommand ("vehicle", VehicleCommandCallback);
            _chatHandler.RegisterCommand ("tp", TeleportToPosition);
            _chatHandler.RegisterCommand ("pos", DisplayPositionCommand);
            _chatHandler.RegisterCommand ("tptowp", TeleportToWaypointCommand);
            _chatHandler.RegisterCommand ("openbank", OpenBankMenu);
            _chatHandler.RegisterCommand ("createBankAccount", async (player, args) => await CreateBankAccountAsync (player, args));
            _chatHandler.RegisterCommand ("createbusiness", async  (player, args) => await CreateNewBusinessAsync(player, args));
            _chatHandler.RegisterCommand ("setBusinessOwner", async (player, args) => await SetBusinessOwnerAsync(player, args));
            _chatHandler.RegisterCommand ("openBusinessMenu", OpenBusinessMenu);
            _chatHandler.RegisterCommand ("enterCinema", EnterCinema);
            _chatHandler.RegisterCommand ("exitCinema", ExitCinema);
            _chatHandler.RegisterCommand ("bring", BringPlayer);
            _chatHandler.RegisterCommand ("tpToPlayer", TeleportToPlayer);
            _chatHandler.RegisterCommand("addMoney", AddMoneyToPlayer);
            _chatHandler.RegisterCommand ("openVehicleShop", OpenVehicleShop);
            _chatHandler.RegisterCommand ("openFractionMenu", OpenFractionMenu);
            _chatHandler.RegisterCommand("setFractionOwner", async (player, args) => await SetFractionOwnerAsync(player,args));
            _chatHandler.RegisterCommand("addEmployeeToFraction", async (player, args) => await AddEmployeeToFractionAsync(player,args));
            _chatHandler.RegisterCommand("getAllVehicles", GetAllVehicles);
            _chatHandler.RegisterCommand("setAdminLevel", SetAdminLevel);
            _chatHandler.RegisterCommand("dropItem", async (player, args) => await DropItemAsync(player, args));
            _chatHandler.RegisterCommand("addItem", async (player, args) => await AddItemAsync(player, args));
            _chatHandler.RegisterCommand("getInventory", GetInventory);
            _chatHandler.RegisterCommand("useItem", async (player, args) => await UseItemAsync(player, args));
            _chatHandler.RegisterCommand("removeItem", async (player, args) => await RemoveItemAsync(player, args));
            _chatHandler.RegisterCommand("lookupInventory", LookupInventory);
            _chatHandler.RegisterCommand("testEquipItem", TestEquipItem);
            _chatHandler.RegisterCommand("testSave", TestSave);
        }

        private void TestSave(IPlayer player, string[] args)
        {
            var startTime = Time.GetTimestampMs();
            int savedCharacters = 0;
            _ = Task.Run(() =>
              {
                  foreach (var character in CharacterManager.Instance.GetAllCharacters())
                  {
                      lock (character)
                      {
                          if (character.Player != null && character.Player.Exists)
                          {
                              _characterDatabaseService.UpdateCharacterAsync(character);
                          }
                      }
                  }
              });
            _logger.LogInformation("Saved {characterCount} characters to databse in {elapsedTime}", savedCharacters, Time.GetElapsedTime(startTime));
        }

        private void OpenVehicleShop (IPlayer player, string[] arg2)
        {
            var vehicleShop = _vehicleShopsManager.GetClosestVehicleShop (player.Position);
            if (vehicleShop == null) return;

            player.Emit ("openVehicleShop", vehicleShop.VehicleShopId, JsonConvert.SerializeObject (vehicleShop.AvailableVehicles));
        }

        private void ExitCinema (IPlayer player, string[] arg2)
        {
            if (!player.GetData ("beforeCinemaPosition", out Position position))
            {
                player.Position = new Position (AppSettings.Current.ServerConfig.SpawnPosition.X,
                    AppSettings.Current.ServerConfig.SpawnPosition.Y,
                    AppSettings.Current.ServerConfig.SpawnPosition.Z);
            }
            else
            {
                player.Position = position;
            }
        }

        private void EnterCinema (IPlayer arg1, string[] args)
        {
            foreach (var player in Alt.GetAllPlayers ())
            {
                player.SetData ("beforeCinemaPosition", player.Position);
                player.Position = new Position (-1427.299f, -245.1012f, 16.8039f);
            }

            Alt.EmitAllClients ("enterCinema");
        }

        private async Task SetBusinessOwnerAsync (IPlayer player, string[] args)
        {
            if (args == null || args.Length < 2 || !player.TryGetCharacter (out Character sender)) return;
            if (sender.Account.AdminLevel < AdminLevel.Admin) return;

            try
            {
                if (!int.TryParse (args[0].ToString (), out int characterId))
                {
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", $"Podano zły numer postaci.", 5000);
                    return;
                }
                var character = CharacterManager.Instance.GetCharacter (characterId);
                if (character == null)
                {
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", $"Nie znaleziono postaci z takim id.", 5000);
                    return;
                }

                if (!int.TryParse (args[1].ToString (), out int businessId))
                {
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", $"Podano złe id biznesu.", 5000);
                    return;
                }
                var business = _businessesManager.GetBusiness (businessId);
                if (business == null)
                {
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", $"Nie znaleziono biznesu z takim id.", 5000);
                    return;
                }

                if (await _businessesManager.UpdateBusinessOwnerAsync (business, character))
                {
                    await _notificationService.ShowSuccessNotificationAsync(player, "Aktualizacja właściciela", 
                        $"Pomyślnie zaktualizowano właściciela biznesu ID({business.Id}) na {character.GetFullName()}", 6000);
                    _logger.LogInformation("Character {characterName} CID({senderId}) updated owner of business {businessName} ID({businessId}) to character {characterName} CID({characterId})", 
                        sender.GetFullName(), sender.Id, business.BusinessName, businessId, character.GetFullName(), characterId);
                }
                else
                {
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się zaktualizować właściciela biznesu.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error setting business owner.");
                throw;
            }
        }

        private void OpenBusinessMenu (IPlayer player, string[] args)
        {
            var character = player.GetCharacter ();
            if (character == null) return;
            _businessHandler.OpenBusinessMenu (character);
        }

        private void OpenFractionMenu (IPlayer player, string[] args)
        {
            if (!player.TryGetCharacter (out var character)) return;
            _fractionHandler.OpenFractionMenu (character);
        }

        private async Task AddEmployeeToFractionAsync(IPlayer player, string[] args)
        {
            if(args == null || args.Length < 1 || !player.TryGetCharacter (out Character sender)) return;
            if (sender.Account.AdminLevel < AdminLevel.Admin) return;
            if(!int.TryParse(args[0].ToString(), out int fractionId)) return;

            await _fractionHandler.AcceptFractionInviteEventAsync(player, fractionId);
        }

        private async Task SetFractionOwnerAsync(IPlayer player, string[] args)
        {
            if (args == null || args.Length < 2 || !player.TryGetCharacter (out Character sender)) return;
            if (sender.Account.AdminLevel < AdminLevel.Admin) return;

            try
            {
                if (!int.TryParse (args[0].ToString (), out int characterId))
                {
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", $"Podano zły numer postaci.", 5000);
                    return;
                }
                var newOwner = CharacterManager.Instance.GetCharacter (characterId);
                if (newOwner == null)
                {
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", $"Nie znaleziono postaci z takim id.", 5000);
                    return;
                }

                if (!int.TryParse (args[1].ToString (), out int fractionId))
                {
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", $"Podano złe id biznesu.", 5000);
                    return;
                }

                if (!_fractionsManager.TryToGetFraction(fractionId, out var fraction)) return;

                if (await fraction.ForceFractionOwnerAsync(newOwner, _fractionDatabaseService))
                {
                    await _notificationService.ShowSuccessNotificationAsync(player, "Aktualizacja właściciela", 
                        $"Pomyślnie zaktualizowano właściciela frakcji ID({fractionId}) na ID({newOwner.Id}) {newOwner.GetFullName()}");
                    _logger.LogInformation("Character {characterName} CID({characterId}) updated fraction {fractionName} ID({fractionId}) owner to character {characterName} CID({characterId})",
                        sender.GetFullName(), sender.Id, fraction.Name, fractionId, newOwner.GetFullName(), newOwner.Id);
                }
                else
                {
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się zaktualizować właściciela biznesu.");
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error setting fraction owner.");
                throw;
            }
        }

        private async Task CreateNewBusinessAsync (IPlayer player, string[] args)
        {
            if (args == null || args.Length < 2 || !player.TryGetCharacter (out Character character)) return;
            if (character.Account.AdminLevel < AdminLevel.Admin) return;

            try
            {
                // First arg is business type
                // Second one is business name
                if (!Enum.TryParse (args[0], out BusinessType businessType))
                {
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Podano zły typ biznesu");
                    return;
                }

                if (await _businessesManager.CreateNewBusinessAsync (character.Id, businessType, player.Position, args[1]))
                {
                    await _notificationService.ShowSuccessNotificationAsync(player, "Nowy biznes",
                        $"Pomyślnie stworzono nowy biznes: {businessType} z nazwą {args[1]}.", 6000);
                    _logger.LogInformation("Character {characterName} CID({characterId}) created new business of type {businessType} with name {businessName}",
                        character.GetFullName(), character.Id, businessType, args[1]);
                }
                else
                {
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", $"Nie udało się stworzyć biznesu: {businessType} z nazwą {args[1]}.", 6000);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while creating new business.");
                throw;
            }
        }

        private async Task CreateBankAccountAsync(IPlayer player, string[] arg2)
        {
            await _bankHandler.CreateBankAccountAsync (player);
        }

        private void OpenBankMenu (IPlayer player, string[] arg2)
        {
            _bankHandler.TryToOpenBankMenu(player);
        }

        private void TeleportToWaypointCommand (IPlayer player, string[] args)
        {
            player.Emit ("teleportToWaypoint");
        }

        private void DisplayPositionCommand (IPlayer player, string[] arg2)
        {
            _logger.LogInformation("Position {position} Dimension {dimension}", player.Position, player.Dimension);
        }

        private void TeleportToPosition (IPlayer player, string[] args)
        {
            if (args == null || args.Length < 3) return;
            try
            {
                player.Position = new Position (float.Parse (args[0]), float.Parse (args[1]), float.Parse (args[2]));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error teleporting player {playerId} to new position with command", player.Id);
                throw;
            }
        }

        private void AddMoneyToPlayer(IPlayer player, string[] args)
        {
            if (args == null || args.Length < 2 || !player.TryGetCharacter(out Character character)) return;
            if(character.Account.AdminLevel < AdminLevel.SuperModerator) return;
            if (!int.TryParse(args[0].ToString(), out int playerId)) return;
            if (!float.TryParse(args[1].ToString(), out float money)) return;
            var characterToGiveMoneyTo = CharacterManager.Instance.GetCharacter(playerId);
            if (characterToGiveMoneyTo == null)
            {
                _notificationService.ShowErrorNotification(player, "Błędne ID", $"Nie znaleziono żadnej postaci z ID {playerId}.", 6000);
                return;
            }

            _moneyService.GiveMoney(characterToGiveMoneyTo, money);
            _notificationService.ShowSuccessNotification(characterToGiveMoneyTo.Player, "Otrzymano pieniądze", $"Otrzymałeś {money} pieniędzy.");
            _logger.LogInformation("Character {characterName} ({characterId}) added money ({money})$ to character {characterName} CID({characterID})",
                character.GetFullName(), character.Id, money, characterToGiveMoneyTo.GetFullName(), characterToGiveMoneyTo.Id);
        }


        private void BringPlayer (IPlayer player, string[] args)
        {
            if (args == null || args.Length < 1 || !player.TryGetCharacter (out Character character)) return;
            if (character.Account.AdminLevel < AdminLevel.Support) return;
            if (!int.TryParse (args[0].ToString(), out int playerId)) return;
            var playerToBring = Alt.GetAllPlayers().FirstOrDefault(p => p.Id == playerId);
            if (playerToBring == null)
            {
                _notificationService.ShowErrorNotification (player, "Błąd", "Nie znaleziono gracza z podanym ID.", 4000);
                return;
            }

            playerToBring.Position = player.Position;
        }

        private void TeleportToPlayer (IPlayer player, string[] args)
        {
            if (args == null || args.Length < 1 || !player.TryGetCharacter (out Character character)) return;
            if (character.Account.AdminLevel < AdminLevel.TrialSupport) return;
            if (!int.TryParse (args[0].ToString (), out int playerId)) return;
            var playerToTeleportTo = Alt.GetAllPlayers().FirstOrDefault(p => p.Id == playerId);
            if (playerToTeleportTo == null)
            {
                _notificationService.ShowErrorNotification (player, "Błąd", "Nie znaleziono gracza z podanym ID.", 4000);
                return;
            }

            player.Position = playerToTeleportTo.Position;
        }

        private void VehicleCommandCallback (IPlayer player, string[] args)
        {
            if (args == null || args.Length < 1 || !player.TryGetCharacter (out Character character)) return;
            if (character.Account.AdminLevel < AdminLevel.Admin) return;
            var model = args[0];
            var containsEnum = Enum.GetNames(typeof(VehicleModel)).Contains(model.GetCapitalizedFirstLetter());
            if (!containsEnum)
            {
                _notificationService.ShowErrorNotification(player, "Zła nazwa pojazdu.", $"Nie można stworzyć pojazdu z nazwa {model}");
                return;
            }

            var vehicle = _vehiclesManager.CreateVehicle (model, PositionHelper.GetPositionInFrontOf (player.Position, player.HeadRotation.Roll, 4f),
                player.Rotation, player.Dimension, character.Id, OwnerType.Character);

            _vehicleSpawnService.SpawnVehicle (vehicle);

            player.Emit ("putIntoVehicle");
        }

        private void GetAllVehicles(IPlayer player, string[] args)
        {
            if (args == null || args.Length < 1 || !player.TryGetCharacter(out Character character)) return;
            if (character.Account.AdminLevel < AdminLevel.Moderator) return;
            if(!int.TryParse(args[0].ToString(), out int playerId)) return;
            var characterToLook = CharacterManager.Instance.GetCharacter(playerId);
            if (characterToLook == null)
            {
                _notificationService.ShowErrorNotification (player, "Błąd", "Nie znaleziono gracza z podanym ID.", 4000);
                return;
            }

            var characterVehicles = _vehiclesManager.GetAllPlayerVehicles(character);
            if (characterVehicles == null)
            {
                _notificationService.ShowErrorNotification (player, "Błąd", "Gracz nie posiada żadnych pojazdów.");
                return;
            }

            // Need to make some dto with vehicle info instead of converting whole object
            // player.Emit("testVehicleList", JsonConvert.SerializeObject(characterVehicles));
        }

        private void SetAdminLevel(IPlayer player, string[] args)
        {
            if (!player.TryGetCharacter(out Character character)) return;
            character.Account.AdminLevel = AdminLevel.HeadAdmin;
        }

        private void GetInventory(IPlayer player, string[] args)
        {
            if (!player.TryGetCharacter(out var character)) return;
            var inventory = JsonConvert.SerializeObject(character.Inventory.Items, Formatting.Indented);
            Alt.Log(inventory);
            player.Emit("testInventory", inventory);
        }

        private async Task AddItemAsync(IPlayer player, string[] args)
        {
            var startTime = Time.GetTimestampMs();
            if (args == null || args.Length < 2) return;
            if (!int.TryParse(args[0].ToString(), out int itemID)) return;
            if (!int.TryParse(args[1].ToString(), out int itemAmount)) return;
            if (!player.TryGetCharacter(out var character)) return;
            if (!Enum.IsDefined(typeof(ItemType), itemID))
            {
                //_chat.Send(player, "Podano błędne item ID");
                return;
            }
            var newItem = _itemFactory.CreateItem((ItemType)itemID);
            if (newItem == null) return;
            Alt.Log($"New item is of type {newItem.GetType()} and name {newItem.Name}");
            var response = await character.Inventory.AddItemAsync(newItem, itemAmount, _inventoryDatabaseService);
            string itemIds = string.Empty;
            foreach (var item in response.NewItems)
            {
                itemIds += $"{item.Id},";
            }

            Alt.Log($"Added item id is {itemIds} in {Time.GetElapsedTime(startTime)}ms");
        }

        private async Task DropItemAsync(IPlayer player, string[] args)
        {
            var startTime = Time.GetTimestampMs();
            if(args == null || args.Length < 2) return;
            if (!int.TryParse(args[0].ToString(), out int itemId))
            {
                return;
            }
            if (!int.TryParse(args[1].ToString(), out int amount))
            {
                return;
            }
            await _inventoryHandler.DropItemAsync(player, itemId, amount, new Position(player.Position.X + 1, player.Position.Y + 1, player.Position.Z));
            Alt.Log($"Dropped item {itemId} in {Time.GetElapsedTime(startTime)}ms");
        }

        private async Task UseItemAsync(IPlayer player, string[] args)
        {
            var startTime = Time.GetTimestampMs();
            if(args == null || args.Length < 1) return;
            if (!int.TryParse(args[0].ToString(), out int itemId)) return;
            await _inventoryHandler.UseInventoryItemAsync(player, itemId);
            Alt.Log($"Used item in {Time.GetElapsedTime(startTime)}ms");
        }

        private async Task RemoveItemAsync(IPlayer player, string[] args)
        {
            if (args == null || args.Length < 2) return;
            if (!int.TryParse(args[0].ToString(), out int itemId)) return;
            if (!int.TryParse(args[1].ToString(), out int amount)) return;
            await _inventoryHandler.InventoryRemoveItemAsync(player, itemId, amount);
        }

        private void LookupInventory(IPlayer player, string[] args)
        {
            if (args == null || args.Length < 1) return;
            if (!int.TryParse(args[0].ToString(), out int vehicleId)) return;
            if (!_vehiclesManager.TryGetVehicleModel(vehicleId, out var vehicle)) return;

            var inventory = JsonConvert.SerializeObject(vehicle.Inventory.Items, Formatting.Indented);
            Alt.Log(inventory);
        }

        private void TestEquipItem(IPlayer player, string[] args)
        {
            if (args == null || args.Length < 1) return;
            if (!player.TryGetCharacter(out var charatcer)) return;
            if (!int.TryParse(args[0], out var itemId)) return;

            charatcer.Equipment.EquipItem(itemId);
        }
    }
}