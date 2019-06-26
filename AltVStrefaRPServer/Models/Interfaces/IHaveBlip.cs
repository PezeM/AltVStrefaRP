namespace AltVStrefaRPServer.Models
{
    public interface IHaveBlip
    {
        int BlipSprite { get; }
        int BlipColor { get; }
        string BlipName { get; }
    }
}
