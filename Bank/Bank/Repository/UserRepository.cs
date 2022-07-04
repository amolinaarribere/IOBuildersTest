using Bank.Models;
using System;
using System.Collections.Generic;

namespace Bank.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;

        public UserRepository(UserContext context)
            => _context = context;

        public void AddUser(User user)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAllUser()
        {
            throw new NotImplementedException();
        }

        public User GetUserById(Guid id)
            => _context.Users.Find(id);

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
