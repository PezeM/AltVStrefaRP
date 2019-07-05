using AltVStrefaRPServer.Models.Interfaces.Inventory;

namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public class FoodItem : BaseItem, IWorldItem, IDroppable
    {
        public ushort Value { get; set; }
        public string Model { get; private set; }

        public FoodItem(string name, int stackSize, string model, ushort value) : base(name, stackSize)
        {
            Model = model;
            Value = value;
        }

        public BaseItem GetWorldItemAsBaseItem()
        {
            throw new System.NotImplementedException();
        }

        public override bool UseItem(Character character)
        {
            character.Player.Health += Value;
            return true;
        }
    }
}
