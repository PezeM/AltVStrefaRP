using System;
using System.Collections.Generic;
using System.Linq;
using AltV.Net;
using AltVStrefaRPServer.Helpers;
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
        /// Adds list of VehicleModel to dictionary and sets IsSpawned flag to false
        /// </summary>
        /// <param name="vehiclesList">List of <see cref="VehicleModel"/></param>
        public void AddVehicles(List<VehicleModel> vehiclesList)
        {
            var startTime = Time.GetTimestampMs();
            foreach (var vehicle in vehiclesList)
            {
                vehicle.IsSpawned = false;
                Vehicles.Add(vehicle.Id, vehicle);
            }
            Alt.Log($"Loaded {Vehicles.Count} vehicles from database in {Time.GetTimestampMs() - startTime}ms.");
        }

        /// <summary>
        /// Gets VehicleModel by id
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns>Returns <see cref="VehicleModel"/></returns>
        public VehicleModel GetVehicleModel(int vehicleId) => Vehicles.ContainsKey(vehicleId) ? Vehicles[vehicleId] : null;

        /// <summary>
        /// Gets VehicleModel by vehicleHandle id
        /// </summary>
        /// <param name="vehicleID">Id of vehicle handle</param>
        /// <returns></returns>
        public VehicleModel GetVehicleModel(ushort vehicleID) => Vehicles.Values.FirstOrDefault(v => v.VehicleHandle?.Id == vehicleID);


    }
}
