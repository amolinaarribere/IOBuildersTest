using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bank.Models
{
    public class Account
    {
        [Key]
        public string address { get; set; }

        [Required]
        public string publicKey { get; set; }

        [Required]
        public string privateKey { get; set; }

        [Required]
        public string userPassport { get; set; }

        [Required]
        public string amount { get; set; }

    }
}
