using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Services.Inventory
{
    public class ItemFactory
    {
        public BaseItem CreateClothItem(string name, EquipmentSlot slot, int drawableId, int textureId, int paletteId, bool isProp = false, string model = "p_cs_shirt_01_s")
        {
            return new ClothItem(name, slot, drawableId, textureId, paletteId, isProp, model);
        }

        public BaseItem CreateBurger(int stackSize = 20, ushort hungerValue = 20) => new FoodItem("Burger", stackSize, "prop_cs_burger_01", hungerValue);
        public BaseItem CreateWater(int stackSize = 10, ushort thirstValue = 10) => new FoodItem("Woda", stackSize, "ng_proc_brkbottle_02a", thirstValue);
        public BaseItem CreateSprunk(int stackSize = 10, ushort thirstvalue = 13) => new FoodItem("Sprunk", stackSize, "ng_proc_sodacan_01b", thirstvalue);

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
                default:
                    return null;
            }
        }
    }
}
