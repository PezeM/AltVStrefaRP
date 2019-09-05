namespace AltVStrefaRPServer.Models.Environment.Weathers
{
    public class ExtraSunnyWeather : IWeather
    {
        public uint Weather { get; } = (uint)Enums.Weathers.ExtraSunny;
        public uint PreviousWeather { get; }

        public ExtraSunnyWeather(uint previousWeather)
        {
            PreviousWeather = previousWeather;
        }

        public IWeather GetNextWeather(int weatherChance)
        {
            if (weatherChance < 50)
            {
                return new ExtraSunnyWeather(Weather);
            }
            else if (weatherChance < 60)
            {
                return new ClearWeather(Weather);
            }
            else if (weatherChance < 70)
            {
                return new CloudsWeather(Weather);
            } 
            else if (weatherChance < 80)
            {
                return new OvercastWeather(Weather);
            }
            else if(weatherChance < 90)
            {
                return new NeutralWeather(Weather);
            }
            else if (weatherChance <= 100)
            {
                return new LightRainWeather(Weather);
            }

            return new ExtraSunnyWeather(Weather);
        }
    }
}
