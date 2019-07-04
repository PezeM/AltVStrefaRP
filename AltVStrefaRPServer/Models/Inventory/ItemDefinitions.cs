using System.Collections.Generic;
using AltV.Net.Enums;
using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class ItemDefinitions
    {
        public static Dictionary<ItemType, BaseItem> Items { get; set; } = new Dictionary<ItemType, BaseItem>
        {
            { ItemType.Weapon_CombatPistol, new WeaponItem
            {
                Name = "Combat pistol",
                Model = "w_pi_combatpistol",
                StackSize = 1,
                Ammo = 20,
                Slot = EquipmentSlot.LeftHand,
                WeaponModel = WeaponModel.CombatPistol
            }},
            { ItemType.FoodItem_Burger, new FoodItem("Burger", 20, "prop_bodyarmour_04", 20) }
        };
    }

    public enum ItemType
    {
        Weapon_CombatPistol = 100,
        FoodItem_Burger = 1000,
    }
}
