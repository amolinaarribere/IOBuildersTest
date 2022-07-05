using Bank.Models;
using System.Collections.Generic;

namespace Bank.DTO
{
    public struct WalletDTO
    {
        public CustodialWallet walletInfo { get; set; }

        public List<BankTransaction> transactions { get; set; }

        public WalletDTO(CustodialWallet _walletInfo, List<BankTransaction> _transactions)
        {
            walletInfo = _walletInfo;
            transactions = _transactions;
        }
    }
}
