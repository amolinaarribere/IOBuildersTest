using System.Threading.Tasks;

namespace Bank.Blockchain
{
    public interface IBankContract
    {
        public Task<bool> ActivateAccountAsync(string address);

        public Task<bool> DepositAsync(string address, string amount);

        public Task<bool> TransferAsync(string addressSender, string addressReceiver, string amount, string privateKeySender);
    }
}
