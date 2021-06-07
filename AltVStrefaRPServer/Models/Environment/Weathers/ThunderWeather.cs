namespace AltVStrefaRPServer.Models.Environment.Weathers
{
    public class ThunderWeather : IWeather
    {
        public uint Weather { get; } = (uint) Enums.Weathers.Thunderstorm;
        public uint PreviousWeather { get; }

        public ThunderWeather(uint previousWeather)
        {
            PreviousWeather = previousWeather;
        }

        public IWeather GetNextWeather(int weatherChance)
        {
            if (weatherChance < 85)
            {
                return new LightRainWeather(Weather);
            }
            else if(weatherChance < 95)
            {
                return new RainWeather(Weather);
            }
            else if(weatherChance <= 100)
            {
                return new ThunderWeather(Weather);
            }

            return new ExtraSunnyWeather(Weather);
        }
    }
}