using System;
using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Money;

namespace AltVStrefaRPServer.Modules.Environment
{
    public class ThrashBinsHandler
    {
        private Random _rng;
        private INotificationService _notificationService;
        private IMoneyService _moneyService;

        public ThrashBinsHandler(INotificationService notificationService, IMoneyService moneyService)
        {
            _notificationService = notificationService;
            _moneyService = moneyService;
            _rng = new Random();
            Alt.On<IPlayer, bool>("SearchedInBin", SearchedInBin);
        }

        private void SearchedInBin(IPlayer player, bool bigBin)
        {
            var character = player.GetCharacter();
            if (character == null) return;
            int reward;
            // Temporary till items 
            if (bigBin)
            {
                reward = _rng.Next(10, 40);
                _moneyService.GiveMoney(character, reward);
            }
            else
            {
                reward = _rng.Next(5, 20);
                _moneyService.GiveMoney(character, reward);
            }

            _notificationService.ShowInfoNotification(player, "Sukces!", $"Właśnie znalazłeś {reward}$ w śmietniku.", 4000);
        }
    }
}
