namespace AltVStrefaRPServer.Models.Dto
{
    public class CharacterSelectDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float Money { get;set; }
        public string BackgroundImage { get; set; }
        public string ProfileImage { get; set; }
        public int TimePlayed { get; set; }
    }
}
