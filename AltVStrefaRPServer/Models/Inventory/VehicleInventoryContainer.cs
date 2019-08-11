using AltVStrefaRPServer.Models.Vehicles;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class VehicleInventoryContainer : InventoryContainer
    {
        public VehicleModel Owner { get; set; }
        protected VehicleInventoryContainer() { }

        public VehicleInventoryContainer(int maxSlots) : base(maxSlots)
        {
            MaxSlots = maxSlots;
        }
    }
}
