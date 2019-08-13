using AltV.Net.Enums;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models.Inventory.Interfaces;

namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public class WeaponItem : Equipmentable, IDroppable
    {
        public string Model { get; private set; }

        public WeaponModel WeaponModel { get; private set; }
        public int Ammo { get; set; }

        public WeaponItem(string name, string model, WeaponModel weaponModel, int ammo, EquipmentSlot equipmentSlot = EquipmentSlot.LeftHand, string description = null) 
            : base(name, equipmentSlot)
        {
            Model = model;
            Ammo = ammo;
            WeaponModel = weaponModel;
            Description = !description.IsNullOrEmpty() ? description : $"{Name} z {ammo} amunicji";
        }

        public override bool UseItem(Character character)
        {
            // Equip on slot
            character.Player?.GiveWeapon((uint)WeaponModel, Ammo, false);
            return true;
        }

        public override bool UnequipItem(Character character)
        {
            // For now till ammo
            character.Player?.RemoveWeapon((uint)WeaponModel);
            return true;
        }

        public override BaseItem Copy()
        {
            return (WeaponItem)MemberwiseClone();
        }
    }
}
