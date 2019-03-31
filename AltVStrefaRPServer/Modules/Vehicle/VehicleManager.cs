using System;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehicleManager
    {
        private static readonly Lazy<VehicleManager> lazy = new Lazy<VehicleManager>(() => new VehicleManager());

        public static VehicleManager Instance { get { return lazy.Value; } }

        private VehicleManager()
        {
        }


    }
}
