using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Data;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Modules.HousingModule
{
    public class HouseHandler
    {
        public HouseHandler()
        {
            Alt.OnColShape += AltOnOnColShape;
        }

        private void AltOnOnColShape(IColShape colshape, IEntity entity, bool entered)
        {
            if (!(entity is IStrefaPlayer player)) return;
            
            if(colshape.GetData(MetaData.COLSHAPE_HOUSE_ID, out int houseId)) return;

            player.HouseId = entered ? houseId : 0;
            player.Emit("inHouseEnterColshape", entered);
        }
    }
}