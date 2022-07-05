using System;
using Bank.Models;

namespace Bank.DTO
{
    public struct TransactionDTO
    {
        public BankTransaction transactionInfo { get; set; }

        public void addTransactionId()
        {
            transactionInfo.id = Guid.NewGuid();
        }
    }
}
