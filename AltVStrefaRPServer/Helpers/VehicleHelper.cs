using AltV.Net;
using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Helpers
{
    public class VehicleHelper
    {
        public static IVehicle GetClosestVehicle(IPlayer player, float radius = 4f)
        {
            var startTime = Time.GetTimestampMs();
            IVehicle returnVehicle = null;
            foreach (var vehicle in Alt.GetAllVehicles())
            {
                if (player.Dimension != vehicle.Dimension) continue;

                var playerDistanceToVehicle = player.Position.Distance(vehicle.Position);
                if (playerDistanceToVehicle < radius)
                {
                    radius = playerDistanceToVehicle;
                    returnVehicle = vehicle;
                }
            }
            Alt.Log($"Found the nearest vehicle in {Time.GetTimestampMs() - startTime} ms.");
            return returnVehicle;
        }
    }
}
