using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Modules.Chat;
using AltVStrefaRPServer.Modules.Vehicle;
using System;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Modules.Businesses;
using AltVStrefaRPServer.Modules.Money;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Modules.CharacterModule;

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

        public AdminCommands(TemporaryChatHandler chatHandler, VehicleManager vehicleManager, BankHandler bankHandler, 
            BusinessManager businessManager, BusinessHandler businessHandler, INotificationService notificationService)
        {
            _chatHandler = chatHandler;
            _vehicleManager = vehicleManager;
            _bankHandler = bankHandler;
            _businessManager = businessManager;
            _businessHandler = businessHandler;
            _notificationService = notificationService;

            Alt.Log($"Admin commands initialized");
            AddCommands();
        }

        private void AddCommands()
        {
            _chatHandler.RegisterCommand("vehicle", VehicleCommandCallback);
            _chatHandler.RegisterCommand("tp", TeleportToPosition);
            _chatHandler.RegisterCommand("pos", DisplayPositionCommand);
            _chatHandler.RegisterCommand("tptowp", TeleportToWaypointCommand);
            _chatHandler.RegisterCommand("openbank", OpenBankMenu);
            _chatHandler.RegisterCommand("createBankAccount", CreateBankAccount);
            _chatHandler.RegisterCommand("createbusiness", CreateNewBusiness);
            _chatHandler.RegisterCommand("setBusinessOwner", SetBusinessOwner);
            _chatHandler.RegisterCommand("openBusinessMenu", OpenBusinessMenu);
        }

        private void SetBusinessOwner(IPlayer player, string[] args)
        {
            if(args == null || args.Length < 2) return;
            try
            {
                if (!int.TryParse(args[0].ToString(), out int characterId))
                {
                    _notificationService.ShowErrorNotfication(player,$"Podano zły numer postaci.", 5000);
                    return;
                }
                var character = CharacterManager.Instance.GetCharacter(characterId);
                if (character == null)
                {
                    _notificationService.ShowErrorNotfication(player,$"Nie znaleziono postaci z takim id.", 5000);
                    return;
                }

                if (!int.TryParse(args[1].ToString(), out int businessId))
                {
                    _notificationService.ShowErrorNotfication(player,$"Podano złe id biznesu.", 5000);
                    return;
                }
                var business = _businessManager.GetBusiness(businessId);
                if (business == null)
                {
                    _notificationService.ShowErrorNotfication(player,$"Nie znaleziono biznesu z takim id.", 5000);
                    return;
                }

                if (_businessManager.UpdateBusinessOwner(business, character).Result)
                {
                    _notificationService.ShowSuccessNotification(player, $"Pomyślnie zaktualizowano właściciela biznesu ID({business.Id}) " +
                                                                         $"na {character.GetFullName()}", 6000);
                    Alt.Log($"Updated owner of business ID({business.Id}) Name({business.BusinessName}) " +
                            $"to character ID({character.Id}) Name({character.GetFullName()})");
                }
                else
                {
                    _notificationService.ShowErrorNotfication(player, "Nie udało się zaktualizować właściciela biznesu.");
                }
            }
            catch (Exception e)
            {
                Alt.Log($"Error setting business owner. Ex: {e}");
                throw;
            }
        }

        private void OpenBusinessMenu(IPlayer player, string[] args)
        {
            var character = player.GetCharacter();
            if (character == null) return;
            _businessHandler.OpenBusinessMenu(character);
        }

        private void CreateNewBusiness(IPlayer player, string[] args)
        {
            if (args == null || args.Length < 2) return;
            try
            {
                var character = CharacterManager.Instance.GetCharacter(player);
                if (character == null) return;
                // First arg is business type
                // Second one is business name
                if(!Enum.TryParse(args[0], out BusinessType businessType))
                {
                    _notificationService.ShowErrorNotfication(player, "Podano zły typ biznesu");
                    return;
                }

                if (_businessManager.CreateNewBusinessAsync(character.Id, businessType, player.Position, args[1]).Result)
                {
                    _notificationService.ShowSuccessNotification(player, 
                        $"Pomyślnie stworzono nowy biznes: {businessType} z nazwą {args[1]}.", 6000);
                }
                else
                {
                    _notificationService.ShowErrorNotfication(player, $"Nie udało się stworzyć biznesu: {businessType} z nazwą {args[1]}.", 6000);
                }
            }
            catch (Exception e)
            {
                Alt.Log($"Error creating new business for player {player.Name}: {e}");
                throw;
            }
        }

        private void CreateBankAccount(IPlayer arg1, string[] arg2)
        {
            _bankHandler.CreateBankAccountAsync(arg1, arg2);
        }

        private async void OpenBankMenu(IPlayer arg1, string[] arg2)
        {
            await _bankHandler.TryToOpenBankMenu(arg1, arg2);
        }

        private void TeleportToWaypointCommand(IPlayer player, string[] args)
        {
            player.Emit("teleportToWaypoint");
        }

        private void DisplayPositionCommand(IPlayer player, string[] arg2)
        {
            Alt.Log($"Position: {player.Position} Dimension: {player.Dimension}");
        }

        private void TeleportToPosition(IPlayer player, string[] args)
        {
            if(args == null || args.Length < 3) return;
            try
            {
                player.Position = new Position(float.Parse(args[0]), float.Parse(args[1]),float.Parse(args[2]));
            }
            catch (Exception e)
            {
                Alt.Log($"Error teleporting player ID({player.Id}) to new position with command. {e}");
                throw;
            }
        }

        private void VehicleCommandCallback(IPlayer player, string[] args)
        {
            if(args == null || args.Length < 1) return;
            var model = args[0];
            var character = CharacterManager.Instance.GetCharacter(player);
            if (character == null) return;

            var vehicle = _vehicleManager.CreateVehicle(model, player.Position, player.HeadRotation.pitch, player.Dimension, character.Id, OwnerType.None);
            _vehicleManager.SpawnVehicle(vehicle.Id);
        }
    }
}
