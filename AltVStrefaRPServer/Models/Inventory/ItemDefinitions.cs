﻿using System.Collections.Generic;
using AltV.Net.Enums;
using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class ItemDefinitions
    {
        public static Dictionary<ItemType, BaseItem> Items { get; set; } = new Dictionary<ItemType, BaseItem>
        {
            { ItemType.Weapon_CombatPistol, new WeaponItem("Combat pistol", "w_pi_combatpistol", WeaponModel.CombatPistol, 20, EquipmentSlot.LeftHand) },
            { ItemType.FoodItem_Burger, new FoodItem("Burger", 20, "prop_bodyarmour_04", 20) },
            { ItemType.Cloth_Pants, new ClothItem("Jakieś spodnie", EquipmentSlot.Legs, 4, 1, 2, false) }
        };

        public static BaseItem GenerateNewItem(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Cloth_Pants:
                    return new ClothItem("Jakieś spodnie", EquipmentSlot.Legs, 4, 1, 2, false);
                case ItemType.FoodItem_Burger:
                    return new FoodItem("Burger", 20, "prop_bodyarmour_04", 20);
                case ItemType.Weapon_CombatPistol:
                    return new WeaponItem("Combat pistol", "w_pi_combatpistol", WeaponModel.CombatPistol, 20, EquipmentSlot.LeftHand);
            }
            return null;
        }
    }

    public enum ItemType
    {
        Weapon_CombatPistol = 100,
        FoodItem_Burger = 1000,
        Cloth_Pants = 10000,
    }
}
