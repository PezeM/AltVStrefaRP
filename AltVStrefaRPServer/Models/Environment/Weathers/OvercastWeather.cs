namespace AltVStrefaRPServer.Models.Environment.Weathers
{
    public class OvercastWeather : IWeather
    {
        public uint Weather { get; } = (uint) Enums.Weathers.Overcast;
        public uint PreviousWeather { get; }

        public OvercastWeather(uint previousWeather)
        {
            PreviousWeather = previousWeather;
        }

        public IWeather GetNextWeather(int weatherChance)
        {
            if (weatherChance < 50)
            {
                return new ExtraSunnyWeather(Weather);
            } 
            else if (weatherChance < 70)
            {
                return new CloudsWeather(Weather);
            }
            else if (weatherChance < 85)
            {
                return new LightRainWeather(Weather);
            }
            else if(weatherChance < 95)
            {
                return new NeutralWeather(Weather);
            }
            else if (weatherChance < 100)
            {
                return new ClearWeather(Weather);
            }

            return new ExtraSunnyWeather(Weather);
        }
    }
}