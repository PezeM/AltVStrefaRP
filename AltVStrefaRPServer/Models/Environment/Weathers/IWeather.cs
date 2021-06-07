namespace AltVStrefaRPServer.Models.Environment.Weathers
{
    public interface IWeather
    {
        uint Weather { get; }
        uint PreviousWeather { get; }
        IWeather GetNextWeather(int weatherChance);
    }
}
