using System.Threading;
using System.Threading.Tasks;

namespace Bank.Blockchain
{
    public class BankContract : IBankContract
    {
        public async Task<bool> ActivateAccountAsync(string address)
        {
            await Task.Run(() => Thread.Sleep(10000));
            return true;
        }

        public async Task<bool> DepositAsync(string address, string amount)
        {
            await Task.Run(() => Thread.Sleep(10000));
            return true;
        }

        public async Task<bool> TransferAsync(string addressSender, string addressReceiver, string amount)
        {
            await Task.Run(() => Thread.Sleep(10000));
            return true;
        }
    }
}
