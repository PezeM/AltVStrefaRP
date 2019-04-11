namespace AltVStrefaRPServer.Models.Businesses
{
    public class PubBusiness : Business
    {
        public override byte BlipModel { get; protected set; } = 93;
        public override string BlipName { get; protected set; } = "Pub";
        public override byte BlipColor { get; protected set; } = 49;
    }
}
