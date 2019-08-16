using AltV.Net;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Interfaces;

namespace AltVStrefaRPServer.Services.Inventories
{
    public class InventoryItemAdapter : IMValueAdapter<InventoryItem>
    {
        public InventoryItem FromMValue(IMValueReader reader)
        {
            reader.BeginObject();
            int id = 0;
            string name = string.Empty;
            int slotId = 0;
            int quanity = 0;
            int stackSize = 0;
            bool isDroppable;
            int equipmentSlot = 0;
            string description = string.Empty;
            while (reader.HasNext())
            { 
                switch (reader.NextName())
                {
                    case "id":
                        id = reader.NextInt();
                        break;
                    case "name":
                        name = reader.NextString();
                        break;
                    case "slotId":
                        slotId = reader.NextInt();
                        break;
                    case "quantity":
                        quanity = reader.NextInt();
                        break;
                    case "stackSize":
                        stackSize = reader.NextInt();
                        break;
                    case "isDroppable":
                        isDroppable = reader.NextBool();
                        break;
                    case "equipmentSlot":
                        equipmentSlot = reader.NextInt();
                        break;
                    case "description":
                        description = reader.NextString();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
            reader.EndObject();
            return new InventoryItem(id, null, quanity, slotId);
        }

        public void ToMValue(InventoryItem value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("id");
            writer.Value(value.Id);
            writer.Name("name");
            writer.Value(value.Item.Name);
            writer.Name("slotId");
            writer.Value(value.SlotId);
            writer.Name("quantity");
            writer.Value(value.Quantity);
            writer.Name("stackSize");
            writer.Value(value.Item.StackSize);
            writer.Name("isDroppable");
            writer.Value((value.Item is IDroppable));
            writer.Name("equipmentSlot");
            writer.Value(value.Item is IEquipmentable equipmentable ? (int)equipmentable.EquipmentSlot : -1);
            writer.Name("description");
            writer.Value(value.Item.Description);
            writer.EndObject();
        }

        public void ToMValue(object obj, IMValueWriter writer)
        {
            if (obj is InventoryItem value)
            {
                ToMValue(value, writer);
            }
        }

        object IMValueBaseAdapter.FromMValue(IMValueReader reader) => FromMValue(reader);
    }
}
