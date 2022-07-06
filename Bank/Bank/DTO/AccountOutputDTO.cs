using Bank.Models;
using System.Collections.Generic;

namespace Bank.DTO
{
    public struct AccountOutputDTO
    {
        public Account accountInfo { get; set; }

        public List<Transfer> transfers { get; set; }

        public AccountOutputDTO(Account _accountInfo, List<Transfer> _transfers)
        {
            accountInfo = _accountInfo;
            transfers = _transfers;
        }
    }
}
