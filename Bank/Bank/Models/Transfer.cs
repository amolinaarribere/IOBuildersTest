using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Bank.Models
{
    public class Transfer
    {
        [Key]
        public Guid id { get; set; }

        [Required]
        public string addressSender { get; set; }

        [Required]
        public string addressReceiver { get; set; }

        [Required]
        public string amount { get; set; }

        [Required]
        public DateTime transationDate { get; set; }

    }
}
