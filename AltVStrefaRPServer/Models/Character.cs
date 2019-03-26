namespace AltVStrefaRPServer.Models
{
    public class Character
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BackgroundImage { get; set; }
        public string ProfileImage { get; set; }
        public int Age { get; set; }
        public float Money { get; set; }
        public int TimePlayed { get; set; }

        public string GetFullName()
        {
            return string.Join(' ', FirstName, LastName);
        }
    }
}
