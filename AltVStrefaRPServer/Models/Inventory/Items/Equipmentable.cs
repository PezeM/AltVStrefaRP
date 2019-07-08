using AltVStrefaRPServer.Models.Interfaces.Inventory;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Models.Interfaces.Items
{
    public abstract class Equipmentable : BaseItem, IEquipmentable
    {
        public EquipmentSlot Slot { get; set; }

        public Equipmentable(string name, EquipmentSlot slot) : base(name, 1)
        {
            Slot = slot;
        }
    }
}
