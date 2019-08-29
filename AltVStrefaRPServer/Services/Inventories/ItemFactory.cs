using AltV.Net.Enums;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Services.Inventories
{
    public class ItemFactory
    {
        public BaseItem CreateClothItem(string name, EquipmentSlot slot, int drawableId, int textureId, int paletteId, bool isProp = false,
            string model = "prop_ld_tshirt_02", string description = null)
        {
            var clothItem = new ClothItem(name, slot, drawableId, textureId, paletteId, isProp, model);
            clothItem.ChangeItemDescription(!description.IsNullOrEmpty() ? description : $"Ciuch o nazwie {name} typu {drawableId} z tekstura {textureId}");
            return clothItem;
        }

        public BaseItem CreateBurger(int stackSize = 20, ushort hungerValue = 20, string description = null)
        {
            var foodItem = new FoodItem("Burger", stackSize, "prop_cs_burger_01", hungerValue);
            foodItem.ChangeItemDescription(!description.IsNullOrEmpty() ? description : $"Burger odnawiający {hungerValue} głodu");
            return foodItem;
        }

        public BaseItem CreateWater(int stackSize = 10, ushort thirstValue = 10, string description = null)
        {
            var waterItem =  new DrinkItem("Woda", stackSize, "ng_proc_brkbottle_02a", thirstValue);
            waterItem.ChangeItemDescription(!description.IsNullOrEmpty() ? description : $"Woda odnawiający {thirstValue} napojenia");
            return waterItem;
        }

        // prop_ld_can_01 - sprunk can
        public BaseItem CreateSprunk(int stackSize = 10, ushort thirstValue = 13, string description = null)
        {
            var sprunkItem = new DrinkItem("Sprunk", stackSize, "prop_ld_can_01", thirstValue);
            sprunkItem.ChangeItemDescription(!description.IsNullOrEmpty() ? description : $"Sprunk odnawiający {thirstValue} napojenia");
            return sprunkItem;
        }

        public WeaponItem CreatePistol(int ammo = 20)
        {
            var weaponItem = new WeaponItem("Pistolet", "w_pi_pistol", WeaponModel.Pistol, ammo, EquipmentSlot.LeftHand);
            weaponItem.ChangeItemDescription("Najprostszy możliwy pistolet");
            return weaponItem;
        }

        public WeaponItem CreateCombatPistol(int ammo = 40)
        {
            var weaponItem =  new WeaponItem("Pistolet bojowy", "w_pi_combatpistol", WeaponModel.CombatPistol, ammo, EquipmentSlot.LeftHand);
            weaponItem.ChangeItemDescription("Ulepszona wersja pistoletu, potężny pistolet bojowy.");
            return weaponItem;
        }

        public WeaponItem CreatePumpShotgun(int ammo = 8)
        {
            var weaponItem = new WeaponItem("Strzelba", "w_sg_pumpshotgun", WeaponModel.PumpShotgun, ammo, EquipmentSlot.LeftHand);
            weaponItem.ChangeItemDescription("Podstawowa strzelba");
            return weaponItem;
        }

        public BaseItem CreateItem(ItemType item)
        {
            switch (item)
            {
                case ItemType.Cloth_Pants:
                    return CreateClothItem("Spodnie", EquipmentSlot.Legs, 15, 1, 1);
                case ItemType.FoodItem_Burger:
                    return CreateBurger();
                case ItemType.FoodItem_Water:
                    return CreateWater();
                case ItemType.FoodItem_Sprunk:
                    return CreateSprunk();
                case ItemType.Weapon_CombatPistol:
                    return CreateCombatPistol();
                case ItemType.Weapon_Pistol:
                    return CreatePistol();
                case ItemType.Weapon_PumpShotgun:
                    return CreatePumpShotgun();

                default:
                    return null;
            }
        }
    }
}
