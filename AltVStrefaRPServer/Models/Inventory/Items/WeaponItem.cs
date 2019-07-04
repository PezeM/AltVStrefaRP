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

        public WeaponItem(string name, int stackSize, string model, WeaponModel weaponModel, int ammo, EquipmentSlot slot = EquipmentSlot.LeftHand) 
            : base(name, stackSize)
        {
            Model = model;
            WeaponModel = weaponModel;
            Ammo = ammo;
            WeaponModel = weaponModel;
            Slot = slot;
        }

        public override bool UseItem(Character character)
        {
            // Equip on slot
            character.Player.GiveWeapon((uint)WeaponModel, Ammo, false);
            return true;
        }
    }
}
