using AltV.Net;
using AltV.Net.Elements.Args;
using AltVStrefaRPServer.Models.Dto;
using AltVStrefaRPServer.Models.Inventory;
using System.Collections.Generic;

namespace AltVStrefaRPServer.Services.Inventories
{
    public class InventoryContainerAdapter : IMValueAdapter<InventoryContainerDto>
    {
        private readonly IMValueAdapter<List<InventoryItem>> _inventoryItemAdapter;

        public InventoryContainerAdapter()
        {
            _inventoryItemAdapter = DefaultMValueAdapters.GetArrayAdapter(new InventoryItemAdapter());
        }

        public InventoryContainerDto FromMValue(IMValueReader reader)
        {
            reader.BeginObject();
            int inventoryId = 0;
            string inventoryName = string.Empty;
            int inventorySlots = 0;
            List<InventoryItem> items = null;
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "inventoryId":
                        inventoryId = reader.NextInt();
                        break;
                    case "inventoryName":
                        inventoryName = reader.NextString();
                        break;
                    case "inventorySlots":
                        inventorySlots = reader.NextInt();
                        break;
                    case "items":
                        items = _inventoryItemAdapter.FromMValue(reader);
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            reader.EndObject();
            return new InventoryContainerDto
            {
                Items = items,
                InventoryId = inventoryId,
                InventorySlots = inventorySlots,
                InventoryName = inventoryName
            };
        }

        public void ToMValue(InventoryContainerDto value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("inventoryId");
            writer.Value(value.InventoryId);
            writer.Name("inventoryName");
            writer.Value(value.InventoryName);
            writer.Name("inventorySlots");
            writer.Value(value.InventorySlots);
            writer.Name("items");
            _inventoryItemAdapter.ToMValue(value.Items, writer);
            writer.EndObject();
        }

        public void ToMValue(object obj, IMValueWriter writer)
        {
            if (obj is InventoryContainerDto value)
            {
                ToMValue(value, writer);
            }
        }

        object IMValueBaseAdapter.FromMValue(IMValueReader reader) => FromMValue(reader);
    }
}
