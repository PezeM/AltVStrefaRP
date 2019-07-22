namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public abstract class BaseItem
    {
        public int Id { get; set; }
        public string Name { get; protected set; }
        public int StackSize { get; protected set; }

        protected BaseItem(){}

        public BaseItem(string name, int stackSize)
        {
            Name = name;
            StackSize = stackSize;
        }

        public BaseItem(BaseItem baseItem)
        {
            Id = baseItem.Id;
            Name = baseItem.Name;
            StackSize = baseItem.StackSize;
        }

        public abstract bool UseItem(Character character);

        public BaseItem ShallowClone()
        {
            return (BaseItem)MemberwiseClone();
        }

        public static T ShallowClone<T>(T original) where T : BaseItem
        {
            return (T)original.ShallowClone();
        }

        public abstract BaseItem Copy();
    }
}
