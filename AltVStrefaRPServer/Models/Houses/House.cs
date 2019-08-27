namespace AltVStrefaRPServer.Models.Houses
{
    public class House
    {
        public int Id { get; set; }
        public Character Owner { get; set; }
    }
    

    public class Fract
    {
        public House Hosue;
    }
}
