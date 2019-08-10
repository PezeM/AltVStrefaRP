namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public abstract class BaseItem
    {
        public int Id { get; set; }
        public string Name { get; protected set; }
        public int StackSize { get; protected set; }
        public string Description { get; protected set; } = "Brak opisu";

        protected BaseItem(){}

        protected BaseItem(string name, int stackSize)
        {
            Name = name;
            StackSize = stackSize;
        }

        protected BaseItem(string name, int stackSize, string description)
        {
            Name = name;
            StackSize = stackSize;
            Description = description;
        }

        protected BaseItem(BaseItem baseItem)
        {
            Id = baseItem.Id;
            Name = baseItem.Name;
            StackSize = baseItem.StackSize;
            Description = baseItem.Description;
        }

        public abstract bool UseItem(Character character);

        public BaseItem ShallowClone()
        {
            return (BaseItem)MemberwiseClone();
        }

        public static T ShallowClone<T>(T original) where T : BaseItem
        {
            var newBaseItem = (T)original.ShallowClone();
            newBaseItem.Id = 0;
            return newBaseItem;
        }

        public abstract BaseItem Copy();
    }
}
