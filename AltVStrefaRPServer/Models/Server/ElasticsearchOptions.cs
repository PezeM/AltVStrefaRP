namespace AltVStrefaRPServer.Models.Server
{
    public class ElasticsearchOptions
    {
        public string Uri { get; set; } = "http://localhost:9200/";
        public bool AutoRegisterTemplate { get; set; } = true;
        public string Username { get; set; }
        public string Password { get; set; }
    }
}