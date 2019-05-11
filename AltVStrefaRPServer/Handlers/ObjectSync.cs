using AltV.Net;
using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Handlers
{
    public class ObjectSync
    {
        public ObjectSync()
        {
            Alt.On<IPlayer, string>("SyncObject", SyncObjectEvent);
        }

        private void SyncObjectEvent(IPlayer player, string syncObject)
        {
            Alt.Log($"Calling sync object event on ${player.Name} with object: {syncObject}");
            player.SetSyncedMetaData("ObjectStream", syncObject);

            Alt.EmitAllClients("objectStreamUpdate", (int)player.Id, syncObject);
        }
    }
}
