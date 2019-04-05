using System.Threading.Tasks;

namespace AltVStrefaRPServer.Services.Characters.Customization
{
    public interface ICharacterCreatorService
    {
        Task<bool> CheckIfCharacterExistsAsync(string firstName, string lastName);
        Models.Character CreateNewCharacter(int accountId, string firstName, string lastName, int age, int gender);
        Task SaveNewCharacter(Models.Character character);
    }
}
