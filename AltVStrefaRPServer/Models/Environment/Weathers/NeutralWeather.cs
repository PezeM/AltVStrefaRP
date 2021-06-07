namespace AltVStrefaRPServer.Models.Environment.Weathers
{
    public class NeutralWeather : IWeather
    {
        public uint Weather { get; } = (uint) Enums.Weathers.SmoggyLightRain;
        public uint PreviousWeather { get; }

        public NeutralWeather(uint previousWeather)
        {
            PreviousWeather = previousWeather;
        }

        public IWeather GetNextWeather(int weatherChance)
        {
            if(weatherChance < 30)
            {
                return new ExtraSunnyWeather(Weather);
            }
            else if (weatherChance < 50)
            {
                return new ClearWeather(Weather);
            }
            else if(weatherChance < 70)
            {
                return new CloudsWeather(Weather);
            } 
            else if(weatherChance < 80)
            {
                return new OvercastWeather(Weather);
            }
            else if(weatherChance < 95)
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