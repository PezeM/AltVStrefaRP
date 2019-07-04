using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Models.Interfaces.Inventory
{
    public interface IWorldItem
    {
        BaseItem GetWorldItemAsBaseItem();
    }
}
