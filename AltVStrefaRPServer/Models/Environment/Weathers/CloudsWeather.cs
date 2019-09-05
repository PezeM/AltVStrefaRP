namespace AltVStrefaRPServer.Models.Environment.Weathers
{
    public class CloudsWeather : IWeather
    {
        public uint Weather { get; } = (uint) Enums.Weathers.Clouds;
        public uint PreviousWeather { get; }

        public CloudsWeather(uint previousWeather)
        {
            PreviousWeather = previousWeather;
        }

        public IWeather GetNextWeather(int weatherChance)
        {
            if(weatherChance < 30)
            {
                return new ExtraSunnyWeather(Weather);
            }
            else if(weatherChance < 55)
            {
                return new ClearWeather(Weather);
            } 
            else if(weatherChance < 60)
            {
                return new NeutralWeather(Weather);
            }
            else if(weatherChance < 65)
            {
                return new LightRainWeather(Weather);
            }
            else if (weatherChance < 85)
            {
                return new OvercastWeather(Weather);
            }
            else if(weatherChance < 90)
            {
                return new RainWeather(Weather);
            }
            else if(weatherChance <= 100)
            {
                return new CloudsWeather(Weather);
            }

            return new ExtraSunnyWeather(Weather);
        }
    }
}