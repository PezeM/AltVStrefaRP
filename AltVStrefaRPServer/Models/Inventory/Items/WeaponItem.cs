using AltV.Net.Enums;
using AltVStrefaRPServer.Models.Interfaces.Inventory;

namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public class WeaponItem : BaseItem, IEquipmentable, IDroppable
    {
        public EquipmentSlot Slot { get; set; } = EquipmentSlot.LeftHand;
        public string Model { get; set; }

        public WeaponModel WeaponModel { get; set; }
        public int Ammo { get; set; }

        public override bool UseItem(Character character)
        {
            // Equip on slot
            character.Player.GiveWeapon((uint)WeaponModel, Ammo, false);
            return true;
        }
    }
}
