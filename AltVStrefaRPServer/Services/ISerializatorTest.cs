namespace AltVStrefaRPServer.Services
{
    public interface ISerializatorTest
    {
        TestObject TestObject { get; set; }
        void CalculateSize(object objectToCalucalate);
    }
}
