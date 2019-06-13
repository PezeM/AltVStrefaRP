namespace AltVStrefaRPServer.Models.Businesses
{
    public class PubBusiness : Business
    {
        public override int BlipSprite { get; protected set; } = 93;
        public override string BlipName { get; protected set; } = "Pub";
        public override int BlipColor { get; protected set; } = 49;
    }
}
