namespace AltVStrefaRPServer.Models.Environment.Weathers
{
    public class FoggyWeather : IWeather
    {
        public uint Weather { get; } = (uint) Enums.Weathers.Foggy;
        public uint PreviousWeather { get; }

        public FoggyWeather(uint previousWeather)
        {
            PreviousWeather = previousWeather;
        }

        public IWeather GetNextWeather(int weatherChance)
        {
            if(weatherChance < 15)
            {
                return new ExtraSunnyWeather(Weather);
            }
            else if (weatherChance < 35)
            {
                return new ClearWeather(Weather);
            }
            else if(weatherChance < 60)
            {
                return new CloudsWeather(Weather);
            }
            else if(weatherChance < 70)
            {
                return new OvercastWeather(Weather);
            }
            else if (weatherChance < 80)
            {
                return new LightRainWeather(Weather);
            }
            else if (weatherChance < 85)
            {
                return new RainWeather(Weather);
            }
            else if(weatherChance < 95)
            {
                return new FoggyWeather(Weather);
            }
            else if(weatherChance < 100)
            {
                return new NeutralWeather(Weather);
            }

            return new ExtraSunnyWeather(Weather);
        }
    }
}