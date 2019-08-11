using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Vehicles;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class VehicleInventoryContainer : InventoryContainer, IInventoryOwner<VehicleModel, VehicleInventoryContainer>
    {
        public VehicleModel Owner { get; set; }

        protected VehicleInventoryContainer() { }

        public VehicleInventoryContainer(int maxSlots) : base(maxSlots)
        {
            MaxSlots = maxSlots;
        }
    }
}
