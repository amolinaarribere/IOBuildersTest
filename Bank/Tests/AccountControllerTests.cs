using Bank.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Bank.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Bank.DTO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Tests.Common;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class AccountControllerTests
    {
        private readonly AccountController _controller;
        private BankContext _db;

        public AccountControllerTests()
        {
            var Init = new Init<AccountController>();

            _db = Init.createContext();

            _controller = new AccountController(Init.createLogger(), Init.createConfiguration(), _db, Init.createBankContract());

            Init.FillDB(_db);
        }
   
        
        [TestMethod]
        public void GetAccount()
        {
            // Arrange
            List<AccountOutputDTO> expectedOutput = generateOutput();

            // Act
            List<Account> accounts = _db.Accounts.ToListAsync().Result;
            List<AccountOutputDTO> usersOutput = new List<AccountOutputDTO>();

            foreach (Account account in accounts)
            {
                var result = _controller.Get(account.address) as OkObjectResult;
                var resultContent = result.Value as AccountOutputDTO?;
                usersOutput.Add((AccountOutputDTO)resultContent);
            }

            // Assert
            CheckAccountsOutput(expectedOutput, usersOutput);
        }

        
        [TestMethod]
        public async Task PostAccounts()
        {
            // Arrange
            List<User> users = _db.Users.ToListAsync().Result;
            List<int> numberOfAccounts = new List<int>();

            // Act
            foreach (User user in users)
            {
                numberOfAccounts.Add(_db.Accounts.Where(w => w.userPassport == user.passportId).Count());
                await _controller.Post(user.passportId);
            }

            // Assert
            int i = 0;
            foreach(User user in users)
            {
                Assert.AreEqual(numberOfAccounts[i++] + 1, _db.Accounts.Where(w => w.userPassport == user.passportId).Count());
            }
        }

        [TestMethod]
        public async Task PutAccounts()
        {
            // Arrange
            int fixAmount = 25;
            List<Account> accounts = _db.Accounts.ToListAsync().Result;
            List<int> accountFunds = new List<int>();

            // Act
            int j = 0;
            foreach (Account account in accounts)
            {
                accountFunds.Add(int.Parse(account.amount));
                await _controller.Put(account.address, (fixAmount * (j+1)).ToString());
                j++;
            }

            // Assert
            int i = 0;
            foreach (Account account in accounts)
            {
                int expectedAmount = accountFunds[i] + (fixAmount * (i + 1));
                Assert.AreEqual(expectedAmount, int.Parse(_db.Accounts.Find(account.address).amount));
                i++;
            }
        }


        public List<AccountOutputDTO> generateOutput()
        {
            List<AccountOutputDTO> output = new List<AccountOutputDTO>();

            List<Account> accounts = _db.Accounts.ToListAsync().Result;

            foreach (Account account in accounts)
            {
                var query = _db.Transfers.Where(t => t.addressSender == account.address || t.addressReceiver == account.address);
                List<Transfer> accountTransactions = (query != null) ? query.ToList() : null;

                output.Add(new AccountOutputDTO(account, accountTransactions));
            }

            return output;
        }

        public bool CheckAccountOutput(AccountOutputDTO expectedAccountOutput, AccountOutputDTO actualAccountOutput)
        {
            if(!Checks.CheckAccount(expectedAccountOutput.accountInfo, actualAccountOutput.accountInfo)) return false;

            if (expectedAccountOutput.transfers.Count != actualAccountOutput.transfers.Count) return false;

            int i = 0;

            foreach(Transfer transfer in expectedAccountOutput.transfers)
            {
                if (!Checks.CheckTransfer(transfer, actualAccountOutput.transfers[i++])) return false;
            }

            return true;
        }
        
        public void CheckAccountsOutput(List<AccountOutputDTO> expectedAccountsOutput, List<AccountOutputDTO> actualAccountsOutput)
        {
            int i = 0;
            Assert.AreEqual(expectedAccountsOutput.Count, actualAccountsOutput.Count);

            foreach (AccountOutputDTO expectedAccountOutput in expectedAccountsOutput)
            {
                Assert.IsTrue(CheckAccountOutput(expectedAccountOutput, actualAccountsOutput[i++]));
            }
        }
    }
}
