namespace AltVStrefaRPServer
{
    public class AppSettings
    {
        public static AppSettings Current;

        public AppSettings()
        {
            Current = this;
        }

        public string ConnectionString { get; set; }
        public ServerConfig ServerConfig { get; set; }
    }

    public class ServerConfig
    {
        public ConfigPosition ConfigPosition { get; set; }
        public ConfigPosition LoginPosition { get; set; }
    }

    public class ConfigPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public override string ToString()
        {
            return $"X: {X} Y: {Y} Z: {Z}";
        }
    }
}