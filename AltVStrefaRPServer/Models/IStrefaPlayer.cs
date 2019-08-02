using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Models
{
    public interface IStrefaPlayer : IPlayer
    {
        int AccountId { get;set; }
    }
}
