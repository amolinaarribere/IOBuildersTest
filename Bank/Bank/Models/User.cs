using System;
using System.ComponentModel.DataAnnotations;

namespace Bank.Models
{
    public class User
    {
        [Key]
        public Guid id { get; set; }
        public string name { get; set; }
        public string familyName { get; set; }
    }
}
