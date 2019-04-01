using System;
using System.Collections.Generic;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehicleManager
    {
        private static readonly Lazy<VehicleManager> lazy = new Lazy<VehicleManager>(() => new VehicleManager());

        public static VehicleManager Instance { get { return lazy.Value; } }

        private VehicleManager()
        {
        }

        private Dictionary<int, VehicleModel> Vehicles = new Dictionary<int, VehicleModel>();

        /// <summary>
        /// Gets VehicleModel by id
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns>Returns <see cref="VehicleModel"/></returns>
        public VehicleModel GetVehicleModel(int vehicleId) => Vehicles.ContainsKey(vehicleId) ? Vehicles[vehicleId] : null;
    }
}
