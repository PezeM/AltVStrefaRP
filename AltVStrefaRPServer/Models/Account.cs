using System.Collections.Generic;

namespace AltVStrefaRPServer.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public AdminLevel AdminLevel { get; set; }

        public ICollection<Character> Characters { get; set; }
    }
}
