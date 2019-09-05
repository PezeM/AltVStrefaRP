namespace AltVStrefaRPServer.Models.Environment.Weathers
{
    public class RainWeather : IWeather
    {
        public uint Weather { get; } = (uint) Enums.Weathers.Rain;
        public uint PreviousWeather { get; }

        public RainWeather(uint previousWeather)
        {
            PreviousWeather = previousWeather;
        }

        public IWeather GetNextWeather(int weatherChance)
        {
            if(weatherChance < 70)
            {
                return new LightRainWeather(Weather);
            }
            else if (weatherChance < 80)
            {
                return new OvercastWeather(Weather);
            }
            else if(weatherChance < 90)
            {
                return new ThunderWeather(Weather);
            }
            else if (weatherChance <= 100)
            {
                return new RainWeather(Weather);
            }

            return new ExtraSunnyWeather(Weather);
        }
    }
}