using Bank.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Bank.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace Tests
{
    [TestClass]
    public class UserControllerTests
    {
        private readonly UserController _controller;
        private BankContext _db;

        public UserControllerTests()
        {
            var builder = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(databaseName: "BankDatabase");
            _db = new BankContext(builder.Options);
            _controller = new UserController(_db);
        }

        [TestMethod]
        public void GetUsers()
        {
            // Arrange
            FillDB();

            // Act
            var result = _controller.Get();

            // Assert

        }


        public void FillDB()
        {
            var TestUsers = File.ReadAllText(@"../../../Files/TestUsers.json");
            var TestWallets = File.ReadAllText(@"../../../Files/TestWallets.json");
            var TestTransactions = File.ReadAllText(@"../../../Files/TestTransactions.json");

            List<User> users = JsonConvert.DeserializeObject<List<User>>(TestUsers);
            List<CustodialWallet> wallets = JsonConvert.DeserializeObject<List<CustodialWallet>>(TestWallets);
            List<BankTransaction> transactions = JsonConvert.DeserializeObject<List<BankTransaction>>(TestTransactions);

            _db.Users.AddRange(users);
            _db.Wallets.AddRange(wallets);
            _db.Transactions.AddRange(transactions);

            _db.SaveChanges();

        }

    }
}
