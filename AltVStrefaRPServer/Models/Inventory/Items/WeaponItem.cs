using AltV.Net.Enums;
using AltVStrefaRPServer.Models.Interfaces.Inventory;
using AltVStrefaRPServer.Models.Interfaces.Items;

namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public class WeaponItem : Equipmentable, IDroppable
    {
        public string Model { get; set; }

        public WeaponModel WeaponModel { get; set; }
        public int Ammo { get; set; }

        public WeaponItem(string name, string model, WeaponModel weaponModel, int ammo, EquipmentSlot slot = EquipmentSlot.LeftHand) 
            : base(name, slot)
        {
            Model = model;
            Ammo = ammo;
            WeaponModel = weaponModel;
        }

        public override bool UseItem(Character character)
        {
            // Equip on slot
            character.Player.GiveWeapon((uint)WeaponModel, Ammo, false);
            return true;
        }

        public override BaseItem Copy()
        {
            return (WeaponItem)MemberwiseClone();
        }
    }
}
