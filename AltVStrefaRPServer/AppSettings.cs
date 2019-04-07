namespace AltVStrefaRPServer
{
    public class AppSettings
    {
        public static AppSettings Current;

        public void Initialize()
        {
            Current = this;
        }

        public string ConnectionString { get; set; }
        public ServerConfig ServerConfig { get; set; }
    }

    public class ServerConfig
    {
        public ConfigPosition SpawnPosition { get; set; }
        public ConfigPosition LoginPosition { get; set; }
        public float StartingMoney { get; set; }
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