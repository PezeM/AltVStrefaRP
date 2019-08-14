using AltV.Net.Enums;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Services.Inventories
{
    public class ItemFactory
    {
        public BaseItem CreateClothItem(string name, EquipmentSlot slot, int drawableId, int textureId, int paletteId, bool isProp = false, 
            string model = "prop_ld_tshirt_02", string description = null)
        {
            return new ClothItem(name, slot, drawableId, textureId, paletteId, isProp, model, description);
        }

        public BaseItem CreateBurger(int stackSize = 20, ushort hungerValue = 20, string description = null) 
            => new FoodItem("Burger", stackSize, "prop_cs_burger_01", hungerValue, description);
        public BaseItem CreateWater(int stackSize = 10, ushort thirstValue = 10, string description = null) 
            => new DrinkItem("Woda", stackSize, "ng_proc_brkbottle_02a", thirstValue, description);
        public BaseItem CreateSprunk(int stackSize = 10, ushort thirstvalue = 13, string description = null) 
            => new DrinkItem("Sprunk", stackSize, "prop_ld_can_01", thirstvalue, description);
        // prop_ld_can_01 - sprunk can

        public WeaponItem CreatePistol(int ammo = 20) => new WeaponItem("Pistolet", "w_pi_pistol", WeaponModel.Pistol, ammo, EquipmentSlot.LeftHand);
        public WeaponItem CreateCombatPistol(int ammo = 40) => new WeaponItem("Pistolet bojowy", "w_pi_combatpistol", WeaponModel.CombatPistol, ammo, EquipmentSlot.LeftHand);
        public WeaponItem CreatePumpShotgun(int ammo = 8) => new WeaponItem("Strzelba", "w_sg_pumpshotgun", WeaponModel.PumpShotgun, ammo, EquipmentSlot.LeftHand);

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
