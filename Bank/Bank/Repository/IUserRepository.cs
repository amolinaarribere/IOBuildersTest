using Bank.Models;
using System;
using System.Collections.Generic;

namespace Bank.Repository
{
    public interface IUserRepository
    {
        User GetUserById(Guid id);

        IEnumerable<User> GetAllUser();

        void AddUser(User user);

        void SaveChanges();
    }
}
