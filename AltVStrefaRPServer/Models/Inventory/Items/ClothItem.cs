using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models.Inventory.Interfaces;

namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public class ClothItem : Equipmentable, ICloth, IDroppable
    {
        public int DrawableId { get; set; }
        public int TextureId { get; set; }
        public int PaletteId { get; set; }
        public bool IsProp { get; set; }
        public string Model { get; private set; }

        public ClothItem(string name, EquipmentSlot equipmentSlot, int drawableId, int textureId, int paletteId, bool isProp, string model, string description = null)
            : base(name, equipmentSlot)
        {
            DrawableId = drawableId;
            TextureId = textureId;
            PaletteId = paletteId;
            IsProp = isProp;
            Model = model;
            Description = !description.IsNullOrEmpty() ? description : $"{Name} Typ {DrawableId} Rodzaj {TextureId}";
        }

        public override bool UseItem(Character character)
        {
            character.Player.Emit("equipClothableItem", (int)EquipmentSlot, DrawableId, TextureId, PaletteId, IsProp);
            return true;
        }

        public override bool DeequipItem(Character character)
        {
            throw new System.NotImplementedException();
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
