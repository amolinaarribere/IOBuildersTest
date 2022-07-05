using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bank.Models
{
    public class CustodialWallet
    {
        [Key]
        public string publicKey { get; set; }

        public string address { get; set; }

        public string privateKey { get; set; }

        [ForeignKey("User")]
        public string userPassport { get; set; }

        public string amount { get; set; }
    }
}
