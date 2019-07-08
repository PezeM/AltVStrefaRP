using AltVStrefaRPServer.Models.Interfaces.Inventory;
using AltVStrefaRPServer.Models.Interfaces.Items;
using AltVStrefaRPServer.Models.Inventory.Interfaces;

namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public class ClothItem : Equipmentable, ICloth, IDroppable
    {
        public int DrawableId { get; set; }
        public int TextureId { get; set; }
        public int PaletteId { get; set; }
        public bool IsProp { get; set; }
        public string Model { get; set; }

        public ClothItem(string name, EquipmentSlot slot, int drawableId, int textureId, int paletteId, bool isProp, string model = "p_cs_shirt_01_s")
            : base(name, slot)
        {
            Name = name;
            Slot = slot;
            DrawableId = drawableId;
            TextureId = textureId;
            PaletteId = paletteId;
            IsProp = isProp;
            Model = model;
        }

        public override bool UseItem(Character character)
        {
            character.Player.Emit("equipClothableItem", (int)Slot, DrawableId, TextureId, PaletteId, IsProp);
            return true;
        }

        public override BaseItem Copy()
        {
            return (ClothItem)MemberwiseClone();
        }

        // Usefull models
        // p_lazlow_shirt_s, p_laz_j01_s, prop_ld_tshirt_01, prop_ld_tshirt_02
        // p_jewel_necklace_02, p_jimmyneck_03_s, p_jewel_necklace02_s
        // p_ld_heist_bag_01
        // prop_ld_jeans_01, prop_ld_jeans_02
    }
}
