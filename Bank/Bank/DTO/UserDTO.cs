using Bank.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bank.DTO
{
    public struct UserDTO
    {
        public User userInfo { get; set; }

        public List<CustodialWallet> wallets { get; set; }

        public UserDTO(User _userInfo, List<CustodialWallet> _wallets)
        {
            userInfo = _userInfo;
            wallets = _wallets;
        }
    }
}
