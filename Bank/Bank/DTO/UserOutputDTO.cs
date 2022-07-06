using Bank.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bank.DTO
{
    public struct UserOutputDTO
    {
        public User userInfo { get; set; }

        public List<Account> accounts { get; set; }

        public UserOutputDTO(User _userInfo, List<Account> _accounts)
        {
            userInfo = _userInfo;
            accounts = _accounts;
        }
    }
}
