using AltV.Net.Enums;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Services.Inventory
{
    public class ItemFactory
    {
        public BaseItem CreateClothItem(string name, EquipmentSlot slot, int drawableId, int textureId, int paletteId, bool isProp = false, string model = "prop_ld_tshirt_02")
        {
            return new ClothItem(name, slot, drawableId, textureId, paletteId, isProp, model);
        }

        public BaseItem CreateBurger(int stackSize = 20, ushort hungerValue = 20) => new FoodItem("Burger", stackSize, "prop_cs_burger_01", hungerValue);
        public BaseItem CreateWater(int stackSize = 10, ushort thirstValue = 10) => new FoodItem("Woda", stackSize, "ng_proc_brkbottle_02a", thirstValue);
        public BaseItem CreateSprunk(int stackSize = 10, ushort thirstvalue = 13) => new FoodItem("Sprunk", stackSize, "ng_proc_sodacan_01b", thirstvalue);

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
