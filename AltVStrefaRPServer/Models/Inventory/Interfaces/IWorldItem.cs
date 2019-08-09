using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Models.Inventory.Interfaces
{
    public interface IWorldItem
    {
        BaseItem GetWorldItemAsBaseItem();
    }
}
