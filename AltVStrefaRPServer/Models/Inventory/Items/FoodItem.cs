using AltVStrefaRPServer.Extensions;

namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public class FoodItem : Consumable
    {
        public FoodItem(string name, int stackSize, string model, ushort value, string description) : base(name, stackSize, model, value)
        {
            Description = !description.IsNullOrEmpty() ? description : $"{Name} napełnia {Value} głodu";
        }

        public override bool UseItem(Character character)
        {
            character.Player.Health += Value;
            return true;
        }

        public override BaseItem Copy() => (FoodItem)MemberwiseClone();
    }
}
