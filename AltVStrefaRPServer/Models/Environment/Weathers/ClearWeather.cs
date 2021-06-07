namespace AltVStrefaRPServer.Models.Environment.Weathers
{
    public class ClearWeather : IWeather
    {
        public uint Weather { get; } = (uint) Enums.Weathers.Clear;
        public uint PreviousWeather { get; }

        public ClearWeather(uint previousWeather)
        {
            PreviousWeather = previousWeather;
        }

        public IWeather GetNextWeather(int weatherChance)
        {
            if (weatherChance < 40)
            {
                return new ExtraSunnyWeather(Weather);
            }
            else if(weatherChance < 60)
            {
                return new ClearWeather(Weather);
            }
            else if(weatherChance < 75)
            {
                return new CloudsWeather(Weather);
            }
            else if(weatherChance < 85)
            {
                return new OvercastWeather(Weather);
            }
            else if (weatherChance < 95)
            {
                return new LightRainWeather(Weather);
            }
            else if(weatherChance <= 100)
            {
                return new NeutralWeather(Weather);
            }

            return new ExtraSunnyWeather(Weather);
        }
    }
}