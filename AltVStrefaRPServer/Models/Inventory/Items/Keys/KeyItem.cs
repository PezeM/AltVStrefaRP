using AltVStrefaRPServer.Models.Inventory.Interfaces;

namespace AltVStrefaRPServer.Models.Inventory.Items.Keys
{
    public abstract class KeyItem : BaseItem, IDroppable
    {
        public string Model { get; protected set; }

        protected KeyItem(string model, string name, int stackSize) : base(name, stackSize)
        {
            Model = model;
        }
        
        public override bool UseItem(Character character) 
        {
            return true;
        }

        public override BaseItem Copy() => (KeyItem)MemberwiseClone();
    }
}