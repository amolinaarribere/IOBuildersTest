using System;
using System.ComponentModel.DataAnnotations;

namespace Bank.Models
{
    public class User
    {
        [Key]
        public string passportId { get; set; }

        public string name { get; set; }

        public string familyName { get; set; }

        public DateTime birthdate { get; set; }

        public string address { get; set; }

        public string password { get; set; }

        public string salt { get; set; }

    }
}
