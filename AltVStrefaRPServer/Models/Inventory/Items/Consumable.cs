using AltVStrefaRPServer.Models.Inventory.Interfaces;

namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public abstract class Consumable : BaseItem, IDroppable
    {
        public ushort Value { get; protected set; }
        public string Model { get; protected set; }

        public Consumable(string name, int stackSize, string model, ushort value) : base(name, stackSize)
        {
            Model = model;
            Value = value;
        }

        public override bool UseItem(Character character)
        {
            throw new System.NotImplementedException();
        }

        public override BaseItem Copy() => (Consumable)MemberwiseClone();
    }
}
