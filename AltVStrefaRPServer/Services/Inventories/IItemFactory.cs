using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Items.Keys;

namespace AltVStrefaRPServer.Services.Inventories
{
    public interface IItemFactory
    {
        BaseItem CreateClothItem(string name, EquipmentSlot slot, int drawableId, int textureId, int paletteId, bool isProp = false,
            string model = "prop_ld_tshirt_02", string description = null);

        BaseItem CreateBurger(int stackSize = 20, ushort hungerValue = 20, string description = null);
        BaseItem CreateWater(int stackSize = 10, ushort thirstValue = 10, string description = null);
        BaseItem CreateSprunk(int stackSize = 10, ushort thirstValue = 13, string description = null);
        WeaponItem CreatePistol(int ammo = 20);
        WeaponItem CreateCombatPistol(int ammo = 40);
        WeaponItem CreatePumpShotgun(int ammo = 8);
        HouseKeyItem CreateHouseKeyItem(string lockPatter, string model = "prop_cs_keys_01");
        BaseItem CreateItem(ItemType item);
    }
}