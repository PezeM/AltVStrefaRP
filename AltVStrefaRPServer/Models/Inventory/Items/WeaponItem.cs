using AltV.Net.Enums;
using AltVStrefaRPServer.Models.Inventory.Interfaces;

namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public class WeaponItem : Equipmentable, IDroppable
    {
        public string Model { get; }

        public WeaponModel WeaponModel { get; }
        public int Ammo { get; set; }

        public WeaponItem(string name, string model, WeaponModel weaponModel, int ammo, EquipmentSlot equipmentSlot = EquipmentSlot.LeftHand) 
            : base(name, equipmentSlot)
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

        public override bool DeequipItem(Character character)
        {
            throw new System.NotImplementedException();
        }

        public override BaseItem Copy()
        {
            return (WeaponItem)MemberwiseClone();
        }
    }
}
