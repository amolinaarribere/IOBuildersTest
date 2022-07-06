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
    public class TransferControllerTests
    {
        private readonly TransferController _controller;
        private BankContext _db;

        public TransferControllerTests()
        {
            var Init = new Init<TransferController>();

            _db = Init.createContext();

            _controller = new TransferController(Init.createLogger(), _db, Init.createBankContract());

            Init.FillDB(_db);
        }
   
        
        [TestMethod]
        public async Task PostAccounts()
        {
            // Arrange
            List<Account> accounts = _db.Accounts.ToListAsync().Result;
            List<int> numberOfTransfers = new List<int>();

            // Act
            int TotalAmount = 0;
            int j = 1;
            foreach (Account account in accounts)
            {
                TotalAmount += int.Parse(account.amount);
                numberOfTransfers.Add(_db.Transfers.Where(t => t.addressSender == account.address || t.addressReceiver == account.address).Count());
            }

            foreach (Account account in accounts)
            {
                TransferInputDTO transferInfo = new TransferInputDTO();
                transferInfo.addressSender = account.address;
                transferInfo.addressReceiver = accounts[j % accounts.Count].address;
                transferInfo.amount = account.amount;

                await _controller.Post(transferInfo);
                j++;
            }

            // Assert
            int i = 0;
            foreach(Account account in accounts)
            {
                Assert.AreEqual((i == 0)? TotalAmount : 0, int.Parse(account.amount));
                Assert.AreEqual(numberOfTransfers[i++] + 2, _db.Transfers.Where(t => t.addressSender == account.address || t.addressReceiver == account.address).Count());
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
