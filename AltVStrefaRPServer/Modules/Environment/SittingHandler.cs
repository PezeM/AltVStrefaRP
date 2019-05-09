using System.Collections.Generic;
using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Services;

namespace AltVStrefaRPServer.Modules.Environment
{
    public class SittingHandler
    {
        private INotificationService _notificationService;
        private Dictionary<int, ushort> _seatsTaken = new Dictionary<int, ushort>();

        public SittingHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;

            Alt.On<IPlayer, int>("takeSeat", TakeSeat);
            Alt.On<IPlayer, int>("leaveSeat", LeaveSeat);
        }

        private void LeaveSeat(IPlayer player, int seatId)
        {
            if (_seatsTaken.ContainsKey(seatId))
            {
                Alt.Log($"Removing {seatId} from seatsTaken dict");
                _seatsTaken.Remove(seatId);
            }
        }

        private void TakeSeat(IPlayer player, int seatId)
        {
            if (_seatsTaken.ContainsKey(seatId))
            {
                _notificationService.ShowErrorNotfication(player, "Zajęte", "To miejsce jest już zajęte.", 4000);
                Alt.Log($"Someone sits at object {seatId}");
                return;
            }

            _seatsTaken.Add(seatId, player.Id);
            player.Emit("canSeat");
        }
    }
}
