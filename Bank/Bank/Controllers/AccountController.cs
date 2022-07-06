using Bank.DTO;
using Bank.Models;
using Bank.Blockchain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using NBitcoin;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.HdWallet;
using Microsoft.Extensions.Configuration;
using Bank.Controllers.Common;

namespace Bank.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {

        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _config;
        private BankContext _db;
        private IBankContract _bankContract;

        public AccountController(ILogger<AccountController> logger, IConfiguration config, BankContext bankContext, IBankContract bankContract)
        {
            _logger = logger;
            _config = config;
            _db = bankContext;
            _bankContract = bankContract;
        }

        // Retrieve the account plus all the transfers the account was involved in
        [HttpGet("{address}")]
        [ProducesResponseType(typeof(AccountOutputDTO), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult Get(string address)
        {
            try
            {
                AccountOutputDTO accountDTO = getAccountsDTO(address);

                if (accountDTO.accountInfo == null) return Ok(null);

                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        // Generates a new account for an existing user
        [HttpPost("{passportID}")]
        public async Task<IActionResult> Post(string passportID)
        {
            try
            {
                // Check Input parameters
                if (_db.Users.Find(passportID) == null)
                {
                    return BadRequest("User with passport " + passportID + " does not exist");
                }

                // Generate the new Wallet
                List<string> keys = generateKeys();

                // Trigger the blockchain asynchrnous task
                var activationTask = _bankContract.ActivateAccountAsync(keys[1]);

                // Adds the account to the DB while Blockchain is running
                Models.Account newAccount = generateAccount(keys, passportID);
                _db.Accounts.Add(newAccount);

                // Wait for blockchain result. If successful we commit DB changes and return success, otherwise we revert and return error.
                await waiting.waitForBlockchainOperation(activationTask);

                _db.SaveChanges();

                return Ok("account " + newAccount.address + " created for user with passport " + passportID);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        // deposits some money into a account
        [HttpPut]
        [Route("{address}/deposit/{amount}")]
        public async Task<IActionResult> Put(string address, string amount)
        {
            try
            {
                // Check Input parameters
                BigInteger amountBI = BigInteger.Parse(amount);
                Models.Account account = _db.Accounts.Find(address);

                if (account == null)
                {
                    return BadRequest("account with address " + address + " does not exist");
                }

                // Trigger the blockchain asynchrnous task
                var depositTask = _bankContract.DepositAsync(address, amount);

                // Adds the deposits to the DB while Blockchain is running
                account.amount = (BigInteger.Parse(account.amount) + amountBI).ToString();
                _db.Transfers.Add(generateDepositTransfer(address, amount));

                // Wait for blockchain result. If successful we commit DB changes and return success, otherwise we revert and return error.
                await waiting.waitForBlockchainOperation(depositTask);

                _db.SaveChanges();

                return Ok("Amount deposited in account with address " + address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        internal AccountOutputDTO getAccountsDTO(string address)
        {
            AccountOutputDTO accountDTO = new AccountOutputDTO();

            Models.Account account = _db.Accounts.Find(address);

            if (account != null)
            {
                var query = _db.Transfers.Where(t => t.addressSender == address || t.addressReceiver == address);
                List<Transfer> accountTransactions = (query != null) ? query.ToList() : null;
                accountDTO = new AccountOutputDTO(account, accountTransactions);
            }

            return accountDTO;
        }

        internal List<string> generateKeys()
        {
            List<string> keys = new List<string>();

            Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            string Password = _config.GetSection("Password").Value;

            var wallet = new Wallet(mnemo.ToString(), Password);

            keys.Add(wallet.GetAccount(0).PublicKey);
            keys.Add(wallet.GetAccount(0).Address);
            keys.Add(wallet.GetAccount(0).PrivateKey);

            return keys;
        }

        internal Models.Account generateAccount(List<string> keys, string passportID)
        {
            Models.Account newAccount = new Models.Account();
            newAccount.publicKey = keys[0];
            newAccount.address = keys[1];
            newAccount.privateKey = keys[2];
            newAccount.userPassport = passportID;
            newAccount.amount = "0";

            return newAccount;
        }

        internal Transfer generateDepositTransfer(string address, string amount)
        {
            Transfer depositTtransfer = new Transfer();
            depositTtransfer.id = Guid.NewGuid();
            depositTtransfer.addressSender = "-";
            depositTtransfer.addressReceiver = address;
            depositTtransfer.amount = amount;
            depositTtransfer.transationDate = DateTime.Now;

            return depositTtransfer;
        }

    }
}
