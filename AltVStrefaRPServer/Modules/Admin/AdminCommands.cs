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
using AltVStrefaRPServer.Modules.Businesses;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Modules.Chat;
using AltVStrefaRPServer.Modules.Fractions;
using AltVStrefaRPServer.Modules.Money;
using AltVStrefaRPServer.Modules.Vehicle;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Vehicles;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Modules.Admin
{
    public class AdminCommands
    {
        private TemporaryChatHandler _chatHandler;
        private VehicleManager _vehicleManager;
        private BankHandler _bankHandler;
        private BusinessManager _businessManager;
        private BusinessHandler _businessHandler;
        private INotificationService _notificationService;
        private VehicleShopsManager _vehicleShopsManager;
        private IVehicleSpawnService _vehicleSpawnService;
        private readonly FractionHandler _fractionHandler;

        public AdminCommands (TemporaryChatHandler chatHandler, VehicleManager vehicleManager, BankHandler bankHandler,
            BusinessManager businessManager, BusinessHandler businessHandler, INotificationService notificationService,
            VehicleShopsManager vehicleShopsManager, IVehicleSpawnService vehicleSpawnService, FractionHandler fractionHandler)
        {
            _chatHandler = chatHandler;
            _vehicleManager = vehicleManager;
            _bankHandler = bankHandler;
            _businessManager = businessManager;
            _businessHandler = businessHandler;
            _notificationService = notificationService;
            _vehicleShopsManager = vehicleShopsManager;
            _vehicleSpawnService = vehicleSpawnService;
            _fractionHandler = fractionHandler;

            Alt.Log ($"Admin commands initialized");
            AddCommands ();
        }

        private void AddCommands ()
        {
            _chatHandler.RegisterCommand ("vehicle", VehicleCommandCallback);
            _chatHandler.RegisterCommand ("tp", TeleportToPosition);
            _chatHandler.RegisterCommand ("pos", DisplayPositionCommand);
            _chatHandler.RegisterCommand ("tptowp", TeleportToWaypointCommand);
            _chatHandler.RegisterCommand ("openbank", OpenBankMenu);
            _chatHandler.RegisterCommand ("createBankAccount", async (player, args) => await CreateBankAccount (player, args));
            _chatHandler.RegisterCommand ("createbusiness", CreateNewBusiness);
            _chatHandler.RegisterCommand ("setBusinessOwner", SetBusinessOwner);
            _chatHandler.RegisterCommand ("openBusinessMenu", OpenBusinessMenu);
            _chatHandler.RegisterCommand ("enterCinema", EnterCinema);
            _chatHandler.RegisterCommand ("exitCinema", ExitCinema);
            _chatHandler.RegisterCommand ("bring", BringPlayer);
            _chatHandler.RegisterCommand ("tpToPlayer", TeleportToPlayer);
            _chatHandler.RegisterCommand ("openVehicleShop", OpenVehicleShop);
            _chatHandler.RegisterCommand ("openFractionMenu", OpenFractionMenu);
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

        private void SetBusinessOwner (IPlayer player, string[] args)
        {
            if (args == null || args.Length < 2 || !player.TryGetCharacter (out Character sender)) return;
            if (sender.Account.AdminLevel < AdminLevel.Admin) return;

            try
            {
                if (!int.TryParse (args[0].ToString (), out int characterId))
                {
                    _notificationService.ShowErrorNotfication (player, "Błąd", $"Podano zły numer postaci.", 5000);
                    return;
                }
                var character = CharacterManager.Instance.GetCharacter (characterId);
                if (character == null)
                {
                    _notificationService.ShowErrorNotfication (player, "Błąd", $"Nie znaleziono postaci z takim id.", 5000);
                    return;
                }

                if (!int.TryParse (args[1].ToString (), out int businessId))
                {
                    _notificationService.ShowErrorNotfication (player, "Błąd", $"Podano złe id biznesu.", 5000);
                    return;
                }
                var business = _businessManager.GetBusiness (businessId);
                if (business == null)
                {
                    _notificationService.ShowErrorNotfication (player, "Błąd", $"Nie znaleziono biznesu z takim id.", 5000);
                    return;
                }

                if (_businessManager.UpdateBusinessOwner (business, character).Result)
                {
                    _notificationService.ShowSuccessNotification (player, "Aktualizacja właściciela", $"Pomyślnie zaktualizowano właściciela biznesu ID({business.Id}) na {character.GetFullName()}", 6000);
                    Alt.Log ($"Updated owner of business ID({business.Id}) Name({business.BusinessName}) " +
                        $"to character ID({character.Id}) Name({character.GetFullName()})");
                }
                else
                {
                    _notificationService.ShowErrorNotfication (player, "Błąd", "Nie udało się zaktualizować właściciela biznesu.");
                }
            }
            catch (Exception e)
            {
                Alt.Log ($"Error setting business owner. Ex: {e}");
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

        private void CreateNewBusiness (IPlayer player, string[] args)
        {
            if (args == null || args.Length < 2 || !player.TryGetCharacter (out Character character)) return;
            if (character.Account.AdminLevel < AdminLevel.Admin) return;

            try
            {
                // First arg is business type
                // Second one is business name
                if (!Enum.TryParse (args[0], out BusinessType businessType))
                {
                    _notificationService.ShowErrorNotfication (player, "Błąd", "Podano zły typ biznesu");
                    return;
                }

                if (_businessManager.CreateNewBusinessAsync (character.Id, businessType, player.Position, args[1]).Result)
                {
                    _notificationService.ShowSuccessNotification (player, "Nowy biznes",
                        $"Pomyślnie stworzono nowy biznes: {businessType} z nazwą {args[1]}.", 6000);
                }
                else
                {
                    _notificationService.ShowErrorNotfication (player, "Błąd", $"Nie udało się stworzyć biznesu: {businessType} z nazwą {args[1]}.", 6000);
                }
            }
            catch (Exception e)
            {
                Alt.Log ($"Error creating new business for player {player.Name}: {e}");
                throw;
            }
        }

        private async Task CreateBankAccount (IPlayer player, string[] arg2)
        {
            await _bankHandler.CreateBankAccountAsync (player);
        }

        private async void OpenBankMenu (IPlayer player, string[] arg2)
        {
            await _bankHandler.TryToOpenBankMenu (player);
        }

        private void TeleportToWaypointCommand (IPlayer player, string[] args)
        {
            player.Emit ("teleportToWaypoint");
        }

        private void DisplayPositionCommand (IPlayer player, string[] arg2)
        {
            Alt.Log ($"Position: {player.Position} Dimension: {player.Dimension}");
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
                Alt.Log ($"Error teleporting player ID({player.Id}) to new position with command. {e}");
                throw;
            }
        }

        private void BringPlayer (IPlayer player, string[] args)
        {
            if (args == null || args.Length < 1 || !player.TryGetCharacter (out Character character)) return;
            if (character.Account.AdminLevel < AdminLevel.Support) return;
            if (!int.TryParse (args[0].ToString (), out int playerId)) return;
            var playerToBring = Alt.GetAllPlayers ().FirstOrDefault (p => p.Id == playerId);
            if (playerToBring == null)
            {
                _notificationService.ShowErrorNotfication (player, "Błąd", "Nie znaleziono gracza z podanym ID.", 4000);
                return;
            }

            playerToBring.Position = player.Position;
        }

        private void TeleportToPlayer (IPlayer player, string[] args)
        {
            if (args == null || args.Length < 1 || !player.TryGetCharacter (out Character character)) return;
            if (character.Account.AdminLevel < AdminLevel.TrialSupport) return;
            if (!int.TryParse (args[0].ToString (), out int playerId)) return;
            var playerToTeleportTo = Alt.GetAllPlayers ().FirstOrDefault (p => p.Id == playerId);
            if (playerToTeleportTo == null)
            {
                _notificationService.ShowErrorNotfication (player, "Błąd", "Nie znaleziono gracza z podanym ID.", 4000);
                return;
            }

            player.Position = playerToTeleportTo.Position;
        }

        private void VehicleCommandCallback (IPlayer player, string[] args)
        {
            if (args == null || args.Length < 1 || !player.TryGetCharacter (out Character character)) return;
            if (character.Account.AdminLevel < AdminLevel.Admin) return;
            var model = args[0];

            var vehicle = _vehicleManager.CreateVehicle (model, PositionHelper.GetPositionInFrontOf (player.Position, player.HeadRotation.Roll, 4f),
                player.Rotation, player.Dimension, character.Id, OwnerType.Character);

            _vehicleSpawnService.SpawnVehicle (vehicle);

            player.Emit ("putIntoVehicle");
        }
    }
}