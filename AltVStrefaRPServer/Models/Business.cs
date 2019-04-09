using AltV.Net.Data;

namespace AltVStrefaRPServer.Models
{
    public class Business : IMoney
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Title { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Money { get; set; }

        public Position GetPosition()
        {
            return new Position(X,Y,Z);
        }
    }
}
