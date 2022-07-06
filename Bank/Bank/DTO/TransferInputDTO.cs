using System;
using Bank.Models;

namespace Bank.DTO
{
    public struct TransferInputDTO
    {
        public string addressSender { get; set; }

        public string addressReceiver { get; set; }

        public string amount { get; set; }

    }
}
