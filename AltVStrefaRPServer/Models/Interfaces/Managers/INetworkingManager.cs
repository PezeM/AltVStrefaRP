using AltV.Net.NetworkingEntity.Elements.Entities;
using AltVStrefaRPServer.Models.Inventory;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface INetworkingManager : IManager<NetworkingEntity>
    {
        bool TryGetNetworkingEntity(int networkignEntityId, out INetworkingEntity networkingEntity);
        bool DoesNetworkingEntityExists(int networkingEntityId);
        INetworkingEntity AddNewDroppedItem(DroppedItem droppedItem, int streamingRange = 50, int dimension = 0);
        void DescreaseDroppedItemQuantity(int networkingItemId, int itemsToRemove);
        bool RemoveNetworkingEntity(int networkingItemId);
    }
}