namespace AltVStrefaRPServer.Models.Inventory.Interfaces
{
    public interface ICloth
    {
        int DrawableId { get; set; }
        int TextureId { get; set; }
        int PaletteId { get; set; }
        bool IsProp { get; set; }
    }
}
