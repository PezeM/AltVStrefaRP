namespace AltVStrefaRPServer.Models.Inventory
{
    public class VehicleInventoryController : InventoryController
    {
        public VehicleModel Owner { get; set; }

        public VehicleInventoryController(int maxSlots) : base()
        {
            MaxSlots = maxSlots;
        }
    }
}
