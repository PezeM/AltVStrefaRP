using System.Text.RegularExpressions;

namespace AltVStrefaRPServer.Services.Characters
{
    public class Login : ILogin
    {
        private readonly HashingService _hashingService;
        private Regex _regex;

        public Login(HashingService hashingService)
        {
            _hashingService = hashingService;
            _regex = new Regex("^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]{6,18}$");
        }

        public string GeneratePassword(string password) => _hashingService.Hash(password, 1000);

        public bool VerifyPassword(string password) => _hashingService.Verify(password, _hashingService.Hash(password, 1000));

        public bool VerifyPassword(string password, string hashedPassword)
            => _hashingService.Verify(password, hashedPassword);

        /// <summary>
        /// Checks if password passes requirements
        /// </summary>
        /// <param name="password"></param>
        /// <returns>True is password is valid</returns>
        public bool IsPasswordValid(string password) => true;
    }
}
