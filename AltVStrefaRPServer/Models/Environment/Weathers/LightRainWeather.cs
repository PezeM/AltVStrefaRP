namespace AltVStrefaRPServer.Models.Environment.Weathers
{
    public class LightRainWeather : IWeather
    {
        public uint Weather { get; } = (uint) Enums.Weathers.Lightrain;
        public uint PreviousWeather { get; }

        public LightRainWeather(uint previousWeather)
        {
            PreviousWeather = previousWeather;
        }

        public IWeather GetNextWeather(int weatherChance)
        {
            if(weatherChance < 15)
            {
                return new ExtraSunnyWeather(Weather);
            } 
            else if(weatherChance < 30)
            {
                return new ClearWeather(Weather);
            }
            else if (weatherChance < 55)
            {
                return new CloudsWeather(Weather);
            }
            else if(weatherChance < 65)
            {
                return new FoggyWeather(Weather);
            }
            else if(weatherChance < 75)
            {
                return new RainWeather(Weather);
            }
            else if (weatherChance < 80)
            {
                return new ThunderWeather(Weather);
            }
            else if (weatherChance < 90)
            {
                return new LightRainWeather(Weather);
            }
            else if(weatherChance < 95)
            {
                return new OvercastWeather(Weather);
            }
            else if(weatherChance <= 100)
            {
                return new NeutralWeather(Weather);
            }

            return new ExtraSunnyWeather(Weather);
        }
    }
}