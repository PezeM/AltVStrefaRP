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

            Alt.OnClient("takeSeat", TakeSeat);
            Alt.OnClient("leaveSeat", LeaveSeat);
        }

        private void LeaveSeat(IPlayer player, object[] args)
        {
            if (!int.TryParse(args[0].ToString(), out int objectId)) return;

            if (_seatsTaken.ContainsKey(objectId))
            {
                Alt.Log($"Removing {objectId} from seatsTaken dict");
                _seatsTaken.Remove(objectId);
            }
        }

        private void TakeSeat(IPlayer player, object[] args)
        {
            if (!int.TryParse(args[0].ToString(), out int objectId)) return;

            if (_seatsTaken.ContainsKey(objectId))
            {
                _notificationService.ShowErrorNotfication(player, "To miejsce jest już zajęte.", 4000);
                Alt.Log($"Someone sits at object {objectId}");
                return;
            }

            _seatsTaken.Add(objectId, player.Id);
            player.Emit("canSeat");
        }
    }
}
