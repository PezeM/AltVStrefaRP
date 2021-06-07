using AltVStrefaRPServer.Models.Inventory.Interfaces;

namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public abstract class Equipmentable : BaseItem, IEquipmentable
    {
        public EquipmentSlot EquipmentSlot { get; set; }

        protected Equipmentable(string name, EquipmentSlot slot) : base(name, 1)
        {
            EquipmentSlot = slot;
        }

        public abstract bool UnequipItem(Character character);
    }
}
