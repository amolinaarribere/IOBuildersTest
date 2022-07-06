using Bank.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bank.Models
{
    public class User
    {
        [Key]
        public string passportId { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string familyName { get; set; }

        public DateTime birthdate { get; set; }

        public string address { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        public string salt { get; set; }

    }
}
