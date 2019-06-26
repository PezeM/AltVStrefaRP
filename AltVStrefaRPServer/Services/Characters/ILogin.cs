namespace AltVStrefaRPServer.Services.Characters
{
    public interface ILogin
    {
        string GeneratePassword(string password);
        bool VerifyPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        bool IsPasswordValid(string password);
    }
}
