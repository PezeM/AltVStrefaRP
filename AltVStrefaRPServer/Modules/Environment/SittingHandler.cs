using System.Collections.Generic;
using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Services;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Modules.Environment
{
    public class SittingHandler
    {
        private INotificationService _notificationService;
        private readonly ILogger<SittingHandler> _logger;
        private Dictionary<int, int> _seatsTaken;

        public SittingHandler(INotificationService notificationService, ILogger<SittingHandler> logger)
        {
            _seatsTaken = new Dictionary<int, int>();
            _notificationService = notificationService;
            _logger = logger;

            Alt.On<IPlayer, int>("takeSeat", TakeSeat);
            Alt.On<IPlayer, int>("leaveSeat", LeaveSeat);
        }

        private void LeaveSeat(IPlayer player, int seatId)
        {
            if (_seatsTaken.ContainsKey(seatId))
            {
                _logger.LogDebug("Removing {seatId} from taken seats dictionary", seatId);
                _seatsTaken.Remove(seatId);
            }
        }

        private void TakeSeat(IPlayer player, int seatId)
        {
            if (_seatsTaken.ContainsKey(seatId))
            {
                _notificationService.ShowErrorNotification(player, "Zajęte", "To miejsce jest już zajęte.", 4000);
                return;
            }

            _seatsTaken.Add(seatId, player.Id);
            player.Emit("canSeat");
        }
    }
}
