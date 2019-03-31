using System.Collections.Generic;
using AltV.Net;
using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Modules.Environment
{
    public class Sitting
    {
        private Dictionary<int, ushort> _seatsTaken = new Dictionary<int, ushort>();

        public Sitting()
        {
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
            else
            {
                Alt.Log($"There is seat taken with object {objectId}");
            }
        }

        private void TakeSeat(IPlayer player, object[] args)
        {
            if (!int.TryParse(args[0].ToString(), out int objectId)) return;

            if (_seatsTaken.ContainsKey(objectId))
            {
                Alt.Log($"Someone sits at object {objectId}");
                return;
            }

            _seatsTaken.Add(objectId, player.Id);
            player.Emit("canSeat");
        }
    }
}
