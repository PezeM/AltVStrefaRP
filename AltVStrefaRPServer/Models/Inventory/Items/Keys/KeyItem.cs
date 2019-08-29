using AltVStrefaRPServer.Models.Inventory.Interfaces;

namespace AltVStrefaRPServer.Models.Inventory.Items.Keys
{
    public abstract class KeyItem : BaseItem, IDroppable
    {
        public string Model { get; protected set; }

        protected KeyItem(string model)
        {
            Model = model;
        }
        
        public override bool UseItem(Character character)
        {
            throw new System.NotImplementedException();
        }

        public override BaseItem Copy() => (KeyItem)MemberwiseClone();
    }
}