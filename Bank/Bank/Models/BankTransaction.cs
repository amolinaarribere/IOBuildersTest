using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Bank.Models
{
    public class BankTransaction
    {
        [Key]
        public Guid id { get; set; }

        [ForeignKey("Wallet")]
        public string publicKeySender { get; set; }

        [ForeignKey("Wallet")]
        public string publicKeyReceiver { get; set; }

        public string amount { get; set; }

        public DateTime transationDate { get; set; }

    }
}
