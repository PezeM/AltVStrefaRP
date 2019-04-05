using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Modules.Character;
using AltVStrefaRPServer.Modules.Chat;
using AltVStrefaRPServer.Modules.Vehicle;
using System;
using AltV.Net.Data;
using AltVStrefaRPServer.Modules.Money;

namespace AltVStrefaRPServer.Modules.Admin
{
    public class AdminCommands
    {
        private TemporaryChatHandler _chatHandler;
        private VehicleManager _vehicleManager;
        private BankHandler _bankHandler;

        public AdminCommands(TemporaryChatHandler chatHandler, VehicleManager vehicleManager, BankHandler bankHandler)
        {
            _chatHandler = chatHandler;
            _vehicleManager = vehicleManager;
            _bankHandler = bankHandler;

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
        }

        private void CreateBankAccount(IPlayer arg1, string[] arg2)
        {
            _bankHandler.CreateBankAccountAsync(arg1, arg2);
        }

        private void OpenBankMenu(IPlayer arg1, string[] arg2)
        {
            _bankHandler.TryToOpenBankMenu(arg1, arg2);
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

            var vehicle = _vehicleManager.CreateVehicle(model, player.Position, player.HeadRotation.pitch, player.Dimension, character.Id);
            _vehicleManager.SpawnVehicle(vehicle.Id);
        }
    }
}
