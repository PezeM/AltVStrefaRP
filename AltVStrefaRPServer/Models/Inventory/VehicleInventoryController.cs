using AltVStrefaRPServer.Models.Inventory.Interfaces;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class VehicleInventoryController : InventoryController, IInventoryOwner<VehicleModel, VehicleInventoryController>
    {
        public VehicleModel Owner { get; set; }

        public VehicleInventoryController(int maxSlots) : base()
        {
            MaxSlots = maxSlots;
        }
    }
}
