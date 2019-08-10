using AltVStrefaRPServer.Extensions;

namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public class DrinkItem : Consumable
    {
        public DrinkItem(string name, int stackSize, string model, ushort value, string description) : base(name, stackSize, model, value)
        {
            Description = !description.IsNullOrEmpty() ? description : $"{Name} napełnia {Value} pragnienia";
        }

        public override bool UseItem(Character character)
        {
            // Decrease thirst or someshit
            return true;
        }

        public override BaseItem Copy() => (DrinkItem)MemberwiseClone();
    }
}
