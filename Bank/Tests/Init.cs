using Bank.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tests
{
    public class Init<T>
    {
        public void FillDB(BankContext _db)
        {
            if (_db.Users.CountAsync().Result == 0)
            {
                var TestUsers = File.ReadAllText(@"../../../Files/TestUsers.json");
                var TestAccounts = File.ReadAllText(@"../../../Files/TestAccounts.json");
                var TestTransfers = File.ReadAllText(@"../../../Files/TestTransfers.json");

                List<User> users = JsonConvert.DeserializeObject<List<User>>(TestUsers);
                List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(TestAccounts);
                List<Transfer> transfers = JsonConvert.DeserializeObject<List<Transfer>>(TestTransfers);

                _db.Users.AddRange(users);
                _db.Accounts.AddRange(accounts);
                _db.Transfers.AddRange(transfers);

                _db.SaveChanges();
            }

        }

        public ILogger<T> createLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            return loggerFactory.CreateLogger<T>();
        }

        public IConfiguration createConfiguration()
        {
            var myConfiguration = new Dictionary<string, string>
                {
                    {"Password", "S3cretPassword"}
                };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();
        }

        public BankContext createContext()
        {
            var builder = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(databaseName: "BankDatabase");
            return new BankContext(builder.Options);
        }
    }
}
