using AltV.Net.NetworkingEntity.Elements.Entities;
using AltVStrefaRPServer.Models.Core;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Server;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface INetworkingManager : IManager<NetworkingEntity>
    {
        void InitializeNetworkingManager(AppSettings appSettings);
        bool TryGetNetworkingEntity(int networkignEntityId, out INetworkingEntity networkingEntity);
        bool DoesNetworkingEntityExists(int networkingEntityId);
        INetworkingEntity AddNewDroppedItem(DroppedItem droppedItem, int streamingRange = 50, int dimension = 0);
        void AddNewMarker(Marker marker);
        void DescreaseDroppedItemQuantity(int networkingItemId, int itemsToRemove);
        bool RemoveNetworkingEntity(int networkingItemId);
    }
}