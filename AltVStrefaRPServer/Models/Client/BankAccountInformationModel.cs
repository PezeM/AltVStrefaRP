namespace AltVStrefaRPServer.Models.Client
{
    public class BankAccountInformationModel
    {
        public string Name { get; }
        public int AccountNumber { get; }
        public float Money { get; }

        public BankAccountInformationModel(string name, int accountNumber, float money)
            Name = name;
            AccountNumber = accountNumber;
            Money = money;
        }
    }
}
